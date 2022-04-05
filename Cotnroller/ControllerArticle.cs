using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebservicesSage.Singleton;
using WebservicesSage.Object;
using Objets100cLib;
using System.Windows.Forms;
using WebservicesSage.Utils;
using WebservicesSage.Utils.Enums;
using System.Timers;

namespace WebservicesSage.Cotnroller
{
    class ControllerArticle
    {

        public static void LaunchService()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(SendStockCrone);
            timer.Interval = UtilsConfig.CronTaskStock;
            timer.Enabled = true;

            //timer for the restart
            System.Timers.Timer timerRestartApplication = new System.Timers.Timer();
            timerRestartApplication.Elapsed += new ElapsedEventHandler(RestartAPP);
            timerRestartApplication.Interval = UtilsConfig.CronTaskRestart;
            timerRestartApplication.Enabled = true;
        }
        public static void RestartAPP(object source, ElapsedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("date de redémarrage : " + DateTime.Now + Environment.NewLine);
            System.IO.File.AppendAllText("Log\\restart.txt", sb.ToString());
            sb.Clear();
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Application.ExitThread();
            Application.Exit();
            Environment.Exit(200);
        }
        /// <summary>
        /// Permets de remonter toute la base articles de SAGE vers Prestashop
        /// Ne remonte que les articles coché en publier sur le site marchand !
        /// </summary>
        public static void SendAllArticles()
        {
            try
            {
                List<ArticleNomenclature> ArticleNomenclature = new List<ArticleNomenclature>();
                var gescom = SingletonConnection.Instance.Gescom;

                var articleSageObj = gescom.FactoryArticle.List;
                
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;


                foreach (Article article in articles)
                {

                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : "+increm));

                   /* if (increm == iter)
                    {
                        if (SingletonUI.Instance.ProgressBar.Value != 100 && SingletonUI.Instance.ProgressBar.Value + 10 < 100)
                            SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value += 10));
                    }*/
                    // on ajoute les nomenclature à une liste.
                    if (article.HaveNomenclature)
                    {
                        ArticleNomenclature.Add(article.ArticleNomenclature);
                    }


                    string articleXML = UtilsSerialize.SerializeObject<Article>(article);

                    UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Article.Value, articleXML);

                    increm++;
                }
                // une fois que tout les produit sont remonter on remonte les nomenclature
                foreach (ArticleNomenclature articleNomenclature in ArticleNomenclature)
                {
                    string articleNomenclatureXML = UtilsSerialize.SerializeObject<ArticleNomenclature>(articleNomenclature);

                }

               // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendCustomArticles(string reference)
        {
            try
            {
                List<ArticleNomenclature> ArticleNomenclature = new List<ArticleNomenclature>();
                var gescom = SingletonConnection.Instance.Gescom;

                var articleSageObj = gescom.FactoryArticle.ReadReference(reference);

                var article = new Article(articleSageObj);

                if (article.HaveNomenclature)
                {
                    ArticleNomenclature.Add(article.ArticleNomenclature);
                }

                //SingletonUI.Instance.LogBox.Invoke((MethodInvoker)(() => SingletonUI.Instance.LogBox.AppendText("-- Processing Article -- " + article.Reference + " START -- " + Environment.NewLine)));

                string articleXML = UtilsSerialize.SerializeObject<Article>(article);

                UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Article.Value, articleXML);


                //SingletonUI.Instance.LogBox.Invoke((MethodInvoker)(() => SingletonUI.Instance.LogBox.AppendText("Processing " + articles.Count + " Articles" + Environment.NewLine)));

                // une fois que tout les produit sont remonter on remonte les nomenclature
                foreach (ArticleNomenclature articleNomenclature in ArticleNomenclature)
                {
                    string articleNomenclatureXML = UtilsSerialize.SerializeObject<ArticleNomenclature>(articleNomenclature);

                    //Console.WriteLine(UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.ArticleNomenclature.Value, articleNomenclatureXML));
                }
                MessageBox.Show("end sync", "end",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
           
        }

        /// <summary>
        /// Permets de récupérer une liste d'articles propre depuis une liste d'artivcle SAGE
        /// Permets de gérer la configuration des produits
        /// </summary>
        /// <param name="articleSageObj">Liste d'article SAGE</param>
        /// <returns></returns>
        private static List<Article> GetListOfClientToProcess(IBICollection articleSageObj)
        {
            List<Article> articleToProcess = new List<Article>();
            string CurrentRefArticle = "";

                int incre = 0;
                foreach (IBOArticle3 articleSage in articleSageObj)
                {
                    CurrentRefArticle = articleSage.AR_Ref;
                    try
                    {
                        SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Fetching Data : " + incre));

                        // on check si l'article est cocher en publier sur le site marchand
                        if (!articleSage.AR_Publie)
                            continue;

                        Article article = new Article(articleSage);

                        if (!HandleArticleError(article))
                        {
                            articleToProcess.Add(article);
                        }
                    }
                    catch (Exception e)
                    {
                    UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "ARTICLE "+ CurrentRefArticle);
                    }
                incre++;
                }
            return articleToProcess;
        }

        /// <summary>
        /// Permet de vérifier si un article comporte des erreur ou non
        /// </summary>
        /// <param name="article">Article à tester</param>
        /// <returns></returns>
        private static bool HandleArticleError(Article article)
        {

            return false;
        }

        /// <summary>
        /// Permet de récupérer l'énuméré SAGE 1 d'un article 
        /// </summary>
        /// <param name="article"></param>
        /// <param name="gamme">Gamme sur laquelle nous devont chercher l'énuméré</param>
        /// <returns></returns>
        public static IBOArticleGammeEnum3 GetArticleGammeEnum1(IBOArticle3 article, Gamme gamme)
        {
            foreach(IBOArticleGammeEnum3 articleEnum in article.FactoryArticleGammeEnum1.List)
            {
                if (articleEnum.EG_Enumere.Equals(gamme.Value_Intitule))
                {
                    return articleEnum;
                }
            }

            return null;
        }

        /// <summary>
        /// Permet de récupérer l'énuméré SAGE 2 d'un article 
        /// </summary>
        /// <param name="article"></param>
        /// <param name="gamme">Gamme sur laquelle nous devont chercher l'énuméré</param>
        /// <returns></returns>
        public static IBOArticleGammeEnum3 GetArticleGammeEnum2(IBOArticle3 article, Gamme gamme)
        {
            foreach (IBOArticleGammeEnum3 articleEnum in article.FactoryArticleGammeEnum1.List)
            {
                foreach (IBOArticleGammeEnumRef3 articleEnum2 in articleEnum.FactoryArticleGammeEnumRef.List)
                {
                    if (articleEnum.EG_Enumere.Equals(gamme.Value_Intitule) && articleEnum2.ArticleGammeEnum2.EG_Enumere.Equals(gamme.Value_Intitule2))
                    {
                        return articleEnum2.ArticleGammeEnum2;
                    }
                }
               
            }

            return null;
        }

        public static void SendStockCrone(object source, ElapsedEventArgs e)
        {
            string currentArticleRef = "";
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.List;
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;

                foreach (Article article in articles)
                {
                    currentArticleRef = article.Reference;
                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));

                    
                    string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                    UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Stock.Value, articleXML);
                    increm++;
                }

               // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));

            }
            catch (Exception t)
            {
                UtilsMail.SendErrorMail(DateTime.Now + t.Message + Environment.NewLine + t.StackTrace + Environment.NewLine, "CRON STOCK");
            }
        }

        public static void SendStock()  
        {
            string currentArticleRef = "";
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.List;
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;

                foreach (Article article in articles)
                {
                    currentArticleRef = article.Reference;
                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));

                   /* if (increm == iter)
                    {
                        if (SingletonUI.Instance.ProgressBar.Value != 100 && SingletonUI.Instance.ProgressBar.Value + 10 < 100)
                            SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value += 10));
                    }*/
                    string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                    UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Stock.Value, articleXML);
                    increm++;
                }

                //SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendCustomStock(string reference)
        {
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.ReadReference(reference);
                Article article = new Article(articleSageObj);

                int increm = 1;

                SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));
                string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Stock.Value, articleXML);                

              //  SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendPrice()
        {
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.List;
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;

                foreach (Article article in articles)
                {

                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));

                    /*if (increm == iter)
                    {
                        if (SingletonUI.Instance.ProgressBar.Value != 100 && SingletonUI.Instance.ProgressBar.Value + 10 < 100)
                            SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value += 10));
                    }*/
                    string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                    UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Price.Value, articleXML);
                    increm++;
                }

               // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendCustomPrice(string reference)
        {
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.ReadReference(reference);
                Article article = new Article(articleSageObj);

                int increm = 1;

                SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));
                string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Price.Value, articleXML);

               // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}







