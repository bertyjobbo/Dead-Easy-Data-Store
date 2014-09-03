using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DedStore
{
    /// <summary>
    /// Context
    /// </summary>
    public class DedStoreContext
    {
        public DedStoreContext()
        {
            _getDataMethod = (type) =>
            {
                var text = FileHelper.GetText(type);
                if(string.IsNullOrWhiteSpace(text)) return new Hashtable();

                var data = JsonConvert.DeserializeObject(text, type);
                return (Hashtable)data;
            };
        }

        public DedStoreContext(Func<Type, Hashtable> getDataFunc)
        {
            _getDataMethod = getDataFunc;
        }

        // fields
        private readonly IDictionary<Type, DedStoreTableRowCollection> _collectionsInContext = new Dictionary<Type, DedStoreTableRowCollection>();
        private readonly Func<Type, Hashtable> _getDataMethod;

        /// <summary>
        /// Get table
        /// </summary>
        /// <returns></returns>
        public DedStoreTable<T> GetTable<T>()
        {
            var type = typeof (T);
            if (!_collectionsInContext.ContainsKey(type))
            {
                _collectionsInContext.Add(type, readTextToCollection(type));
            }
            return _collectionsInContext[type].ToTyped<T>().Table;

        }

        /// <summary>
        /// Read json to collection
        /// </summary>
        /// <returns></returns>
        private DedStoreTableRowCollection readTextToCollection(Type type)
        {
            
        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <returns></returns>
        public DedStoreResponse Commit()
        {
            
        }
    }
}
