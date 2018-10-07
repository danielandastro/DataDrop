using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataDrop
{

    public class DataController
    {
        public DataController(string nameInput, string inputKey, bool encryptInput, bool persistInput, bool threadSafeSaveinput)
        {
            name = nameInput;
            encryptEnabled = encryptInput;
            persist = persistInput;
            fileName = nameInput + ".dddb";
            threadSafeSaveinput = threadSafeSave;
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
            if(threadSafeSave) ThreadSafeSave();
        }
        private ConcurrentDictionary<string, string> stringHolders = new ConcurrentDictionary<string, string>();
        private bool encryptEnabled, persist, init, threadSafeSave;
        private string name, fileName, encryptKey;
        public void Insert(string key, string value)
        {
            if (!init) { throw new Exception("Datacontroller not initialised, please use DataController.Init()"); }
            stringHolders[key] = value;
            if (persist && !threadSafeSave) { Save(); }
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
            if (persist&&!threadSafeSave) { Save(); }
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
            if (persist && !threadSafeSave) { Save(); }
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
        async Task ThreadSafeSave()
        {

            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                while (true)
                {
                    System.Threading.Thread.Sleep(10000);
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
            });
        }
    }
}