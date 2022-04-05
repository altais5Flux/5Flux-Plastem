using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;

namespace WebservicesSage.Utils
{
    static class UtilsConfig
    {
        public static string User
        {
            get
            {
                return ConfigurationManager.AppSettings["USER"];
            }
            set
            {
                UpdateConfig("USER", value.ToString());
            }
        }
        public static string CRONSYNCHROCLIENT
        {
            get
            {
                return ConfigurationManager.AppSettings["CRONSYNCHROCLIENT"];
            }
            set
            {
                UpdateConfig("CRONSYNCHROCLIENT", value.ToString());
            }
        }
        public static string CRONSYNCHROCLIENTDONE
        {
            get
            {
                return ConfigurationManager.AppSettings["CRONSYNCHROCLIENTDONE"];
            }
            set
            {
                UpdateConfig("CRONSYNCHROCLIENTDONE", value.ToString());
            }
        }
        public static string CategorieComptable { get { return ConfigurationManager.AppSettings["CATEGORIECOMPTABLEDEFAUT"]; } }
        public static string CategorieComptableForeigner { get { return ConfigurationManager.AppSettings["CATEGORIECOMPTABLEFOREIGNER"]; } }
        public static int CronTaskStock { get { return int.Parse(ConfigurationManager.AppSettings["CRONSTOCK"]); } }
        public static int CronTaskRestart { get { return int.Parse(ConfigurationManager.AppSettings["CRONTASKRESTART"]); } }

        public static string BaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["BASEURL"];
            }
            set
            {
                UpdateConfig("BASEURL", value.ToString());
            }
        }
        public static double TVA
        {
            get
            {
                return double.Parse(ConfigurationManager.AppSettings["TVA"]);
            }
            set
            {
                UpdateConfig("TVA", value.ToString());
            }
        }
        public static double TVACalculer { get { return (TVA + 100.00) / 100.00; } }

        #region CronTaskConfig
        public static int CronTaskUpdateStatut
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["CRONTASKUPDATESTATUT"]);
            }
            set
            {
                UpdateConfig("CRONTASKUPDATESTATUT", value.ToString());
            }
        }
        public static int CronTaskCheckForNewOrder
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["CRONTASKCHECKFORNEWORDER"]);
            }
            set
            {
                UpdateConfig("CRONTASKCHECKFORNEWORDER", value.ToString());
            }
        }
        #endregion

        public static string CompteGnum {
            get {
                string[] tokens = ConfigurationManager.AppSettings["COMPTG"].Split(new[] { "||" }, StringSplitOptions.None);
                return tokens[0].Trim();
            }
        }

        public static string CompteG
        {
            get
            {
                return ConfigurationManager.AppSettings["COMPTG"];
            }
            set
            {
                UpdateConfig("COMPTG", value);
            }
        }

        public static int ArrondiDigits {
            get {
                return Int32.Parse(ConfigurationManager.AppSettings["ARRONDI"]);
            }
            set 
            {   
                UpdateConfig("ARRONDI", value.ToString());
            }

        }

        public static string CatTarif
        {
            get
            {
                return ConfigurationManager.AppSettings["CATTARIF"];
            }
            set
            {
                UpdateConfig("CATTARIF", value);
            }
        }

        public static string CondLivraison
        {
            get
            {
                return ConfigurationManager.AppSettings["CONDLIVRAISON"];
            }
            set
            {
                UpdateConfig("CONDLIVRAISON", value);
            }
        }

        public static string Expedition
        {
            get
            {
                return ConfigurationManager.AppSettings["EXPEDITION"];
            }
            set
            {
                UpdateConfig("EXPEDITION", value);
            }
        }

        public static string Souche
        {
            get
            {
                return ConfigurationManager.AppSettings["SOUCHE"];
            }
            set
            {
                UpdateConfig("SOUCHE", value);
            }
        }

        public static string PrefixClient
        {
            get
            {
                return ConfigurationManager.AppSettings["PREFIXCLIENT"];
            }
            set
            {
                UpdateConfig("PREFIXCLIENT", value);
            }
        }

        public static string Gcm_User
        {
            get
            {
                return ConfigurationManager.AppSettings["GCM_USER"];
            }
            set
            {
                UpdateConfig("GCM_USER", value.ToString());
            }
        }

        public static string Gcm_Pass
        {
            get
            {
                return ConfigurationManager.AppSettings["GCM_PASS"];
            }
            set
            {
                UpdateConfig("GCM_PASS", value.ToString());
            }
        }

        public static string Gcm_Path
        {
            get
            {
                return ConfigurationManager.AppSettings["GCM_PATH"];
            }
            set
            {
                UpdateConfig("GCM_PATH", value.ToString());
            }
        }

        public static string Gcm_Set
        {
            get
            {
                return ConfigurationManager.AppSettings["GCM_SET"];
            }
            set
            {
                UpdateConfig("GCM_SET", value.ToString());
            }
        }

        public static string Mae_User
        {
            get
            {
                return ConfigurationManager.AppSettings["MAE_USER"];
            }
            set
            {
                UpdateConfig("MAE_USER", value.ToString());
            }
        }

        public static string Mae_Pass
        {
            get
            {
                return ConfigurationManager.AppSettings["MAE_PASS"];
            }
            set
            {
                UpdateConfig("MAE_PASS", value.ToString());
            }
        }

        public static string Mae_Path
        {
            get
            {
                return ConfigurationManager.AppSettings["MAE_PATH"];
            }
            set
            {
                UpdateConfig("MAE_PATH", value.ToString());
            }
        }

        public static string Mae_Set
        {
            get
            {
                return ConfigurationManager.AppSettings["MAE_SET"];
            }
            set
            {
                UpdateConfig("MAE_SET", value.ToString());
            }
        }
        public static string DefaultStock
        {
            get
            {
                return ConfigurationManager.AppSettings["DEFAULTSTOCK"];
            }
            set
            {
                UpdateConfig("DEFAULTSTOCK", value.ToString());
            }
        }
        
        public static Dictionary<string, string> OrderMapping
        {
            get
            {
                return (ConfigurationManager.GetSection("OrderSetting/OrderMapping") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
            }
            set
            {
            }
        }

        public static Dictionary<string, string> OrderCarrierMapping
        {
            get
            {
                return (ConfigurationManager.GetSection("OrderSetting/OrderCarrierMapping") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
            }
            set
            {
            }
        }



        private static void UpdateConfig(string key, string value)
        {

            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                settings[key].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        public static void AddNodeInCustomSection(string nodeKey, string key, string value)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            var nodeRegion = xmlDoc.CreateElement("add");
            nodeRegion.SetAttribute("key", key);
            nodeRegion.SetAttribute("value", value);

            xmlDoc.SelectSingleNode("//"+nodeKey).AppendChild(nodeRegion);
            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            ConfigurationManager.RefreshSection(nodeKey);
        }
    }
}
