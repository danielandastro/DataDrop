using System;
using DataDrop;

namespace Demo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var database = new DataController();
            var database2 = new DataController();
            database2.Init("test2","", false, false);
            database.Init("test","", false, true);
            database.Insert("test", "dog");
            database.Insert("dog", "test");
            database2.Insert("try", "catch");
            database.Delete("dog");
            Console.WriteLine(database.Lookup("test"));
            Console.WriteLine(database2.Lookup("try"));
            Console.WriteLine(database.ValueCheck("test", "dog"));
            Console.WriteLine(database.PresenceCheck("help"));
        }
    }
}
