using DataDrop;
using NUnit.Framework;
using System;
namespace NuGet_Unit_Tests
{
    [TestFixture()]
    public class NugetTests
    {
        [Test()]
        public void IOTests()
        {
            var database = new DataController();
            database.Init("unitTestDB", "", false, false);
            database.Insert("sampleKey1", "sampleValue1");
            database.Insert("sampleKey2", "sampleValue2");
            Assert.True(database.PresenceCheck("sampleKey1") && database.ValueCheck("sampleKey2", "sampleValue2"));
        }
    }
}
