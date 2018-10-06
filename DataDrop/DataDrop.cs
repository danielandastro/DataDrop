using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataDrop
{

    public class DataController
    {
        private static Dictionary<string, string> stringHolders = new Dictionary<String, String>();
        private static bool encryptEnabled, persist, init;
        private static string name, fileName, encryptKey;
        public void Init(string nameInput, String inputKey, bool encryptInput, bool persistInput)
        {
            name = nameInput;
            encryptEnabled = encryptInput;
            persist = persistInput;
            fileName = nameInput + ".dddb";
            if (encryptInput) { encryptKey = inputKey;
                if (File.Exists(fileName))
                {
                    var lines = File.ReadLines(fileName);
                    foreach (var line in lines)
                    {
                        var temp = line.Split(',');
                        stringHolders[temp[0]] = Decrypt(temp[1]);
                    }
                }}
            else if (File.Exists(fileName))
            {
                var lines = File.ReadLines(fileName);
                foreach (var line in lines)
                {
                    var temp = line.Split(',');
                    stringHolders[temp[0]] = temp[1];
                }
            }
            init = true;

        }
        public void Insert(string key, string value)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            stringHolders[key] = value;
            if (persist) { Save(); }
        }
        private static void Save()
        {
            if (encryptEnabled)
            {
                foreach (KeyValuePair<string, string> kvp in stringHolders)
                {
                    File.WriteAllText(fileName, string.Format("{0}, {1} {2}", kvp.Key, Encrypt(kvp.Value), Environment.NewLine));
                }
            }
            else
            {

                foreach (KeyValuePair<string, string> kvp in stringHolders)
                {
                    File.WriteAllText(fileName, string.Format("{0}, {1} {2}", kvp.Key, kvp.Value, Environment.NewLine));
                }
            }
        }
        public void Delete(string key)
        {

            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            stringHolders.Remove(key);
            if (persist) { Save(); }
        }
        public string Lookup(string key)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            var value = "";
            stringHolders.TryGetValue(key, out value);
            return value;
        }

        public bool ValueCheck(string key, string expectedValue)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            var actualValue = "";
            return stringHolders.TryGetValue(key, out actualValue) &&
                                actualValue.Equals(expectedValue);
        }
        public bool PresenceCheck(string key)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            var value = "";
            return stringHolders.TryGetValue(key, out value);
        }
        public void RebuildDatabase()
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            File.Delete(fileName);
            Save();
        }
        public void Drop(bool confirm)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            if (confirm)
            {
                stringHolders.Clear();
            }
        }

        private static string Encrypt(string input)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(encryptKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string input)  
        {  
            byte[] inputArray = Convert.FromBase64String(input);  
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();  
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(encryptKey);  
            tripleDES.Mode = CipherMode.ECB;  
            tripleDES.Padding = PaddingMode.PKCS7;  
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();  
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);  
            tripleDES.Clear();   
            return UTF8Encoding.UTF8.GetString(resultArray);  
        }  
    }
}