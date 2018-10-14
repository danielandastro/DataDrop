using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataDrop.Interfaces;

namespace DataDrop
{
    /// <summary>
    /// Class to create a key/value pair store in the form of either instance based or file.
    /// </summary>
    public class DataController
    {
        /// <summary>
        /// The settings to use for the store.
        /// </summary>
        protected readonly IDataControllerSettings Settings;
        
        /// <summary>
        /// The internal dictionary for the key/value pair.
        /// </summary>
        protected IDictionary<string, string> DataStore { get; set; }

        /// <summary>
        /// Default constructor that will initialize with the default settings.
        /// </summary>
        public DataController() : this(new DefaultDataControllerSettings()) {
            
        }

        /// <summary>
        /// Constructor that takes in custom settings to utilize for the data store.
        /// </summary>
        /// <param name="settings">The settings to use for the data store.</param>
        public DataController(IDataControllerSettings settings) {
            Settings = settings;
            Load();
        }
        
        /// <summary>
        /// Upserts a key/value pair.
        /// </summary>
        /// <param name="key">The key to update or insert.</param>
        /// <param name="value">The value associated with the key.</param>
        public void Insert(string key, string value) {
            DataStore[key] = value;
            Save();
        }
        
        /// <summary>
        /// Removes a key/value pair from the store.
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key) {
            DataStore.Remove(key);
            Save();
        }
        
        /// <summary>
        /// Attempts to get a value out of the store by key. If it doesn't exist it returns null.
        /// </summary>
        /// <param name="key">The key associated to a specific value.</param>
        /// <returns>The value associated with the key that was passed in.</returns>
        public string Lookup(string key) {
            DataStore.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>
        /// Boolean check to see if a value is what it is expected to be.
        /// </summary>
        /// <param name="key">The key to the value that's being checked.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="valueComparer">The type of string comparison to use. [Default is InvariantCulture]</param>
        /// <returns>A boolean value.</returns>
        public bool IsValueEqual(string key, string expectedValue, StringComparison valueComparer = StringComparison.InvariantCulture) {
            DataStore.TryGetValue(key, out var value);
            return expectedValue.Equals(value, valueComparer);
        }

        /// <summary>
        /// A lookup to see if the store contains a specific key.
        /// </summary>
        /// <param name="key">The value to verify existence of.</param>
        /// <returns>A boolean value.</returns>
        public bool ContainsKey(string key) {
            return DataStore.ContainsKey(key);
        }

        /// <summary>
        /// A lookup to see if a specific key/value pair exists.
        /// </summary>
        /// <param name="key">The key to validate.</param>
        /// <param name="value">The value to validate.</param>
        /// <param name="valueComparer">The type of string comparison to use. [Default is InvariantCulture]</param>
        /// <returns>A boolean value.</returns>
        public bool ContainsPair(string key, string value, StringComparison valueComparer = StringComparison.InvariantCulture) {
            return DataStore.ContainsKey(key) && DataStore[key].Equals(value, valueComparer);
        }

        /// <summary>
        /// Used to rebuild the store.
        /// </summary>
        /// <remarks>Probably unnecessary.</remarks>
        public void RebuildStore() {
            File.Delete(Settings.FileName);
            Save();
        }

        /// <summary>
        /// Used to clear the current store.
        /// </summary>
        public void ClearCurrentStore() {
            DataStore.Clear();
            File.Delete(Settings.FileName);
        }

        /// <summary>
        /// Used to load the data from the file into the data store dictionary.
        /// </summary>
        protected virtual void Load() {
            Action<Func<string, string>> createDataStoreFactory = process => {
                                             DataStore = (from line in File.ReadLines(Settings.FileName)
                                              let pair = line.Split(Settings.KeyValuePairDelimiter)
                                              select new {Key = pair.First(), Value = process(pair.Last())})
                                                 .ToDictionary(k => k.Key, v => v.Value);
                                         };
            if (File.Exists(Settings.FileName)) {
                if (Settings.EnableEncryption) {
                    createDataStoreFactory(Decrypt);
                }
                else {
                    createDataStoreFactory(value => value);
                }
            }
            else {
                DataStore = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Provides a way to decrypt values in a pair if encryption is enabled.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The unencrypted value.</returns>
        protected string Decrypt(string input) {
            var inputArray = Convert.FromBase64String(input);
            var resultArray = Settings.EncryptionProvider.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        private void Save() {
            if (!Settings.PersistStore) return;
            Action<Func<string, string>, IDictionary<string, string>> writeFileFactory =
                (process, store) => {
                    var storeSet = store
                        .Select(pair => $"{pair.Key}{Settings.KeyValuePairDelimiter}{process(pair.Value)}");
                    File.WriteAllText(Settings.FileName, string.Join(Environment.NewLine, storeSet));
                };
            if (Settings.EnableEncryption) {
                writeFileFactory(Encrypt, DataStore);
            }
            else {
                writeFileFactory(v => v, DataStore);
            }
        }

        private string Encrypt(string input) {
            var inputArray = Encoding.UTF8.GetBytes(input);
            var resultArray = Settings.EncryptionProvider.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }

    /// <inheritdoc />
    /// <typeparam name="TStore">Any implementation of IDictionary{string, string}.</typeparam>
    public sealed class DataController<TStore> : DataController where TStore : class, IDictionary<string, string>
    {
        private new TStore DataStore { get; set; }

        /// <inheritdoc />
        public DataController() : this(new DefaultDataControllerSettings()) {
            
        }

        /// <inheritdoc />
        public DataController(IDataControllerSettings settings) : base(settings) {
            
        }

        /// <inheritdoc />
        protected override void Load() {
            Action<Func<string, string>> createDataStoreFactory = process => {
                                                                      DataStore = (from line in File.ReadLines(Settings.FileName)
                                                                                   let pair = line.Split(',')
                                                                                   select new {Key = pair.First(), Value = process(pair.Last())})
                                                                          .ToDictionary(k => k.Key, v => v.Value) as TStore;
                                                                  };
            if (File.Exists(Settings.FileName)) {
                if (Settings.EnableEncryption) {
                    createDataStoreFactory(Decrypt);
                }
                else {
                    createDataStoreFactory(value => value);
                }
            }
            else {
                DataStore = Activator.CreateInstance<TStore>();
            }
        }
    }
}