using System;
using DataDrop;

namespace Demo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var database = new DataController("test", "", false, true);
            var database2 = new DataController("test2", "", false, true);
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