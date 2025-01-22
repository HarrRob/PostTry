using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAuthExample
{
    public class LocalServer
    {
        private static readonly HttpListener _listener = new HttpListener();
        private string _authorizationCode;

        // Starts the local HTTP server to listen for the authorization code
        public async Task StartLocalServer()
        {
            _listener.Prefixes.Add("http://localhost:8888/"); // URI prefix
            _listener.Start();
            Console.WriteLine("Listening for authorization code...");

            await Task.Run(() =>
            {
                while (true)
                {
                    HttpListenerContext context = _listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Check for /callback path
                    if (request.Url.AbsolutePath == "/callback")
                    {
                        var code = System.Web.HttpUtility.ParseQueryString(request.Url.Query)["code"];
                        if (!string.IsNullOrEmpty(code))
                        {
                            _authorizationCode = code; // Store the code
                            Console.WriteLine("Authorization code received: " + _authorizationCode);

                            break; // Exit loop after receiving the code
                        }
                    }
                }
                _listener.Stop(); // Stop the listener
            });
        }

        // Returns the stored authorization code
        public string GetAuthorizationCode() => _authorizationCode;
        //used website https://www.codeproject.com/Tips/485182/Create-a-local-server-in-Csharp
    }
}
