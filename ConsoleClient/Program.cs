using System;

namespace ConsoleClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string key = Console.ReadLine();
            if ("password" == key)
            {
                await IdentityServiceHelper.GetPasswordToken();
            }
            else
            {
                await IdentityServiceHelper.GetClientCredentialsToken();
            }
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
