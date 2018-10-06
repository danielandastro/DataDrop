using System;
using DataDrop;

namespace Demo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var database = new DataController();
            //database.Init("test", false, true);
            database.Insert("test", "dog");

            database.Insert("dog", "test");
            database.Delete("dog");
        }
    }
}
