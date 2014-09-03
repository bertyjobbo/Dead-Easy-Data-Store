using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    /// <summary>
    /// Helper
    /// </summary>
    internal static class FileHelper
    {
        // fields
        internal static readonly string FolderPath = ConfigurationManager.AppSettings["DedStorePath"];

        /// <summary>
        /// Get text from file
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string GetText(Type type)
        {
            if(string.IsNullOrEmpty(FolderPath)) throw new Exception("Set 'DedStorePath' in <appsettings>");
            lock (new object())
            {
                // get path
                var path = getPathForType(type);

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
        /// Get path for type
        /// </summary>
        /// <param name="typeOfT"></param>
        /// <returns></returns>
        private static string getPathForType(Type typeOfT)
        {
            return Path.Combine(FolderPath, typeOfT.FullName + ".deds");
        }

        /// <summary>
        /// Write text
        /// </summary>
        /// <param name="finalJson"></param>
        /// <param name="type"></param>
        internal static void WriteText(string finalJson, Type type)
        {
            var filePath = getPathForType(type);
            File.WriteAllText(filePath,finalJson);
        }
    }
}
