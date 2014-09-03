using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    /// <summary>
    /// Ded store table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DedStoreTable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DedStoreTable(DedStoreTableRowCollection collection)
        {
            InnerCollection = collection;
        }

        /// <summary>
        /// Inner collection
        /// </summary>
        internal DedStoreTableRowCollection InnerCollection { get; private set; }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return InnerCollection.TableRows.Select(x => (T) x.RawItem).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public DedStoreResponse<T> Add(T item)
        {
            InnerCollection.Add(item);
            return new DedStoreResponse<T> { ResponseItem = item, ResponseType = DedStoreResponseType.Add };
        }
    }
}
