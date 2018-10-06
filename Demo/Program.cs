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
            DataController.Insert("test", "dog");
            DataController.Insert("dog", "test");
            DataController.Insert("try", "catch");
            DataController.Delete("dog");
            Console.WriteLine(DataController.Lookup("test"));
            Console.WriteLine(DataController.Lookup("try"));
            Console.WriteLine(DataController.ValueCheck("test", "dog"));
            Console.WriteLine(DataController.PresenceCheck("help"));
        }
    }
}
