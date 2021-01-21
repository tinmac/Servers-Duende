using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Encodings.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OidcClient _oidcClient = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            await Is4LoginAsync();
        }

        private async void btn_Test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await App.HubConn.InvokeAsync("ContactHub", new TestDto { Message = "Wpf Client says HELLO :-)" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("");
                Debug.WriteLine($"Wpf Client Call Hub.....");
                Debug.WriteLine($"exception:  {ex.Message}");
                Debug.WriteLine("");
            }
        }

        private async void btn_Test_api1_Click(object sender, RoutedEventArgs e)
        {
            await CallApiAsync();
        }


        public async Task Is4LoginAsync()
        {
            var options = new OidcClientOptions()
            {
                Authority = "https://localhost:5001",
                ClientId = "wpf",
                //ClientSecret ="secret",
                Scope = "openid sig1 api1 profile email",
                RedirectUri = "http://localhost/sample-wpf-app",
                Browser = new WpfEmbeddedBrowser()
            };

            _oidcClient = new OidcClient(options);

            LoginResult result;
            try
            {
                result = await _oidcClient.LoginAsync();

                if (result.IsError)
                {
                    Debug.WriteLine(result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error);
                }
                else
                {
                    var name = result.User.Identity.Name;
                    var claims = result.User.Claims.ToList();
                    var email = claims.FirstOrDefault(o => o.Type == "email")?.Value;
                    Debug.WriteLine("");
                    Debug.WriteLine($"Wpf Client.....");
                    Debug.WriteLine($"Hello {name} UN/PW accepted by IS4, token issued: {ShortenJWT(result.AccessToken)}  which has the following Claims:");
                    foreach (var claim in claims)
                    {
                        Debug.WriteLine($"{claim.Type} = {claim.Value}");
                    }
                    Debug.WriteLine("");

                    // We have a token, use it when we connect to SigmalR
                    App.Token = result.AccessToken;

                    await ConnectToHub();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Wpf Client..... exception: {ex.Message}");
                //return;
            }
        }


        private async Task CallApiAsync()
        {
            try
            {
                Debug.WriteLine("Calling Api...");
                var accessToken = App.Token;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = await client.GetStringAsync("https://localhost:6001/identity");
                var result = JArray.Parse(content).ToString();
                Debug.WriteLine($"Api result = {result}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("");
                Debug.WriteLine($"Wpf Client Call Api.....");
                Debug.WriteLine($"exception:  {ex.Message}");
                Debug.WriteLine("");
            }

        }


        private async Task ConnectToHub()
        {
            string url = $"{App.BaseAddress}sighub";
            var token = App.Token;

            try
            {
                App.HubConn = new HubConnectionBuilder()
               .ConfigureLogging(logging => {
                   // Log to the Console
                   //logging.AddConsole();

                   // Log to the Output Window
                   logging.AddDebug();

                   // This will set ALL logging to Debug level
                   logging.SetMinimumLevel(LogLevel.Debug);

                   logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug); 
               })
               .WithUrl(url, options =>
               {
                   options.SkipNegotiation = true;
                   options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                   
                   options.AccessTokenProvider = () => Task.FromResult(token);
                   Debug.WriteLine($"Wpf Client.....");
                   Debug.WriteLine($"Connecting to SyncHub @ {App.BaseAddress}  - Token: {ShortenJWT(App.Token)}");
                   Debug.WriteLine("");
               })
               .Build();


                // Closed Handler
                var closedTcs = new TaskCompletionSource<object>();
                App.HubConn.Closed += e =>
                {
                    closedTcs.SetResult(null);

                    return Task.CompletedTask;
                };

                // Register Hub Server endoints
                RegisterMethods();

                //Start Connection to Hub
                await App.HubConn.StartAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Wpf Client..... exception: {ex.Message}");
                Debug.WriteLine("");

                //throw ex;
            }
            finally
            {
                Debug.WriteLine($"Wpf Client..... Hub Status: {App.HubConn.State}");
                Debug.WriteLine("");
            }
        }


        private void RegisterMethods()
        {
            if (App.HubConn != null)
            {
                App.HubConn.On<TestDto>("CallAll", RxMsg);
            }
        }

        private void RxMsg(TestDto payload)
        {
            var msg = payload.Message;
            Debug.WriteLine($"Wpf Client.....");
            Debug.WriteLine($"Messgae: {msg}");
            Debug.WriteLine("");
        }


        //helpers
        private string ShortenJWT(string token)
        {
            // Pass back the first 3 & last 10 digits of the Token
            if (!string.IsNullOrWhiteSpace(token))
                return $"{token.Substring(0, 3)}...{token.Substring(token.Length - 10, 10)}";
            return "no token!";
        }

    }
}
