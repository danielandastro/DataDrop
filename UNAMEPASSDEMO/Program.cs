using System;
using System.Security;
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
            Console.WriteLine("Enter User Name: ");
            var uname = Console.ReadLine();
            Console.WriteLine("Enter Password: ");
            var pass = GetPWDMasked();
            database.Insert(uname, pass);

            Console.WriteLine("Now enter them");
            while (true)
            {
                Console.WriteLine("Enter User Name: ");
                uname = Console.ReadLine();
                Console.WriteLine("Enter Password: ");
                pass = GetPWDMasked();
                if (database.ValueCheck(uname, pass))
                {
                    Console.WriteLine("Correct");
                }
                else { Console.WriteLine("Username or password incorrect"); }
            }
        }

        private static string GetPWDMasked()
        {
            SecureString pwd = new SecureString( );
            while ( true )
            {
                ConsoleKeyInfo i = Console.ReadKey( true );
                if ( i.Key == ConsoleKey.Enter )
                {
                    break;
                }
                else if ( i.Key == ConsoleKey.Backspace )
                {
                    pwd.RemoveAt( pwd.Length - 1 );
                    Console.Write( "\b \b" );
                }
                else
                {
                    pwd.AppendChar( i.KeyChar );
                    Console.Write( "*" );
                }
            }
            return pwd.ToString();
        }
    }
}
