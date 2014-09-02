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
        private readonly IDedsConnectionContext _ctx;

        /// <summary>
        /// Constructor for testing
        /// </summary>
        /// <param name="ctx"></param>
        public DedsConnection(IDedsConnectionContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DedsConnection()
        {
            _ctx = DedsConnectionContext.Current;
        }

        /// <summary>
        /// Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DedsTable<T> Table<T>()
        {
            var pair = _ctx.GetTableRowsPair<T>();
            return pair.Table;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public DedsResponse SaveChanges()
        {
            throw new NotImplementedException();
        } 

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
