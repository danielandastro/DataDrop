using System.Security.Cryptography;
using DataDrop.Interfaces;

namespace DataDrop.Tests
{
    public class TestDataControllerSettings : IDataControllerSettings
    {
        public string FileNamePrefix { get; set; }
        public string FileNameExtension { get; set; }
        public string FileName => $"{FileNamePrefix}.{FileNameExtension}";
        public ICryptoTransform EncryptionProvider { get; set; }
        public bool EnableEncryption { get; set; }
        public bool PersistStore => false;
        public char KeyValuePairDelimiter { get; set; }
    }
}