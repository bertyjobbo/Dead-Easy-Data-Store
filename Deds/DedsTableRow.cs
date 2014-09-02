using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTableRow
    {
        public object PrimaryKeyValue { get; set; }
    }

    public class DedsTableRow<T> : DedsTableRow
    {
        public T Value { get; set; }
    }
}
