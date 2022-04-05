using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WebservicesSage.Singleton
{
    public sealed class SingletonUI
    {
        private static readonly Lazy<SingletonUI> lazy =
            new Lazy<SingletonUI>(() => new SingletonUI());

        public static SingletonUI Instance { get { return lazy.Value; } }

        public Label MenuTitle { get; set; }

        public Bunifu.Framework.UI.BunifuCircleProgressbar ClientCircleProgress { get; set; }
        public Bunifu.Framework.UI.BunifuCircleProgressbar catProgress { get; set; }
        public Bunifu.Framework.UI.BunifuProgressBar ProgressBar { get; set; }
        public Label ArticleNumber { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox LogBox { get; set; }

        public Bunifu.Framework.UI.BunifuDropdown ComptGConf { get; set; }
        public Bunifu.Framework.UI.BunifuDropdown CatTarifConf { get; set; }
        public Bunifu.Framework.UI.BunifuDropdown CondLivraisonConf { get; set; }
        public Bunifu.Framework.UI.BunifuDropdown ExpeditionConf { get; set; }
        public Bunifu.Framework.UI.BunifuMaterialTextbox PrefixClientConf { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox ArticleConfigurationArrondiInput { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox ArticleConfigurationTVAInput { get; set; }
        public Panel currentPanel { get; set; }

        public Bunifu.Framework.UI.BunifuCustomLabel NotificationLabel { get; set; }
        public Bunifu.Framework.UI.BunifuCustomLabel ErrorNotificationLabel { get; set; }
        public void ShowNotification(String NotificationMessage)
        {
            NotificationLabel.Visible = true;
            NotificationLabel.Text = NotificationMessage;
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => HideNotification());
            
        }

        public void ShowErrorNotification(String ErrorNotificationMessage)
        {
            ErrorNotificationLabel.Text = ErrorNotificationMessage;
            ErrorNotificationLabel.Visible = true;
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => HideErrorNotification());

        }
        private void HideNotification()
        {
            SingletonUI.Instance.NotificationLabel.Invoke((MethodInvoker)(() => SingletonUI.Instance.NotificationLabel.Visible = false));
        }
        private void HideErrorNotification()
        {
            SingletonUI.Instance.ErrorNotificationLabel.Invoke((MethodInvoker)(() => SingletonUI.Instance.ErrorNotificationLabel.Visible = false));
            SingletonUI.Instance.NotificationLabel.Invoke((MethodInvoker)(() => ShowNotification("Enregistrement effectuer avec Succée")));

        }
        public Bunifu.Framework.UI.BunifuDropdown SoucheDropdown { get; set; }
        public WindowsFormsControlLibrary1.BunifuCustomTextbox PrefixClient { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox BaseURLConfiguration { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox UserConfiguration { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox Gcm_User { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox Gcm_Pass { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox Gcm_Path { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox Mae_User { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox Mae_Pass { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox Mae_Path { get; set; }

        public Bunifu.Framework.UI.BunifuiOSSwitch MAE_set { get; set; }

        public Bunifu.Framework.UI.BunifuiOSSwitch GCM_set { get; set; }
        public Bunifu.Framework.UI.BunifuiOSSwitch DefaultStock { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox CronTaskUpdateStatut { get; set; }

        public WindowsFormsControlLibrary1.BunifuCustomTextbox CronTaskCheckNewOrder { get; set; }
        public WindowsFormsControlLibrary1.BunifuCustomTextbox SynchroClientCron { get; set; }

        private SingletonUI()
        {

        }
    }
}
