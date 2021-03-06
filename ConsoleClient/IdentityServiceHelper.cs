﻿using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public class IdentityServiceHelper
    {
        public static async Task GetClientCredentialsToken()
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
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api1"
                });
                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }
                Console.WriteLine(tokenResponse.Json);

                client.SetBearerToken(tokenResponse.AccessToken);
                var response = await client.GetAsync("http://localhost:5001/identity");
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
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task GetPasswordToken()
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
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "ro.client",
                    ClientSecret = "secret",
                    UserName= "alice",
                    Password= "password",
                    Scope = "api1"
                });
                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }
                Console.WriteLine(tokenResponse.Json);

                client.SetBearerToken(tokenResponse.AccessToken);
                var response = await client.GetAsync("http://localhost:5001/identity");
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
                Console.WriteLine(ex.Message);
            }
        }
    }
}
