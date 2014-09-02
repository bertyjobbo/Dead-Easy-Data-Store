using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Deds
{
    public class DedsTableRow
    {
        public object PrimaryKeyValue { get; set; }
        public object Value { get; set; }
        public bool Added { get; set; }
    }

    public class DedsTableRow<T> : DedsTableRow
    {
        private T _value;

        public new T Value
        {
            get { return _value; }
            set
            {
                base.Value = value;
                _value = value;
            }
        }

        
    }
}
