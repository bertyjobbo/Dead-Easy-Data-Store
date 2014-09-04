using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DedStore.System;
using Newtonsoft.Json;

namespace DedStore
{
    /// <summary>
    /// Context
    /// </summary>
    public class DedStoreContext : IDisposable
    {

        // fields
        internal readonly IDictionary<Type, DedStoreTableRowCollection> CollectionsInContext = new Dictionary<Type, DedStoreTableRowCollection>();
        private readonly Func<Type, IEnumerable<object>> _getDataMethod;
        private readonly Func<IDictionary<Type, DedStoreTableRowCollection>, DedStoreResponse> _commitMethod;

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

        /// <summary>
        /// Get table
        /// </summary>
        /// <returns></returns>
        public DedStoreTable<T> GetTable<T>()
        {
            var typeOfTable = typeof(T);
            var readCollection = readTextToCollection(typeOfTable);
            if (!CollectionsInContext.ContainsKey(typeOfTable))
            {
                CollectionsInContext.Add(typeOfTable, readCollection);
            }
            else
            {
                CollectionsInContext[typeOfTable] = readCollection;
            }
            var table = new DedStoreTable<T>(this);
            return table;
        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <returns></returns>
        public DedStoreResponse Commit()
        {
            var result = _commitMethod.Invoke(CollectionsInContext);
            return result;
        }

        /// <summary>
        /// Delete type and table etc
        /// </summary>
        /// <param name="type"></param>
        public void DeleteType(Type type)
        {
            using (var ctx = new DedStoreContext())
            {
                var table = ctx.GetTable<RegisteredType>();
                var entry = table.FirstOrDefault(x => x.Id == type.FullName);
                if (entry != null)
                {
                    table.Remove(entry);
                    ctx.Commit();
                    File.Delete(FileHelper.Current.GetTablePath(typeof(RegisteredType)));
                }
            }
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

            File.Delete(FileHelper.Current.GetTablePath(type));
            throw new Exception("Primary key must be Integer, Guid or String");

        }

        /// <summary>
        /// Get pk int
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int getNextIntegerPrimaryKey(Type type)
        {
            using (var ctx = new DedStoreContext())
            {
                var table = ctx.GetTable<LatestIntegerPrimaryKey>();
                var latest = table.FirstOrDefault(x => x.Id == type.FullName);
                if (latest == null)
                {
                    latest = new LatestIntegerPrimaryKey
                    {
                        Id = type.FullName,
                        LatestValue = 1
                    };
                    table.Add(latest);
                    var commitResult1 = ctx.Commit();
                    if (!commitResult1.Success) throw new Exception(commitResult1.ErrorMessage);
                    return 1;
                }

                var output = latest.LatestValue + 1;
                latest.LatestValue = output;
                table.Update(latest);
                var commitResult = ctx.Commit();
                if (!commitResult.Success) throw new Exception(commitResult.ErrorMessage);
                return output;
            }

        }

        /// <summary>
        /// Get data method (standard)
        /// </summary>
        /// <returns></returns>
        private Func<Type, IEnumerable<object>> getDataMethod()
        {
            return (type) =>
            {
                var text = FileHelper.Current.GetText(type);
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
                        var collectionsToCommit = CollectionsInContext.Where(x => x.Value.HasCommits);

                        // loop
                        foreach (var collection in collectionsToCommit.ToList())
                        {
                            // check pk
                            if (!FileHelper.Current.SystemTypes.Contains(collection.Key))
                            {
                                checkType(collection.Value.TypeOfPrimaryKey, collection.Key);
                            }

                            // get table
                            var freshCollection = readTextToCollection(collection.Key);

                            // str
                            var str = new StringBuilder("[");

                            // get pks before add
                            var pks = freshCollection.TableRows.Select(x => x.PrimaryKey.ToString().ToLower()).ToList();

                            //collection.Value.TableRows.Where(x => x.Added).ToList().ForEach(item => freshCollection.TableRows.Add(item));

                            // add
                            foreach (var newItem in collection.Value.ItemsToAdd)
                            {
                                object pk;
                                if (freshCollection.TypeOfPrimaryKey == typeof(int))
                                {
                                    pk = getNextIntegerPrimaryKey(collection.Key);
                                    newItem.SetRawItemPrimaryKey(pk, collection.Value.PrimaryKeyPropertyInfo);
                                }
                                else
                                {
                                    pk = newItem.PrimaryKey;
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

                                // add to collection
                                freshCollection.TableRows.Add(newItem);
                            }

                            // update
                            foreach (var updatedItem in collection.Value.ItemsToUpdate)
                            {
                                // find in collection
                                var foundFromDb =
                                    freshCollection.TableRows.FirstOrDefault(x => x.PrimaryKey.Equals(updatedItem.PrimaryKey));

                                // check
                                if (foundFromDb == null)
                                {
                                    throw new Exception("Cannot find object in collection with primary key value of '" + updatedItem.PrimaryKey + "' when attempting update");
                                }

                                var index = freshCollection.TableRows.IndexOf(foundFromDb);
                                freshCollection.TableRows[index] = updatedItem;
                            }

                            // remove
                            foreach (var removedItem in collection.Value.ItemsToRemove)
                            {
                                // find in collection
                                var foundFromDb =
                                    freshCollection.TableRows.FirstOrDefault(x => x.PrimaryKey.Equals(removedItem.PrimaryKey));

                                // check
                                if (foundFromDb == null)
                                {
                                    throw new Exception("Cannot find object in collection with primary key value of '" + removedItem.PrimaryKey + "' when attempting to remove");
                                }

                                freshCollection.TableRows.Remove(foundFromDb);
                            }



                            // loop collection and add to string
                            var strs = freshCollection.TableRows.Select(row => JsonConvert.SerializeObject(row.RawItem)).ToList();

                            // commit
                            str.Append(string.Join(",", strs));
                            str.Append("]");
                            var finalJson = str.ToString();
                            FileHelper.Current.WriteText(finalJson, collection.Key);

                            // get again
                            CollectionsInContext[collection.Key] = readTextToCollection(collection.Key);
                            
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
            // get custom attrs
            var props =
                type.GetProperties()
                    .Where(x => x.CustomAttributes.Any(p => p.AttributeType == typeof(DedStorePrimaryKeyAttribute))).ToList();

            // check
            if (props.Count > 1 || props.Count < 1) throw new Exception("Type " + type.FullName + " has no primary key or too many primary keys (using 'DedStorePrimaryKeyAttribute')");

            // get
            var pkPropInfo = props.Single();

            // get table
            checkTypeForChanges(type, pkPropInfo);

            // hashtable
            var objects = _getDataMethod(type);

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
        /// Check if type is ok
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private void checkTypeForChanges(Type type, PropertyInfo info)
        {
            if (!FileHelper.Current.SystemTypes.Contains(type))
            {
                using (var ctx = new DedStoreContext())
                {
                    var table = ctx.GetTable<RegisteredType>();
                    var entry = table.FirstOrDefault(x => x.Id == type.FullName);
                    if (entry == null)
                    {
                        table.Add(new RegisteredType { Id = type.FullName, PrimaryKeyPropertyName = info.Name, PrimaryKeyPropertyTypeName = info.PropertyType.FullName });
                        var result = ctx.Commit();
                        if (!result.Success) throw new Exception(result.ErrorMessage);
                        return;
                    }

                    if (entry.Id == type.FullName && entry.PrimaryKeyPropertyName == info.Name && entry.PrimaryKeyPropertyTypeName == info.PropertyType.FullName) return;

                    throw new Exception("Type " + type.FullName + " is expecting a PrimaryKey type of " + info.PropertyType +
                                        " named " + info.Name +
                                        ". If you have changed the composition of your type, you need to call DedStoreContext.DeleteType(type) first.");
                }

            }
        }

        #region dispose

        private IntPtr _nativeResource = Marshal.AllocHGlobal(100);
        //private AnotherResource _managedResource = new AnotherResource();

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~DedStoreContext()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                //if (_managedResource != null)
                //{
                //    _managedResource.Dispose();
                //    _managedResource = null;
                //}
            }
            // free native resources if there are any.
            if (_nativeResource != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_nativeResource);
                _nativeResource = IntPtr.Zero;
            }
        }

        #endregion

#region ASYNC

        /// <summary>
        /// Async commit
        /// </summary>
        /// <returns></returns>
        public async Task<DedStoreResponse> CommitAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Async commit
        /// </summary>
        /// <returns></returns>
        public async Task<DedStoreTable<T>> GetTableAsync<T>()
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
