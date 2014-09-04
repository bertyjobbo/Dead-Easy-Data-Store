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
            _getDataMethod = getDataMethod();

            _commitMethod = commitMethod();

        }

        /// <summary>
        /// Testing constructor
        /// </summary>
        /// <param name="getDataMethod"></param>
        /// <param name="commitMethod"></param>
        public DedStoreContext(Func<Type, IEnumerable<object>> getDataMethod, Func<IDictionary<Type, DedStoreTableRowCollection>, DedStoreResponse> commitMethod)
        {
            _commitMethod = commitMethod;
            _getDataMethod = getDataMethod;
        }

        // fields
        private readonly IDictionary<Type, DedStoreTableRowCollection> _collectionsInContext = new Dictionary<Type, DedStoreTableRowCollection>();
        private readonly Func<Type, IEnumerable<object>> _getDataMethod;
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
        /// Commit
        /// </summary>
        /// <returns></returns>
        public DedStoreResponse Commit()
        {
            return _commitMethod.Invoke(_collectionsInContext);
        }

        /// <summary>
        /// Check primary key for uniqueness
        /// </summary>
        /// <param name="typeOfPk"></param>
        /// <param name="type"></param>
        private void checkType(Type typeOfPk, Type type)
        {
            if (typeOfPk == typeof(int) || typeOfPk == typeof(string) || typeOfPk == typeof(Guid))
                return;

            File.Delete(FileHelper.GetTablePath(type));
            throw new Exception("Primary key must be Integer, Guid or String");

        }

        /// <summary>
        /// Get pk int
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int getNextIntegerPrimaryKey(Type type)
        {
            var filePath = FileHelper.GetIntegerPath(type);
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
        /// Get data method (standard)
        /// </summary>
        /// <returns></returns>
        private Func<Type, IEnumerable<object>> getDataMethod()
        {
            return (type) =>
            {
                var text = FileHelper.GetText(type);
                if (string.IsNullOrWhiteSpace(text)) return JsonConvert.DeserializeObject<IEnumerable<object>>("[]");
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var data = JsonConvert.DeserializeObject(text, collectionType);
                return (IEnumerable<object>)data;
            };
        }

        /// <summary>
        /// Commit methods
        /// </summary>
        /// <returns></returns>
        private Func<IDictionary<Type, DedStoreTableRowCollection>, DedStoreResponse> commitMethod()
        {
            return (collections) =>
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
                            // check pk
                            checkType(collection.Value.TypeOfPrimaryKey, collection.Key);

                            // get table
                            var freshCollection = readTextToCollection(collection.Key);

                            // str
                            var str = new StringBuilder("[");
                            var strs = new List<string>();

                            // get pks before add
                            var pks = freshCollection.TableRows.Select(x => x.PrimaryKey.ToString().ToLower()).ToList();

                            // add
                            collection.Value.TableRows.Where(x => x.Added).ToList().ForEach(item => freshCollection.TableRows.Add(item));



                            // loop collection
                            foreach (var row in freshCollection.TableRows)
                            {

                                // check
                                if (row.Added)
                                {
                                    object pk;
                                    if (freshCollection.TypeOfPrimaryKey == typeof(int))
                                    {
                                        pk = getNextIntegerPrimaryKey(collection.Key);
                                        row.SetRawItemPrimaryKey(pk, collection.Value.PrimaryKeyPropertyInfo);
                                    }
                                    else
                                    {
                                        pk = row.PrimaryKey;
                                        if (pk == null ||
                                            (typeof(string) == collection.Value.TypeOfPrimaryKey &&
                                             string.IsNullOrEmpty((string)pk)) ||
                                            (collection.Value.TypeOfPrimaryKey == typeof(Guid) &&
                                             ((Guid)pk) == Guid.Empty))
                                        {
                                            throw new Exception("No primary key found on item");
                                        }
                                        if (pks.Contains(pk.ToString().ToLower()))
                                        {
                                            throw new Exception("Primary Key '" + pk.ToString() + "' is not unique");
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
    }
}
