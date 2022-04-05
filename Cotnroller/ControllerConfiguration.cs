using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebservicesSage.Singleton;
using Objets100cLib;
using WebservicesSage.Utils;


namespace WebservicesSage.Cotnroller
{
    public static class ControllerConfiguration
    {

        public static void SaveConfiguration()
        {
           
            UtilsConfig.CompteG = SingletonUI.Instance.ComptGConf.selectedValue;
            UtilsConfig.CatTarif = SingletonUI.Instance.CatTarifConf.selectedValue;
            UtilsConfig.CondLivraison = SingletonUI.Instance.CondLivraisonConf.selectedValue;
            UtilsConfig.Expedition = SingletonUI.Instance.ExpeditionConf.selectedValue;
            UtilsConfig.PrefixClient = SingletonUI.Instance.PrefixClientConf.Text;

            //UtilsConfig.ArrondiDigits = Convert.ToInt32(SingletonUI.Instance.ArticleConfigurationArrondiInput.Text);
        }


        public static void LoadConfiguration()
        {
            // compteG
            var compta = SingletonConnection.Instance.Compta;
            var gescom = SingletonConnection.Instance.Gescom;

            LoadArticleConfiguration();
            LoadOrderConfiguration();
            LoadClientConfiguration();
            LoadGeneralConfiguration();
            #region CONFIGURATION CLIENT


            /* #region CONFIGURATION COMPTE GENERAUX
             string[] items = new string[compta.FactoryCompteG.List.Count];
             int count = 0;
             foreach (IBOCompteG3 t in compta.FactoryCompteG.List)
             {
                 items[count] = t.CG_Num + " || " + t.CG_Intitule;
                 count++;
             }
             SingletonUI.Instance.ComptGConf.Items = items;
             SingletonUI.Instance.ComptGConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.CompteG))
             {
                 Console.WriteLine(UtilsConfig.CompteGnum);
                 int index = Array.IndexOf(SingletonUI.Instance.ComptGConf.Items, UtilsConfig.CompteG);
                 SingletonUI.Instance.ComptGConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION CAT TARRIFAIRE
             items = new string[gescom.FactoryCategorieTarif.List.Count];
             count = 0;
             foreach (IBPCategorieTarif t in gescom.FactoryCategorieTarif.List)
             {
                 items[count] = t.CT_Intitule;
                 count++;
             }

             SingletonUI.Instance.CatTarifConf.Items = items;
             SingletonUI.Instance.CatTarifConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.CatTarif))
             {
                 int index = Array.IndexOf(SingletonUI.Instance.CatTarifConf.Items, UtilsConfig.CatTarif);
                 SingletonUI.Instance.CatTarifConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION CONDITION DE LIVRAISON

             items = new string[gescom.FactoryConditionLivraison.List.Count];
             count = 0;
             foreach (IBPConditionLivraison t in gescom.FactoryConditionLivraison.List)
             {
                 items[count] = t.C_Intitule;
                 count++;
             }

             SingletonUI.Instance.CondLivraisonConf.Items = items;
             SingletonUI.Instance.CondLivraisonConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.CondLivraison))
             {
                 int index = Array.IndexOf(SingletonUI.Instance.CondLivraisonConf.Items, UtilsConfig.CondLivraison);
                 SingletonUI.Instance.CondLivraisonConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION MODE EXPEDITION
             items = new string[gescom.FactoryExpedition.List.Count];
             count = 0;
             foreach (IBPExpedition3 t in gescom.FactoryExpedition.List)
             {
                 items[count] = t.E_Intitule;
                 count++;
             }

             SingletonUI.Instance.ExpeditionConf.Items = items;
             SingletonUI.Instance.ExpeditionConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.Expedition))
             {
                 int index = Array.IndexOf(SingletonUI.Instance.ExpeditionConf.Items, UtilsConfig.Expedition);
                 SingletonUI.Instance.ExpeditionConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION PREFIX CLIENT
             if (!String.IsNullOrEmpty(UtilsConfig.PrefixClient))
             {
                 SingletonUI.Instance.PrefixClientConf.Text = UtilsConfig.PrefixClient;
             }
             #endregion*/



            #endregion



        }

