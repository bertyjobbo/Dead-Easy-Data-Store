using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace DedStore
{
    /// <summary>
    /// Context
    /// </summary>
    public class DedStoreContext
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DedStoreContext()
        {
            _getDataMethod = (type) =>
            {
                var text = FileHelper.GetText(type);
                if (string.IsNullOrWhiteSpace(text)) return new Hashtable();
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var data = JsonConvert.DeserializeObject(text, collectionType);
                return (IEnumerable)data;
            };

            _commitMethod = (collections) =>
            {
                lock (new object())
                {
                    // start output
                    var output = new DedStoreResponse
                    {
                        ResponseType = DedStoreResponseType.Commit
                    };

                    try
                    {


                        // collections to commit
                        var collectionsToCommit = _collectionsInContext.Where(x => x.Value.TableRows.Any(y => y.Added));

                        // loop
                        foreach (var collection in collectionsToCommit.ToList())
                        {
                            // pk

                            // get table
                            var freshCollection = readTextToCollection(collection.Key);

                            // str
                            var str = new StringBuilder("[");
                            var strs = new List<string>();

                            // add
                            collection.Value.TableRows.Where(x => x.Added).ToList().ForEach(item => freshCollection.TableRows.Add(item));
                           

                            // loop collection
                            foreach (var row in freshCollection.TableRows)
                            {

                                // check
                                if (row.Added)
                                {
                                    object pk;
                                    if (freshCollection.TypeOfPrimaryKey == typeof (int))
                                    {
                                        pk = getNextIntegerPrimaryKey(collection.Key);
                                        row.SetRawItemPrimaryKey(pk, collection.Value.PrimaryKeyPropertyInfo);
                                    }
                                    else
                                    {
                                        pk = row.PrimaryKey;
                                        if (pk == null ||
                                            (typeof (string) == collection.Value.TypeOfPrimaryKey &&
                                             string.IsNullOrEmpty((string) pk)) ||
                                            (collection.Value.TypeOfPrimaryKey == typeof (Guid) &&
                                             ((Guid) pk) == Guid.Empty))
                                        {
                                            throw new Exception("No primary key found on item");
                                        }
                                    }
                                }

                                // add
                                strs.Add(JsonConvert.SerializeObject(row.RawItem));
                            }

                            // commit
                            str.Append(string.Join(",", strs));
                            str.Append("]");
                            var finalJson = str.ToString();
                            FileHelper.WriteText(finalJson, collection.Key);

                            // get again
                            _collectionsInContext[collection.Key] = readTextToCollection(collection.Key);

                        }
                    }
                    catch (Exception ex)
                    {
                        output.ErrorMessage = ex.Message +
                                              (ex.InnerException == null
                                                  ? ""
                                                  : ". InnerException: " + ex.InnerException.Message);
                    }
                    return output;
                }
            };
        }

        /// <summary>
        /// Get pk int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int getNextIntegerPrimaryKey(Type key)
        {
            var filePath = Path.Combine(FileHelper.FolderPath, "INTPK_" + key.FullName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "1");
                return 1;
            }
            var output = Convert.ToInt32(File.ReadAllText(filePath)) + 1;
            File.WriteAllText(filePath, output.ToString());
            return output;
        }

        /// <summary>
        /// Testing constructor
        /// </summary>
        /// <param name="getDataMethod"></param>
        /// <param name="commitMethod"></param>
        public DedStoreContext(Func<Type, IEnumerable> getDataMethod, Func<IDictionary<Type, DedStoreTableRowCollection>, DedStoreResponse> commitMethod)
        {
            _commitMethod = commitMethod;
            _getDataMethod = getDataMethod;
        }

        // fields
        private readonly IDictionary<Type, DedStoreTableRowCollection> _collectionsInContext = new Dictionary<Type, DedStoreTableRowCollection>();
        private readonly Func<Type, IEnumerable> _getDataMethod;
        private readonly Func<IDictionary<Type, DedStoreTableRowCollection>, DedStoreResponse> _commitMethod;

        /// <summary>
        /// Get table
        /// </summary>
        /// <returns></returns>
        public DedStoreTable<T> GetTable<T>()
        {
            var type = typeof(T);
            if (!_collectionsInContext.ContainsKey(type))
            {
                _collectionsInContext.Add(type, readTextToCollection(type));
            }
            return new DedStoreTable<T>(_collectionsInContext[type]);
        }



        /// <summary>
        /// Read json to collection
        /// </summary>
        /// <returns></returns>
        private DedStoreTableRowCollection readTextToCollection(Type type)
        {
            // pk prop info
            PropertyInfo pkPropInfo = null;

            // get custom attrs
            var props =
                type.GetProperties()
                    .Where(x => x.CustomAttributes.Any(p => p.AttributeType == typeof(DedStorePrimaryKeyAttribute))).ToList();

            // check
            if (props.Count > 1 || props.Count < 0) throw new Exception("Type " + type.FullName + " has no primary key or too many primary keys (using 'DedStorePrimaryKeyAttribute')");

            // get
            pkPropInfo = props.Single();

            // hashtable
            var objects = (IEnumerable<object>)_getDataMethod(type);

            // start
            var collection = new DedStoreTableRowCollection(type)
            {
                TableRows =
                objects
                .Select(x => new DedsStoreTableRow(type)
                    {
                        RawItem = x,
                        Added = false,
                        PrimaryKey = pkPropInfo.GetValue(x)
                    })
                    .ToList(),
                PrimaryKeyPropertyInfo = pkPropInfo
            };

            //
            return collection;

        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <returns></returns>
        public DedStoreResponse Commit()
        {
            return _commitMethod.Invoke(_collectionsInContext);
        }
    }
}
