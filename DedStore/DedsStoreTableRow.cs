using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    public class DedsStoreTableRow
    {
        public object RawItem { get; set; }
    }

    public class DedsStoreTableRow<T>: DedsStoreTableRow
    {
        public new T RawItem { get; set; }
    }
}
