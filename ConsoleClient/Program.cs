using System;

namespace ConsoleClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //await IdentityServiceHelper.GetPasswordToken();
            await IdentityServiceHelper.GetClientCredentialsToken();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
