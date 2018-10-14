using FluentAssertions;
using Xunit;

namespace DataDrop.Tests
{
    public class DataControllerTests
    {
        [Fact]
        public void ConstructorWithDefaultSettingsTest() {
            // act
            var sut = new DataController();

            // assert
            sut.Should().NotBeNull();
        }

        [Fact]
        public void ConstructorWithCustomSettingsTest() {
            // arrange
            var settings = new TestDataControllerSettings {
                                                              EnableEncryption = false,
                                                              FileNamePrefix = "test",
                                                              FileNameExtension = "dddb",
                                                              KeyValuePairDelimiter = ':'
                                                          };
            DataController sut;
            
            // act
            sut = new DataController(settings);
            
            // assert
            sut.Should().NotBeNull();
        }

        [Theory]
        [InlineData("test", "success")]
        [InlineData("value", "success")]
        public void DataControllerInsertMethodTests(string key, string value) {
            // arrange
            var settings = new TestDataControllerSettings {EnableEncryption = false};
            DataController sut = new DataController(settings);
            
            // act
            sut.Insert(key, value);
            
            // assert
            sut.ContainsKey(key).Should().BeTrue();
            sut.Lookup(key).Should().Be(value);
        }
    }
}