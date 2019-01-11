using System;

namespace ConsoleClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            await IdentityServiceHelper.GetTask();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
