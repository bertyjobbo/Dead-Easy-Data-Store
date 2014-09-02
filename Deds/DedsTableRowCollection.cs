using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTableRowCollection
    {
        public DedsTableRowCollection() { List=new List<DedsTableRow>();}
        public Type TypeOfT { get; set; }
        public Type TypeOfPrimaryKey { get; set; }
        public PropertyInfo PrimaryKeyPropertyInfo { get; set; }
        public IList<DedsTableRow> List { get; set; }
    }

    public class DedsTableRowCollection<T> : DedsTableRowCollection
    {
        private IList<DedsTableRow<T>> _list;

        public DedsTableRowCollection()
        {
            _list = new List<DedsTableRow<T>>();
            base.List = (IList<DedsTableRow>)List;
        }
        public new IList<DedsTableRow<T>> List {
            get
            {
                return _list; 
            }
            set
            {
                base.List = (IList<DedsTableRow>) value;
                _list = value;
            }
        }
        
    }
}
