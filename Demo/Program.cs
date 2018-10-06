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
            database2.Init("test2","", false, true);
            database.Init("test","", false, true);
            database.Insert("test", "dog");
            database2.Insert("dog", "test");
            database.Insert("try", "catch");
            
            Console.WriteLine(database.Lookup("test"));
            Console.WriteLine(database.Lookup("try"));
            Console.WriteLine(database.ValueCheck("test", "dog"));
            Console.WriteLine(database2.PresenceCheck("help"));
        }
    }
}