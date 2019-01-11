using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public class IdentityServiceHelper
    {
        public static async Task GetTask()
        {
            try
            {
                var client = new HttpClient();

                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }
                var tokenResponse = await client.RequestTokenAsync(new TokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "secret",
                    GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                    Parameters =
                    {
                        { "scope", "api1" }
                    }
                });
                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }
                Console.WriteLine(tokenResponse.Json);

                client.SetBearerToken(tokenResponse.AccessToken);
                var response = await client.GetAsync("http://localhost:5000/api/values/");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
