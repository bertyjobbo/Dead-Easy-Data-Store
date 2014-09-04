using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace DedStore.System
{
    /// <summary>
    /// Helper
    /// </summary>
    internal class FileHelper
    {
        // singleton
        private FileHelper() { }
        private static FileHelper _current;
        public static FileHelper Current { get { return _current ?? (_current = new FileHelper()); } }

        // fields
        internal  readonly string FolderPath = ConfigurationManager.AppSettings["DedStorePath"];
        internal  readonly IList<Type> SystemTypes = new[] { typeof(RegisteredType), typeof(LatestIntegerPrimaryKey) };

        /// <summary>
        /// Get text from file
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal  string GetText(Type type)
        {
            lock (new object())
            {
                // get path
                var path = GetTablePath(type);

                // check
                if (!File.Exists(path))
                {
                    File.Create(path);
                    return string.Empty;
                }

                //
                return File.ReadAllText(path);
            }
        }

        /// <summary>
        /// Write text
        /// </summary>
        /// <param name="finalJson"></param>
        /// <param name="type"></param>
        internal  void WriteText(string finalJson, Type type)
        {
            lock (new object())
            {
                var filePath = GetTablePath(type);
                File.WriteAllText(filePath, finalJson);
            }
        }

        internal  string GetTablePath(Type type)
        {
            checkPath(type);
            return Path.Combine(FolderPath, (SystemTypes.Contains(type) ? "SystemTables\\" : "") + type.FullName + ".deds");
        }

        private  void checkPath(Type type)
        {
            if (string.IsNullOrEmpty(FolderPath)) throw new Exception("Set 'DedStorePath' in <appsettings>");

            if (SystemTypes.Contains(type) && !Directory.Exists(Path.Combine(FolderPath, "SystemTables")))
                Directory.CreateDirectory(Path.Combine(FolderPath, "SystemTables"));
        }
    }
}
