using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objets100cLib;
using WebservicesSage.Object;
using WebservicesSage.Utils;

namespace WebservicesSage.Object
{
    [Serializable()]
    public class Article
    {
        public string Designation { get; set; }
        public string Reference { get; set; }
        public double PrixAchat { get; set; }
        public double PrixVente { get; set; }
        public string Langue1 { get; set; }
        public string Langue2 { get; set; }
        public string CodeBarres { get; set; }
        public double Poid { get; set; }
        public bool Sommeil { get; set; }
        public bool IsPriceTTC { get; set; }
        public bool isGamme { get; set; }
        public string Unite { get; set; }
        public List<Gamme> Gammes { get; set; }
        public List<PrixGamme> prixGammes { get; set; }
        public List<PrixRemise> prixRemises { get; set; }
        public List<InfoLibre> infoLibre { get; set; }
        public bool IsDoubleGamme { get; set; }
        public double Stock { get; set; }
        public bool HaveNomenclature { get; set; }
        public ArticleNomenclature ArticleNomenclature { get; set; }

        public Article(IBOArticle3 articleFC)
        {

            prixRemises = new List<PrixRemise>();
            infoLibre = new List<InfoLibre>();

            var infolibreField = Singleton.SingletonConnection.Instance.Gescom.FactoryArticle.InfoLibreFields;

            int compteur = 1;
            foreach (var infoLibreValue in articleFC.InfoLibre)
            {
                infoLibre.Add(new InfoLibre(infolibreField[compteur].Name, infoLibreValue.ToString()));
                compteur++;
            }

            Designation = articleFC.AR_Design;
            Reference = articleFC.AR_Ref;
            PrixAchat = articleFC.AR_PrixAchat;
            PrixVente = articleFC.AR_PrixVen;
            Langue1 = articleFC.AR_Langue1;
            Langue2 = articleFC.AR_Langue2;
            CodeBarres = articleFC.AR_CodeBarre;
            if (UtilsConfig.DefaultStock.Equals("TRUE"))
            {
                Stock = articleFC.StockReel();
            }
            else
            {
                Stock = articleFC.StockATerme();
            }
            
            Unite = articleFC.Unite.Intitule;

            // gestion de la conversion du poids en KG pour prestashop

            if (articleFC.AR_UnitePoids == UnitePoidsType.UnitePoidsTypeGramme)
            {
                Poid = articleFC.AR_PoidsBrut / 1000;
            }

            Sommeil = articleFC.AR_Sommeil;
            IsPriceTTC = articleFC.AR_PrixTTC;



            // gestion des prix par remise
            foreach (IBOArticleTarifCategorie3 articleTarif in articleFC.FactoryArticleTarifCategorie.List)
            {
                foreach (IBOArticleTarifQteCategorie3 tarif in articleTarif.FactoryArticleTarifQte.List)
                {
                    PrixRemise prix = new PrixRemise();
                    prix.CategorieTarifaire = articleTarif.CategorieTarif.CT_Intitule;
                    prix.Born_Sup = tarif.BorneSup;
                    prix.Price = tarif.PrixNet;
                    prixRemises.Add(prix);
                }
            }

            if (IsPriceTTC)
            {
                //   PrixVente = articleFC.AR_PrixVen * ;
            }

            // gestion des produits à gammes
            if (articleFC.AR_Type == ArticleType.ArticleTypeGamme)
            {
                isGamme = true;
                Gammes = new List<Gamme>();
                prixGammes = new List<PrixGamme>();
                IsDoubleGamme = false;

                if (articleFC.FactoryArticleGammeEnum2.List.Count > 0)
                    IsDoubleGamme = true;

                if (IsDoubleGamme)
                {
                    foreach (IBOArticleGammeEnum3 gamme in articleFC.FactoryArticleGammeEnum1.List)
                    {
                        foreach (IBOArticleGammeEnumRef3 li in gamme.FactoryArticleGammeEnumRef.List)
                        {
                            foreach (IBPCategorieTarif catTarifaire in Singleton.SingletonConnection.Instance.Gescom.FactoryCategorieTarif.List)
                            {

                                if (!String.IsNullOrEmpty(catTarifaire.CT_Intitule))
                                {
                                    ITarifVente2 test = articleFC.TarifVenteCategorieDoubleGamme(catTarifaire, li.ArticleGammeEnum1, li.ArticleGammeEnum2, 1);
                                    PrixGamme prix = new PrixGamme();
                                    if (test.PrixTTC)
                                    {
                                        // calcul TVA
                                        prix.Price = test.Prix / UtilsConfig.TVACalculer;
                                    }
                                    else
                                    {
                                        prix.Price = test.Prix;
                                    }
                                    prix.Gamme1_Intitule = articleFC.Gamme1.G_Intitule;
                                    prix.Gamme1_Value = li.ArticleGammeEnum1.EG_Enumere;
                                    prix.Gamme2_Intitule = articleFC.Gamme2.G_Intitule;
                                    prix.Gamme2_Value = li.ArticleGammeEnum2.EG_Enumere;
                                    prix.Categori_Intitule = catTarifaire.CT_Intitule;

                                    // gestion des arrondi 
                                    prix.Price = Math.Round(prix.Price, UtilsConfig.ArrondiDigits);

                                    prixGammes.Add(prix);
                                }
                            }

                            Gamme gammeO = new Gamme(articleFC.Gamme1.G_Intitule, li.ArticleGammeEnum1.EG_Enumere, articleFC.Gamme2.G_Intitule, li.ArticleGammeEnum2.EG_Enumere);
                            ITarif2 tarif = articleFC.TarifAchatDoubleGamme(li.ArticleGammeEnum1, li.ArticleGammeEnum2);
                            gammeO.Price = tarif.Prix;
                            gammeO.Reference = tarif.Reference;
                            gammeO.CodeBarre = tarif.CodeBarre;
                            gammeO.Sommeil = li.AE_Sommeil;
                            if (UtilsConfig.DefaultStock.Equals("TRUE"))
                            {
                                gammeO.Stock = articleFC.StockReelDoubleGamme(li.ArticleGammeEnum1, li.ArticleGammeEnum2);
                            }
                            else
                            {
                                gammeO.Stock = articleFC.StockATermeDoubleGamme(li.ArticleGammeEnum1, li.ArticleGammeEnum2);
                            }
                            
                            Gammes.Add(gammeO);
                        }
                    }
                }
                else
                {
                    foreach (IBOArticleGammeEnum3 gamme in articleFC.FactoryArticleGammeEnum1.List)
                    {
                        ITarif2 tarif = articleFC.TarifAchatMonoGamme(gamme);
                        //IBPCategorieTarif t = Singleton.SingletonConnection.Instance.Gescom.FactoryCategorieTarif.ReadIntitule("Grossistes");

                        foreach (IBPCategorieTarif catTarifaire in Singleton.SingletonConnection.Instance.Gescom.FactoryCategorieTarif.List)
                        {
                            if (!String.IsNullOrEmpty(catTarifaire.CT_Intitule))
                            {
                                ITarifVente2 test = articleFC.TarifVenteCategorieMonoGamme(catTarifaire, gamme, 1);

                                PrixGamme prix = new PrixGamme();
                                if (test.PrixTTC)
                                {
                                    // calcul TVA
                                    prix.Price = test.Prix / UtilsConfig.TVACalculer;
                                }
                                else
                                {
                                    prix.Price = test.Prix;
                                }
                                prix.Gamme1_Intitule = articleFC.Gamme1.G_Intitule;
                                prix.Gamme1_Value = gamme.EG_Enumere;
                                prix.Categori_Intitule = catTarifaire.CT_Intitule;

                                // gestion des arrondi 
                                prix.Price = Math.Round(prix.Price, UtilsConfig.ArrondiDigits);

                                prixGammes.Add(prix);
                            }
                        }

                        Gamme gammeO = new Gamme(articleFC.Gamme1.G_Intitule, gamme.EG_Enumere);
                        gammeO.Price = tarif.Prix;
                        gammeO.Reference = tarif.Reference;
                        gammeO.CodeBarre = tarif.CodeBarre;
                        if (UtilsConfig.DefaultStock.Equals("TRUE"))
                        {
                            gammeO.Stock = articleFC.StockReelMonoGamme(gamme);
                        }
                        else
                        {
                            gammeO.Stock = articleFC.StockATermeMonoGamme(gamme);
                        }
                        

                        try
                        {
                            IBOArticleGammeEnumRef3 reff = (IBOArticleGammeEnumRef3)gamme.FactoryArticleGammeEnumRef.List[1];
                            gammeO.Sommeil = reff.AE_Sommeil;
                        }
                        catch (Exception e)
                        {
                            gammeO.Sommeil = false;
                        }

                        Gammes.Add(gammeO);
                    }
                }
            }

            if (articleFC.AR_Nomencl != NomenclatureType.NomenclatureTypeAucun && articleFC.AR_Nomencl != NomenclatureType.NomenclatureTypeFabrication)
            {
                HaveNomenclature = true;
                ArticleNomenclature = new ArticleNomenclature();
                ArticleNomenclature.ArticleRef = articleFC.AR_Ref;

                foreach (IBOArticleNomenclature3 nomenclature in articleFC.FactoryArticleNomenclature.List)
                {
                    ArticleNomenclature.NomenclatureRefList.Add(nomenclature.ArticleComposant.AR_Ref);
                }
            }


        }

        public Article()
        {

        }
    }
}
