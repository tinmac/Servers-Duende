using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;


namespace WpfClient
{
    public partial class App : Application
    {
        // Props
        public static HubConnection HubConn { get; set; }
        public static Uri BaseAddress { get; set; }
        public static string Token { get; set; }


        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Set Hub Server IP
            App.BaseAddress = new Uri($"http://localhost:5050/");
            //App.BaseAddress = new Uri("http://localhost:54787/");

        }
    }
}
