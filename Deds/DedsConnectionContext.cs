using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Deds
{
    /// <summary>
    /// Connection context
    /// </summary>
    public class DedsConnectionContext : IDedsConnectionContext
    {
        #region Singleton

        private DedsConnectionContext()
        {
            _folderPath = ConfigurationManager.AppSettings["DedsConnection"];
            if (string.IsNullOrEmpty(_folderPath)) throw new Exception("Please add 'DedsConnection' to <appsettings> with the folder path to the Deds store folder");
            if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
        }
        private static IDedsConnectionContext _current;
        public static IDedsConnectionContext Current
        {
            get
            {
                return _current ?? (_current = new DedsConnectionContext());
            }
        }

        #endregion

        #region FIELDS

        //
        private readonly IDictionary<Type, DedsTableRowsPair> _tablesAndRows = new Dictionary<Type, DedsTableRowsPair>();
        private readonly object _lockObj = new object();
        private readonly string _folderPath;
        private readonly Type _attributeType = typeof(DedsKeyAttribute);

        #endregion

        #region INTERFACE

        /// <summary>
        /// Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DedsTableRowsPair<T> GetTableRowsPair<T>()
        {
            // type
            var typeOf = typeof(T);

            // check
            if (!_tablesAndRows.ContainsKey(typeOf))
            {
                _tablesAndRows.Add(typeOf, readAndCreateTableAndRows<T>());
            }

            // get from list
            return (DedsTableRowsPair<T>)_tablesAndRows[typeOf];
        }

        /// <summary>
        /// Read table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private DedsTableRowsPair<T> readAndCreateTableAndRows<T>()
        {
            lock (_lockObj)
            {

                // find file
                var filePath = getPathForType<T>();

                // primary keys
                var pkProp =
                    typeof(T).GetProperties()
                        .FirstOrDefault(x => x.CustomAttributes.Any(a => a.AttributeType == _attributeType));

                // check
                if (pkProp == null) throw new Exception("No primary key on " + filePath);

                // rows
                var collection = new DedsTableRowCollection<T>
                {
                    TypeOfPrimaryKey = pkProp.PropertyType,
                    TypeOfT = typeof(T),
                    PrimaryKeyPropertyInfo = pkProp
                };

                var output = new DedsTableRowsPair<T>
                {
                    Rows = collection,
                    Table = null
                };

                // check
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                    output.Table = new DedsTable<T> { InnerList = collection };
                    return output;
                }

                // read file
                var json = File.ReadAllText(filePath);

                // check
                if (string.IsNullOrEmpty(json))
                {
                    output.Table = new DedsTable<T> { InnerList = collection };
                    return output;
                }

                // read
                var jsonResults = JsonConvert.DeserializeObject<List<T>>(json);

                foreach (var item in jsonResults)
                {
                    var newRow = new DedsTableRow<T>
                    {
                        PrimaryKeyValue = pkProp.GetValue(item)
                    };
                    collection.List.Add(newRow);
                }

                //
                output.Table = new DedsTable<T>
                {
                    InnerList = collection
                };
                return output;
            }

        }

        /// <summary>
        /// Get file path for type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string getPathForType<T>()
        {
            return Path.Combine(_folderPath, typeof(T).FullName + ".deds");
        }

        #endregion
    }
}
