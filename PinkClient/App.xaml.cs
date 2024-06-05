using System.Configuration;
using System.Windows;

namespace PinkClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ApiHost = "";
        public static string ApiVersion = "";

        public App()
        {
            App.ApiHost = ConfigurationManager.AppSettings["ApiHost"] switch
            {
                null => ConfigurationManager.AppSettings["ApiHost"],
                _ => "http://localhost:5000"
            };
            App.ApiVersion = ConfigurationManager.AppSettings["ApiVersion"] switch
            {
                null => ConfigurationManager.AppSettings["ApiVersion"],
                _ => "1"
            };
        }
    }

}
