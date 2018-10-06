using System;
using DataDrop;
namespace UNAMEPASSDEMO
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var database = new DataController();
            database.Init("pssd", false, false);
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
