using System;
using System.Diagnostics;

namespace authURL
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Your Spotify app credentials and settings
            string clientId = "f9a18ea2ae15426a8665d4df92feb418";
            string redirectUri = "0ec970e65f81426baa48cdac1d6780b2";
            string scopes = "user-library-read user-top-read"; // Define the scopes you need

            // Construct the authorization URL
            string authorizationUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&scope={Uri.EscapeDataString(scopes)}&redirect_uri={Uri.EscapeDataString(redirectUri)}";

            // Print URL to console
            Console.WriteLine("Opening the following URL to authorize the application:");
            Console.WriteLine(authorizationUrl);

            // Open the URL in the default web browser
            Process.Start(new ProcessStartInfo
            {
                FileName = authorizationUrl,
                UseShellExecute = true // This ensures the URL is opened in the default web browser
            });
        }
    }
}