        public static void LoadOrderConfiguration()
        {
            var compta = SingletonConnection.Instance.Compta;
            var gescom = SingletonConnection.Instance.Gescom;

            List<string> items = new List<string>();
            int count, selectedindex;

            //items = new string[];
            count = 0;
            selectedindex = 0;
            foreach (IBISouche souche in gescom.FactorySoucheVente.List)
            {
                string test = souche.Intitule;
                if (!String.IsNullOrEmpty(souche.Intitule))
                {
                    items.Add(souche.Intitule);
                    if (souche.Intitule.Equals(UtilsConfig.Souche))
                    {
                        selectedindex = count;
                    }
                    count++;
                }
            }
            SingletonUI.Instance.SoucheDropdown.Items = items.ToArray();
            SingletonUI.Instance.SoucheDropdown.selectedIndex = selectedindex;
           // SingletonUI.Instance.SoucheDropdown.Refresh();
        }

        public static void LoadArticleConfiguration()
        {/* OLD VERSION
            var compta = SingletonConnection.Instance.Compta;
            var gescom = SingletonConnection.Instance.Gescom;
            string[] items;
            int count;

            #region CONFIGURATION ARTICLE

            #region CONIGURATION DES DEPOTS

            items = new string[gescom.FactoryDepot.List.Count];
            count = 0;
            foreach (IBODepot3 depot in gescom.FactoryDepot.List)
            {
                items[count] = depot.DE_Intitule;
                count++;
            }
            SingletonUI.Instance.CatTarifConf.Items = items;
            SingletonUI.Instance.CatTarifConf.Refresh();

            #endregion


            #endregion*/


            SingletonUI.Instance.ArticleConfigurationArrondiInput.Text = UtilsConfig.ArrondiDigits.ToString();
            SingletonUI.Instance.ArticleConfigurationTVAInput.Text = UtilsConfig.TVA.ToString();
            if (UtilsConfig.DefaultStock.Equals("TRUE"))
            {
                SingletonUI.Instance.DefaultStock.Value = true;
            }
            else
            {
                SingletonUI.Instance.DefaultStock.Value = false;
            }
             
        }
        public static void  UpdateArticleConfiguration()
        {
            int ArrondiDigits, TVA;
            if (int.TryParse(SingletonUI.Instance.ArticleConfigurationArrondiInput.Text, out ArrondiDigits))
            {
                UtilsConfig.ArrondiDigits = ArrondiDigits;
            }
            if (int.TryParse(SingletonUI.Instance.ArticleConfigurationTVAInput.Text, out TVA))
            {
                UtilsConfig.TVA = TVA;
            }
            if (SingletonUI.Instance.DefaultStock.Value)
            {
                 UtilsConfig.DefaultStock = "TRUE";
            }
            else
            {
                UtilsConfig.DefaultStock = "FALSE";
            }
            SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
        }
        public static void UpdateOrderConfiguration()
        {
            UtilsConfig.Souche = SingletonUI.Instance.SoucheDropdown.selectedValue.ToString();
            SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
        }
        public static void UpdateClientConfiguration()
        {
            UtilsConfig.PrefixClient = SingletonUI.Instance.PrefixClient.Text;
            SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
        }
        public static void LoadClientConfiguration()
        {
            SingletonUI.Instance.PrefixClient.Text = UtilsConfig.PrefixClient;
        }
        public static void LoadGeneralConfiguration()
        {
            SingletonUI.Instance.BaseURLConfiguration.Text = UtilsConfig.BaseUrl.ToString();
            SingletonUI.Instance.UserConfiguration.Text = UtilsConfig.User.ToString();
            SingletonUI.Instance.Gcm_User.Text = UtilsConfig.Gcm_User.ToString();
            SingletonUI.Instance.Mae_User.Text = UtilsConfig.Mae_User.ToString();
            SingletonUI.Instance.Gcm_Pass.Text = UtilsConfig.Gcm_Pass.ToString();
            SingletonUI.Instance.Gcm_Path.Text = UtilsConfig.Gcm_Path.ToString();
            SingletonUI.Instance.Mae_Pass.Text = UtilsConfig.Mae_Pass.ToString();
            SingletonUI.Instance.Mae_Path.Text = UtilsConfig.Mae_Path.ToString();
            SingletonUI.Instance.SynchroClientCron.Text = UtilsConfig.CRONSYNCHROCLIENT.ToString();
            SingletonUI.Instance.CronTaskCheckNewOrder.Text = TimeSpan.FromMilliseconds(UtilsConfig.CronTaskCheckForNewOrder).TotalMinutes.ToString("0.00");
            SingletonUI.Instance.CronTaskUpdateStatut.Text = TimeSpan.FromMilliseconds(UtilsConfig.CronTaskUpdateStatut).TotalMinutes.ToString("0.00");
            if (UtilsConfig.Mae_Set.ToString().Equals("TRUE"))
            {
                SingletonUI.Instance.MAE_set.Value = true;
            }
            else
            {
                SingletonUI.Instance.MAE_set.Value = false;
            }

            if (UtilsConfig.Gcm_Set.ToString().Equals("TRUE"))
            {
                SingletonUI.Instance.GCM_set.Value = true;
            }
            else
            {
                SingletonUI.Instance.GCM_set.Value = false;
            }


        }
        public static void UpdateGeneralConfiguration()
        {
            int result;
            Boolean error = false;
            String CronTaskCheckNewOrder = SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString(); ;
            String CronTaskUpdateStatut = SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString(); ;
            UtilsConfig.BaseUrl = SingletonUI.Instance.BaseURLConfiguration.Text;
            UtilsConfig.User = SingletonUI.Instance.UserConfiguration.Text;
            UtilsConfig.Mae_Path = SingletonUI.Instance.Mae_Path.Text;
            UtilsConfig.Mae_User = SingletonUI.Instance.Mae_User.Text;
            UtilsConfig.Mae_Pass = SingletonUI.Instance.Mae_Pass.Text;
            UtilsConfig.Mae_Set = SingletonUI.Instance.MAE_set.Value.ToString().ToUpper();
            UtilsConfig.Gcm_Path = SingletonUI.Instance.Gcm_Path.Text;
            UtilsConfig.Gcm_User = SingletonUI.Instance.Gcm_User.Text;
            UtilsConfig.Gcm_Pass = SingletonUI.Instance.Gcm_Pass.Text;
            UtilsConfig.Gcm_Set = SingletonUI.Instance.GCM_set.Value.ToString().ToUpper();
            UtilsConfig.CRONSYNCHROCLIENT = SingletonUI.Instance.SynchroClientCron.Text.ToString();
            if (SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf(",")>0)
            {
                CronTaskCheckNewOrder = SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf(","));
            }else
            if (SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf(".") > 0)
            {
                CronTaskCheckNewOrder = SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf("."));
            }

