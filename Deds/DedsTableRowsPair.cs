using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTableRowsPair
    {

    }

    public class DedsTableRowsPair<T> : DedsTableRowsPair
    {
        public DedsTable<T> Table { get; set; }
        public DedsTableRowCollection<T> Rows { get; set; }
    }
}
