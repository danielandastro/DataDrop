using System;
using System.Security.Cryptography;
using DataDrop;
namespace UNAMEPASSDEMO
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();
            var database = new DataController();
            database.Init("pssd","sblt-3hn8-sqoy18", true, true);
            Console.WriteLine("UserName and Password demo");
            Console.Write("Enter User Name: ");
            var uname = Console.ReadLine();
            Console.Write("Enter Password: ");
            var pass = Console.ReadLine();
            database.Insert(uname, pass);

            Console.WriteLine("Now enter them");
            while (true)
            {
                Console.Write("Enter User Name: ");
                uname = Console.ReadLine();
                Console.Write("Enter Password: ");
                pass = Console.ReadLine();
                if (database.ValueCheck(uname, pass))
                {
                    Console.WriteLine("Correct");
                }
                else { Console.WriteLine("Username or password incorrect"); }
            }
        }
    }
}
