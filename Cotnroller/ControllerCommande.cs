using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebservicesSage.Utils;
using WebservicesSage.Utils.Enums;
using WebservicesSage.Singleton;
using Objets100cLib;
using WebservicesSage.Object;
using WebservicesSage.Object.DBObject;
using System.Windows.Forms;
using LiteDB;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace WebservicesSage.Cotnroller
{
    public static class ControllerCommande
    {

        /// <summary>
        /// Lance le service de check des nouvelles commandes prestashop
        /// Définir le temps de passage de la tâche dans la config
        /// </summary>
        public static void LaunchService()
        {
            // SingletonUI.Instance.LogBox.Invoke((MethodInvoker)(() => SingletonUI.Instance.LogBox.AppendText("Commande Services Launched " + Environment.NewLine)));

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(CheckForNewOrder);
            timer.Interval = UtilsConfig.CronTaskCheckForNewOrder;
            timer.Enabled = true;
            
            System.Timers.Timer timerUpdateStatut = new System.Timers.Timer();
            timerUpdateStatut.Elapsed += new ElapsedEventHandler(UpdateStatuOrder);
            timerUpdateStatut.Interval = UtilsConfig.CronTaskUpdateStatut;
            timerUpdateStatut.Enabled = true;
            
            
        }
        public static void CheckForNewOrderClick()
        {
            int done=0;
            int notdone = 0;
            string currentIdOrder = "0";
            try
            {
                string response = UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "checkOrder");
                if (!response.Equals("none") && !response.Equals("[]"))
                {
                    JArray orders = JArray.Parse(response);
                    List<int> currentCustomer_IDs = new List<int>();
                    foreach (var order in orders)
                    {
                        currentIdOrder = order["id_order"].ToString();
                        if (ControllerClient.CheckIfClientExist(order["ALTAIS_CT_NUM"].ToString()))
                        {
                            // si le client existe on associé la commande à son compte
                            AddNewOrderForCustomer(order, order["ALTAIS_CT_NUM"].ToString());
                        }
                        else
                        {
                            // si le client n'existe pas on récupère les info de presta et on le crée dans la base sage 
                            string client = UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "getClient&clientID=" + order["id_customer"] + "&adressID=" + order["id_address"].ToString());
                            string ct_num = ControllerClient.CreateNewClient(client, order);

                            if (!String.IsNullOrEmpty(ct_num))
                            {
                                // le client à bien été crée on peut intégrer la commande sur son compte sage
                                AddNewOrderForCustomer(order, ct_num);
                            }
                        }
                        done++;
                    }
                }
                if (done > 0)
                {
                    MessageBox.Show("Finalisation d'importation de "+done.ToString()+" commandes", "end",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Aucune commande à importer", "end",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            catch (Exception s)
            {
                notdone++;
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "COMMANDE");
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
                UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + currentIdOrder);
                MessageBox.Show("Erreur avec la commande : "+currentIdOrder.ToString(), "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);
            }

        }
        /// <summary>
        /// Event levé par une nouvelle commande dans prestashop
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public static void CheckForNewOrder(object source, ElapsedEventArgs e)
        {

            string currentIdOrder = "0";
            try
            {
                string response = UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "checkOrder");
                if (!response.Equals("none") && !response.Equals("[]"))
                {
                    JArray orders = JArray.Parse(response);
                    List<int> currentCustomer_IDs = new List<int>();
                    foreach (var order in orders)
                    {
                        currentIdOrder = order["id_order"].ToString();
                        if (ControllerClient.CheckIfClientExist(order["ALTAIS_CT_NUM"].ToString()))
                        {
                            // si le client existe on associé la commande à son compte
                            AddNewOrderForCustomer(order, order["ALTAIS_CT_NUM"].ToString());
                        }
                        else
                        {
                            // si le client n'existe pas on récupère les info de presta et on le crée dans la base sage 
                            string client = UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "getClient&clientID=" + order["id_customer"] + "&adressID=" + order["id_address"].ToString());
                            string ct_num = ControllerClient.CreateNewClient(client, order);

                            if (!String.IsNullOrEmpty(ct_num))
                            {
                                // le client à bien été crée on peut intégrer la commande sur son compte sage
                                AddNewOrderForCustomer(order, ct_num);
                            }
                        }
                    }
                }
            }
            catch (Exception s)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now+ s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "COMMANDE");
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
                UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + currentIdOrder);
            }

        }

        public static void UpdateStatuOrder(object source, ElapsedEventArgs e)
        {
            /*try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var compta = SingletonConnection.Instance.Compta;

                IBICollection AllOrders = gescom.FactoryDocumentVente.List;
                using (var db = new LiteDatabase(@"MyData.db"))
                {
                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<LinkedCommandeDB>("Commande");
                    foreach (LinkedCommandeDB item in col.FindAll())
                    {
                        foreach (IBODocumentVente3 order in AllOrders)
                        {
                            if (order.DO_Ref.Equals(item.DO_Ref))
                            {
                                if (!(order.DO_Type == item.OrderType))
                                {
                                    switch (order.DO_Type)
                                    {
                                        case DocumentType.DocumentTypeVenteCommande:
                                            break;
                                        case DocumentType.DocumentTypeVentePrepaLivraison:
                                            UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + item.OrderID + "&orderType=3");
                                            item.OrderType = DocumentType.DocumentTypeVentePrepaLivraison;
                                            col.Update(item);
                                            break;
                                        case DocumentType.DocumentTypeVenteLivraison:
                                            UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + item.OrderID + "&orderType=4");
                                            item.OrderType = DocumentType.DocumentTypeVenteLivraison;
                                            col.Update(item);
                                            break;
                                        case DocumentType.DocumentTypeVenteFacture:
                                            UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + item.OrderID + "&orderType=5");
                                            col.Delete(item.Id);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                            }
                        }
                    }
                }
            }*/
            try
            {
                File.AppendAllText("Log\\time.txt", "start :" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + Environment.NewLine);
                var gescom = SingletonConnection.Instance.Gescom;
                var compta = SingletonConnection.Instance.Compta;

                /***********/
                //test
                //IBICollection AllOrderstest = gescom.FactoryDocumentVente.;
                DB.Connect();
                /***********/
                //IBICollection AllOrders = gescom.FactoryDocumentVente.List;
                using (var db = new LiteDatabase(@"MyData.db"))
                {
                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<LinkedCommandeDB>("Commande");
                    foreach (LinkedCommandeDB item in col.FindAll())
                    {
                        string SQL = null;
                        SQL = "SELECT  DO_Type FROM [" + ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_DOCENTETE] WHERE DO_Ref = '" + item.DO_Ref + "' And DO_Domaine = 0";
                        SqlDataReader dataReader = DB.Select(SQL);
                        while (dataReader.Read())
                        {
                            string data = dataReader.GetValue(0).ToString();
                            /*if ((data.ToString()).Equals("PL"))
                            {
                                //IBODocumentVente3 order = gescom.FactoryDocumentVente.ReadPiece(DocumentType.DocumentTypeVentePrepaLivraison, data);
                                UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + item.OrderID + "&orderType=3");
                                item.OrderType = DocumentType.DocumentTypeVentePrepaLivraison;
                                col.Update(item);
                            }
                            else*/ if (data.Equals("3"))
                            {
                                //IBODocumentVente3 order = gescom.FactoryDocumentVente.ReadPiece(DocumentType.DocumentTypeVenteLivraison, data);
                                UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + item.OrderID + "&orderType=4");
                                item.OrderType = DocumentType.DocumentTypeVenteLivraison;
                                col.Update(item);
                            }
                            else if (data.Equals("7")|| data.Equals("6"))
                            {
                                //IBODocumentVente3 order = gescom.FactoryDocumentVente.ReadPiece(DocumentType.DocumentTypeVenteFacture, data);
                                UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + item.OrderID + "&orderType=5");
                                col.Delete(item.Id);
                            }


                        }
                        dataReader.Close();
                    }

                }
                DB.Disconnect();
                File.AppendAllText("Log\\time.txt", "end :" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + Environment.NewLine);
            }
            catch (Exception s)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                File.AppendAllText("Log\\statut.txt", sb.ToString());
                sb.Clear();
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "UPDATE STATUT ORDER");
            }
        }
        
        /// <summary>
        /// Crée une nouvelle commande pour un utilisateur
        /// </summary>
        /// <param name="jsonOrder">Order à crée</param>
        /// <param name="CT_Num">Client</param>
        public static void AddNewOrderForCustomer(JToken jsonOrder, string CT_Num)
        {
            var gescom = SingletonConnection.Instance.Gescom;

            // création de l'entête de la commande 

            IBOClient3 customer = gescom.CptaApplication.FactoryClient.ReadNumero(CT_Num);
            IBODocumentVente3 order = gescom.FactoryDocumentVente.CreateType(DocumentType.DocumentTypeVenteCommande);
            order.SetDefault();
            order.SetDefaultClient(customer);
            order.DO_Date = DateTime.Now;
            try
            {
                string carrier_id = jsonOrder["order_carriere"].ToString();
                order.Expedition = gescom.FactoryExpedition.ReadIntitule(UtilsConfig.OrderCarrierMapping[carrier_id]);
            }
            catch (Exception s)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "TRANSPORTEUR");
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
            }
            order.Souche = gescom.FactorySoucheVente.ReadIntitule(UtilsConfig.Souche);
            order.DO_Ref = "WEB " + jsonOrder["id_order"].ToString();
            order.SetDefaultDO_Piece();

            // on définis l'adresse de livraison de la commande

            bool asAdressMatch = false;
            IBOClientLivraison3 currentAdress = null;
            foreach (IBOClientLivraison3 tmpAdress in customer.FactoryClientLivraison.List)
            {
                if (tmpAdress.LI_Intitule.ToUpper().Equals(jsonOrder["shipping_name"].ToString().ToUpper()))
                {
                    /*currentAdress = tmpAdress;
                    tmpAdress.Telecom.EMail = customer.Telecom.EMail;
                    if ((jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString()).Length > 35)
                    {
                        tmpAdress.LI_Contact = (jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString()).Substring(0, 35);
                    }
                    else
                    {
                        tmpAdress.LI_Contact = jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString();
                    }
                    tmpAdress.Write();*/
                    asAdressMatch = true;
                    order.LieuLivraison = tmpAdress;
                    break;
                }
                if (tmpAdress.LI_Intitule.Equals(jsonOrder["shipping_company"].ToString().ToUpper()))
                {
                    /*currentAdress = tmpAdress;
                    tmpAdress.Telecom.EMail = customer.Telecom.EMail;
                    if ((jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString()).Length > 35)
                    {
                        tmpAdress.LI_Contact = (jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString()).Substring(0, 35);
                    }
                    else
                    {
                        tmpAdress.LI_Contact = jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString();
                    }
                    tmpAdress.Write();*/
                    asAdressMatch = true;
                    order.LieuLivraison = tmpAdress;
                    break;
                }
            }


            // si on a trouver aucune adresse coresspondant e sur le client alors on la crée
            IBOClientLivraison3 adress;
            if (!asAdressMatch)
            {
                adress = (IBOClientLivraison3)customer.FactoryClientLivraison.Create();
                adress.SetDefault();

                adress.Telecom.EMail = customer.Telecom.EMail;

                try
                {
                    string carrier_id = jsonOrder["order_carriere"].ToString();
                    adress.Expedition = gescom.FactoryExpedition.ReadIntitule(UtilsConfig.OrderCarrierMapping[carrier_id]);
                }
                catch (Exception s)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                    sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                    UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "TRANSPORTEUR");
                    File.AppendAllText("Log\\order.txt", sb.ToString());
                    sb.Clear();
                }

                if (jsonOrder["shipping_company"].ToString() != "")
                {
                    if (jsonOrder["shipping_company"].ToString().Length > 35)
                    {
                        adress.LI_Intitule = jsonOrder["shipping_company"].ToString().ToUpper().Substring(0, 35);
                    }
                    else
                    {
                        adress.LI_Intitule = jsonOrder["shipping_company"].ToString().ToUpper();
                    }

                }
                else
                {
                    if (jsonOrder["shipping_name"].ToString().Length > 35)
                    {
                        adress.LI_Intitule = jsonOrder["shipping_name"].ToString().ToUpper().Substring(0, 35);
                    }
                    else
                    {
                        adress.LI_Intitule = jsonOrder["shipping_name"].ToString().ToUpper();
                    }

                }

                // Setup champ contact dans adress
                if ((jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString()).Length > 35)
                {
                    adress.LI_Contact = (jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString()).Substring(0, 35);
                }
                else
                {
                    adress.LI_Contact = jsonOrder["shipping_firstname"].ToString() + " " + jsonOrder["shipping_lastname"].ToString();
                }

                if (jsonOrder["shipping_adresse1"].ToString().Length > 35)
                {
                    adress.Adresse.Adresse = jsonOrder["shipping_adresse1"].ToString().Substring(0, 35);
                }
                else
                {
                    adress.Adresse.Adresse = jsonOrder["shipping_adresse1"].ToString();
                }
                adress.Adresse.Complement = jsonOrder["shipping_adresse2"].ToString();
                adress.Adresse.CodePostal = jsonOrder["shipping_postcode"].ToString();
                adress.Adresse.Ville = jsonOrder["shipping_city"].ToString();
                adress.Adresse.Pays = jsonOrder["shipping_country"].ToString();
                adress.Telecom.Telephone = jsonOrder["shipping_phone"].ToString();
                adress.Telecom.Telecopie = jsonOrder["shipping_phone_mobile"].ToString();
                adress.Telecom.EMail = customer.Telecom.EMail.ToString();
                if (String.IsNullOrEmpty(UtilsConfig.CondLivraison))
                {
                    // pas de configuration renseigner pour CondLivraison par defaut
                    // todo log
                }
                else
                {
                    adress.ConditionLivraison = gescom.FactoryConditionLivraison.ReadIntitule(UtilsConfig.CondLivraison);
                }
                if (String.IsNullOrEmpty(UtilsConfig.Expedition))
                {
                    // pas de configuration renseigner pour Expedition par defaut
                    // todo log
                }
                else
                {
                    adress.Expedition = gescom.FactoryExpedition.ReadIntitule(UtilsConfig.Expedition);
                }
                
                adress.Write();

                // on ajoute une adresse par defaut sur la fiche client si il y en a pas

                // On met à jour l'adresse de facturation du client
                #region Update invoice Adress
                if (jsonOrder["invoice_adresse1"].ToString().Length > 35)
                {
                    customer.Adresse.Adresse = jsonOrder["invoice_adresse1"].ToString().Substring(0, 35);
                }
                else
                {
                    customer.Adresse.Adresse = jsonOrder["invoice_adresse1"].ToString();
                }
                customer.Adresse.Complement = jsonOrder["invoice_adresse2"].ToString();
                customer.Adresse.CodePostal = jsonOrder["invoice_postcode"].ToString();
                customer.Adresse.Ville = jsonOrder["invoice_city"].ToString();
                customer.Adresse.Pays = jsonOrder["invoice_country"].ToString();
                customer.Telecom.Telephone = jsonOrder["invoice_phone"].ToString();
                customer.Telecom.Telecopie = jsonOrder["invoice_phone_mobile"].ToString();
                customer.CT_Identifiant = jsonOrder["shipping_vat"].ToString();
                customer.LivraisonPrincipal = adress;
                customer.Write();
                #endregion
                
                order.LieuLivraison = adress;

            }
            order.Write();
           
            // création des lignes de la commandes
            try
            {
                foreach (JToken product in jsonOrder["products"].Children())
                {
                    // on récupère la chaine de gammages d'un produit
                    string product_attribut_string = product["product_attribute_id_string"].ToString();
                    String[] subgamme = product_attribut_string.Split('|');


                    IBODocumentLigne3 docLigne = (IBODocumentLigne3)order.FactoryDocumentLigne.Create();

                    IBOArticle3 article = gescom.FactoryArticle.ReadReference(product["product_ref"].ToString());
                    docLigne.DL_PrixUnitaire = double.Parse(product["price"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    
                    if (subgamme.Length == 2)
                    {
                        // produit à simple gamme
                        IBOArticleGammeEnum3 articleEnum = ControllerArticle.GetArticleGammeEnum1(article, new Gamme(subgamme[0], subgamme[1]));
                        docLigne.SetDefaultArticleMonoGamme(articleEnum, Int32.Parse(product["product_quantity"].ToString()));
                    }
                    else if (subgamme.Length == 4)
                    {
                        // produit à double gamme
                        IBOArticleGammeEnum3 articleEnum = ControllerArticle.GetArticleGammeEnum1(article, new Gamme(subgamme[0], subgamme[1], subgamme[2], subgamme[3]));
                        IBOArticleGammeEnum3 articleEnum2 = ControllerArticle.GetArticleGammeEnum2(article, new Gamme(subgamme[0], subgamme[1], subgamme[2], subgamme[3]));
                        docLigne.SetDefaultArticleDoubleGamme(articleEnum, articleEnum2, Int32.Parse(product["product_quantity"].ToString()));
                    }
                    else
                    {
                        // produit simple
                        docLigne.SetDefaultArticle(gescom.FactoryArticle.ReadReference(product["product_ref"].ToString()), Int32.Parse(product["product_quantity"].ToString()));
                        if (product["product_ref"].ToString().Equals("TRANSPORT"))
                        {
                            docLigne.DL_PrixUnitaire = Convert.ToDouble(product["price"].ToString().Replace('.', ','));
                            //ajout message transporteur
                            if (!String.IsNullOrEmpty(jsonOrder["message"].ToString()))
                            {
                                docLigne.TxtComplementaire = jsonOrder["message"].ToString();
                            }
                        }
                        else if (product["product_ref"].ToString().Equals("REMISE"))
                        {
                            docLigne.DL_PrixUnitaire = Convert.ToDouble(product["price"].ToString().Replace('.', ','));
                        }
                    }
                    
                    docLigne.Write();
                }

            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + e.Message + Environment.NewLine);
                sb.Append(DateTime.Now + e.StackTrace + Environment.NewLine);
                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "COMMANDE LIGNE");
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
                UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + jsonOrder["id_order"]);
                order.Remove();
                return;
            }
            addOrderToLocalDB(jsonOrder["id_order"].ToString(), order.Client.CT_Num, order.DO_Piece, order.DO_Ref);
            // on notfie prestashop que la commande à bien été crée dans SAGE
            UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "validateOrder&orderID=" + jsonOrder["id_order"]);
        }

        private static void addOrderToLocalDB(string orderID, string CT_Num, string DO_piece, string DO_Ref)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<LinkedCommandeDB>("Commande");

                // Create your new customer instance
                var commande = new LinkedCommandeDB
                {
                    OrderID = orderID,
                    OrderType = DocumentType.DocumentTypeVenteCommande,
                    CT_Num = CT_Num,
                    DO_piece = DO_piece,
                    DO_Ref = DO_Ref

                };
                col.Insert(commande);
            }
        }

        public static string GetPrestaOrderStatutFromMapping(DocumentType orderSageType)
        {
            string prestaType;
            if(UtilsConfig.OrderMapping.TryGetValue(orderSageType.ToString(), out prestaType))
            {
                return prestaType;
            }
            else
            {
                return null;
            }
        }

        public static void UpdateStatutOnPresta(string orderID, string newType)
        {
            UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "updateOrder&orderID=" + orderID + "&orderType=" + newType);
        }
    }
}
