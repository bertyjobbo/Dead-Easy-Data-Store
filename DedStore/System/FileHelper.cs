using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace DedStore.System
{
    /// <summary>
    /// Helper
    /// </summary>
    internal class FileHelper
    {
        private static object _syncLock = new object();

        // singleton
        private FileHelper() { }
        private static FileHelper _current;
        public static FileHelper Current { get { return _current ?? (_current = new FileHelper()); } }

        // fields
        internal readonly string FolderPath = ConfigurationManager.AppSettings["DedStorePath"];
        internal readonly IList<Type> SystemTypes = new[] { typeof(RegisteredType), typeof(LatestIntegerPrimaryKey) };

        /// <summary>
        /// Get text from file
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal string GetText(Type type)
        {
            lock (_syncLock)
            {
                // get path
                var filePath = GetTablePath(type);

                // filestream
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    // output
                    var output = string.Empty;

                    // check length (ie new file)
                    if (fileStream.Length > 0)
                    {
                        using (var streamReader = new StreamReader(fileStream, Encoding.Unicode))
                        {
                            output = streamReader.ReadToEnd();
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                    else
                    {
                        // get encoding and write empty text
                        var uniEncoding = new UnicodeEncoding();
                        fileStream.Write(uniEncoding.GetBytes(""), 0, uniEncoding.GetByteCount(""));
                        fileStream.Flush();
                        fileStream.Close();
                    }

                    //
                    return output;
                }
            }
        }

        /// <summary>
        /// Write text
        /// </summary>
        /// <param name="finalJson"></param>
        /// <param name="type"></param>
        internal void WriteText(string finalJson, Type type)
        {
            lock (_syncLock)
            {
                // get path
                var path = GetTablePath(type);

                // filestream
                using (var fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write))
                {
                    // get encoding
                    var uniEncoding = new UnicodeEncoding();

                    // write
                    fileStream.Write(uniEncoding.GetBytes(finalJson), 0, uniEncoding.GetByteCount(finalJson));

                    // finally
                    fileStream.Flush();
                    fileStream.Close();
                }

            }

        }

        internal string GetTablePath(Type type)
        {
            checkPath(type);
            return Path.Combine(FolderPath, (SystemTypes.Contains(type) ? "SystemTables\\" : "") + type.FullName + ".deds");
        }

        private void checkPath(Type type)
        {
            if (string.IsNullOrEmpty(FolderPath)) throw new Exception("Set 'DedStorePath' in <appsettings>");

            if (SystemTypes.Contains(type) && !Directory.Exists(Path.Combine(FolderPath, "SystemTables")))
                Directory.CreateDirectory(Path.Combine(FolderPath, "SystemTables"));
        }
    }
}
