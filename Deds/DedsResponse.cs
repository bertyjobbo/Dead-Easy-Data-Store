using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsResponse
    {
        public string ErrorMessage { get; set; }
        public bool Success { get { return !string.IsNullOrEmpty(ErrorMessage); } }
        public DedsResponseType ResponseType { get; set; }
    }

    public class DedsResponse<T> : DedsResponse
    {
        public T ResponseItem { get; set; }
    }
}
