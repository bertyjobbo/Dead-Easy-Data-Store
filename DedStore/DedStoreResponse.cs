using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    public class DedStoreResponse
    {
        public string ErrorMessage { get; set; }
        public bool Success { get { return string.IsNullOrEmpty(ErrorMessage); } }
        public DedStoreResponseType ResponseType { get; set; }
    }

    public class DedStoreResponse<T> : DedStoreResponse
    {
        public T ResponseItem { get; set; }
    }
}
