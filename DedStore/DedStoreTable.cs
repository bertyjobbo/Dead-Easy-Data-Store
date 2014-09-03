using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    public class DedStoreTable : IEnumerable
    {
        internal DedStoreTableRowCollection InnerCollection { get; set; }
        public IEnumerator GetEnumerator()
        {
            return InnerCollection.TableRows.Select(x=>x.RawItem).GetEnumerator();
        }
    }

    public class DedStoreTable<T> : IEnumerable<T>
    {
        internal DedStoreTableRowCollection<T> InnerCollection { get; set; }


        public IEnumerator<T> GetEnumerator()
        {
            return InnerCollection.TableRows.Select(x => x.RawItem).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
