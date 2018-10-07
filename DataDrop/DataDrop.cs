using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataDrop
{

    public class DataController
    {
        public DataController(string nameInput, string inputKey, bool encryptInput, bool persistInput){
            name = nameInput;
            encryptEnabled = encryptInput;
            persist = persistInput;
            fileName = nameInput + ".dddb";
            if (encryptInput)
            {
                encryptKey = inputKey;
                if (File.Exists(fileName))
                {
                    var lines = File.ReadLines(fileName);
                    foreach (var line in lines)
                    {
                        var temp = line.Split(',');
                        stringHolders[temp[0]] = Decrypt(temp[1]);
                    }
                }
            }
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
        private ConcurrentDictionary<string, string> stringHolders = new ConcurrentDictionary<string, string>();
        private bool encryptEnabled, persist, init;
        private string name, fileName, encryptKey;
        public void Insert(string key, string value)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            stringHolders[key] = value;
            if (persist) { Save(); }
        }
        private void Save()
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

                foreach (var kvp in stringHolders)
                {
                    File.WriteAllText(fileName, string.Format("{0}, {1} {2}", kvp.Key, kvp.Value, Environment.NewLine));
                }
            }
        }
        public void Delete(string key)
        {

            if (!init) { InitException(); }
            stringHolders.TryRemove(key, out var grab);
            if (persist) { Save(); }
        }
        public string Lookup(string key)
        {
            if (!init) { InitException(); }

            stringHolders.TryGetValue(key, out var value);
            return value;
        }

        public bool ValueCheck(string key, string expectedValue)
        {
            if (!init) { InitException(); }

            return stringHolders.TryGetValue(key, out var actualValue) &&
                                actualValue.Equals(expectedValue);
        }
        public bool ContainsKey(string key)
        {
            if (!init)
            {
                InitException();
            }

            return stringHolders.TryGetValue(key, out _);
        }

        private static void InitException()
        {
            throw new Exception("Datacontroller not initialised, please use DataController.Init()");
        }

        public void RebuildDatabase()
        {
            if (!init) { InitException(); }
            File.Delete(fileName);
            Save();
        }
        public void Drop(bool confirm)
        {
            if (!init) { InitException(); }
            if (confirm)
            {
                stringHolders.Clear();
            }
        }

        private string Encrypt(string input)
        {
            var inputArray = UTF8Encoding.UTF8.GetBytes(input);
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(encryptKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private string Decrypt(string input)  
        {  
            var inputArray = Convert.FromBase64String(input);  
            var tripleDES = new TripleDESCryptoServiceProvider();  
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(encryptKey);  
            tripleDES.Mode = CipherMode.ECB;  
            tripleDES.Padding = PaddingMode.PKCS7;  
            var cTransform = tripleDES.CreateDecryptor();  
            var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);  
            tripleDES.Clear();   
            return UTF8Encoding.UTF8.GetString(resultArray);  
        }  
    }
}