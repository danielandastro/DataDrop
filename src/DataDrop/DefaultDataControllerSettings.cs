using System.Security.Cryptography;
using DataDrop.Interfaces;

namespace DataDrop
{
    public sealed class DefaultDataControllerSettings : IDataControllerSettings
    {
        public string FileNamePrefix => "datadrop";
        public string FileNameExtension => "dddb";
        public string FileName => $"{FileNamePrefix}.{FileNameExtension}";
        public ICryptoTransform EncryptionProvider => null;
        public bool EnableEncryption => false;
        public bool PersistStore => true;
        public char KeyValuePairDelimiter => ',';
    }
}