using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTableRowsPair
    {
        public DedsTable Table { get; set; }
        public DedsTableRowCollection Rows { get; set; }
    }

    public class DedsTableRowsPair<T> : DedsTableRowsPair
    {
        private DedsTable<T> _table ;
        private DedsTableRowCollection<T> _rows;

        public new DedsTable<T> Table
        {
            get { return _table; }
            set
            {
                base.Table = value;
                _table = value;
            }
        }

        public new DedsTableRowCollection<T> Rows {
            get { return _rows; }
            set
            {
                base.Rows = value;
                _rows = value;
            }
        }
    }
}
