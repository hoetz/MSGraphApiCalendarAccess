using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Net.Http.Headers;

namespace ConsoleApp1
{
    partial class Program
    {
        public class AuthenticatedGraphServiceClient
        {
            // Get an authenticated Microsoft Graph Service client.
            public static GraphServiceClient Get(IConfigurationRoot config)
            {
                GraphServiceClient graphClient = new GraphServiceClient(
                    new DelegateAuthenticationProvider(
                        async (requestMessage) =>
                        {
                            string accessToken = await AdalAuthProvider.CreateInstance(config).GetUserAccessTokenAsync();

                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                        // Get event times in the current time zone.
                        requestMessage.Headers.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");
                        }));
                return graphClient;
            }
        }
    }
}