            if (Int32.TryParse(CronTaskCheckNewOrder.ToString(), out result)&& result>0)
            {
                UtilsConfig.CronTaskCheckForNewOrder = Convert.ToInt32(TimeSpan.FromMinutes(result).TotalMilliseconds);
                SingletonUI.Instance.CronTaskCheckNewOrder.BackColor = System.Drawing.Color.White;
            }
            else
            {
                SingletonUI.Instance.ShowErrorNotification("Merci de saisie une valeur supérieur à 1");
                SingletonUI.Instance.CronTaskCheckNewOrder.BackColor = System.Drawing.Color.Red;
                error = true;
            }

            if (SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf(",") > 0)
            {
                CronTaskUpdateStatut = SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf(","));
            }
            else
            if (SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf(".") > 0)
            {
                CronTaskUpdateStatut = SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf("."));
            }

            if (Int32.TryParse(CronTaskUpdateStatut.ToString(), out result) && result >0)
            {
                UtilsConfig.CronTaskUpdateStatut = Convert.ToInt32(TimeSpan.FromMinutes(result).TotalMilliseconds);
                SingletonUI.Instance.CronTaskUpdateStatut.BackColor = System.Drawing.Color.White;
            }
            else
            {
                SingletonUI.Instance.ShowErrorNotification("Merci de saisie une valeur supérieur à 1");
                SingletonUI.Instance.CronTaskUpdateStatut.BackColor = System.Drawing.Color.Red;
                error = true;
            }
            if (!error)
            {
                SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
            }
        }
    }

}
