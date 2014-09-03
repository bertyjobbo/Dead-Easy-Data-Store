using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    public static class ExtensionMethods
    {
        public static DedStoreTableRowCollection<T> ToTyped<T>(this DedStoreTableRowCollection collection)
        {
            return new DedStoreTableRowCollection<T>();
        }
    }
}
