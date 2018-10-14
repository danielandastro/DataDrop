using System.Security.Cryptography;

namespace DataDrop.Interfaces
{
    public interface IDataControllerSettings
    {
        string FileNamePrefix { get; }
        string FileNameExtension { get; }
        string FileName { get; }
        ICryptoTransform EncryptionProvider { get; }
        bool EnableEncryption { get; }
        bool PersistStore { get; }
        char KeyValuePairDelimiter { get; }
    }
}