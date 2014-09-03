using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    public class DedStoreTableRowCollection
    {
        public IEnumerable<DedsStoreTableRow> TableRows  { get; set; }

        public DedStoreTable Table { get; set; }
    }

    public class DedStoreTableRowCollection<T> : DedStoreTableRowCollection
    {
        public new IEnumerable<DedsStoreTableRow<T>> TableRows { get; set; }

        public new DedStoreTable<T> Table { get; set; }
    }
}
