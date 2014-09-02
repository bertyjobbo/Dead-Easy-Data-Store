using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsResponse<T>
    {
        public string ErrorMessage { get; set; }
        public bool Success { get { return !string.IsNullOrEmpty(ErrorMessage); } }
        public T ResponseItemOrNullIfDelete { get; set; }
    }
}
