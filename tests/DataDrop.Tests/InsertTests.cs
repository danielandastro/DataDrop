using System;
using Xunit;

namespace DataDrop.Tests
{
    public class InsertTests
    {
        private readonly DataController database;
        
        public InsertTests()
        {
            database = new DataController("unitTestDB", "", false, false);
        }
        
        [Fact]
        public void InsertTest1()
        {
            database.Insert("sampleKey1", "sampleValue1");
            database.Insert("sampleKey2", "sampleValue2");
            Assert.True(database.ContainsKey("sampleKey1"));
            Assert.True(database.ValueCheck("sampleKey2", "sampleValue2"));
        }
    }
}
