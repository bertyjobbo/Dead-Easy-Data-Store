using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    /// <summary>
    /// Connection
    /// </summary>
    public class DedsConnection : IDisposable
    {
        // fields
        private readonly IDictionary<Type, object> _tables = new Dictionary<Type, object>();

        /// <summary>
        /// Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DedsTable<T> Table<T>()
        {
            var lockObj = new object();
            lock (lockObj)
            {
                var typeOf = typeof (T);
                if (!_tables.ContainsKey(typeOf))
                {
                    _tables.Add(new KeyValuePair<Type, object>(typeOf, readTable<T>()));
                }
                return (DedsTable<T>) _tables[typeOf];
            }
        }

        private DedsTable<T> readTable<T>()
        {
            return new DedsTable<T>();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
