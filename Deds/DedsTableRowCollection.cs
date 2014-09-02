using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTableRowCollection { }

    public class DedsTableRowCollection<T>
    {
        public DedsTableRowCollection()
        {
            List = new List<DedsTableRow<T>>();
        }

        public Type TypeOfT { get; set; }
        public Type TypeOfPrimaryKey { get; set; }
        public PropertyInfo PrimaryKeyPropertyInfo { get; set; }
        public IList<DedsTableRow<T>> List { get; private set; }
    }
}
