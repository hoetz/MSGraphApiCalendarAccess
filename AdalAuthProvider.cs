using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    partial class Program
    {
        public class AdalAuthProvider
        {
            private string CreateAuthorityFrom (string aadInstance, string tenant) => String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            private static string MSGraphResourceId = "https://graph.microsoft.com/";
            private static AuthenticationContext authContext = null;
            private static ClientCredential clientCredential = null;

            private AdalAuthProvider(IConfigurationRoot config)
            {
                authContext = new AuthenticationContext(CreateAuthorityFrom(config["aadInstance"],config["tenant"]));
                clientCredential = new ClientCredential(config["clientId"], config["appKey"]);
            }

            public static AdalAuthProvider CreateInstance(IConfigurationRoot config)
            {
                return new AdalAuthProvider(config);
            }
            

            public async Task<string> GetUserAccessTokenAsync()
            {

                AuthenticationResult result = null;
                int retryCount = 0;
                bool retry = false;
                do
                {
                    retry = false;
                    try
                    {
                        result = await authContext.AcquireTokenAsync(MSGraphResourceId, clientCredential);
                    }
                    catch (AdalException ex)
                    {
                        if (ex.ErrorCode == "temporarily_unavailable")
                        {
                            retry = true;
                            retryCount++;
                            Thread.Sleep(3000);
                        }

                        Console.WriteLine(
                            String.Format("An error occurred while acquiring a token\nTime: {0}\nError: {1}\nRetry: {2}\n",
                            DateTime.Now.ToString(),
                            ex.ToString(),
                            retry.ToString()));
                    }

                } while ((retry == true) && (retryCount < 3));

                if (result == null)
                {
                    Console.WriteLine("Canceling attempt to contact To Do list service.\n");
                    return null;
                }
                return result.AccessToken;
            }
        }
    }
}