
namespace DedStore
{
    public class DedStoreResponse
    {
        public string ErrorMessage { get; set; }
        public bool Success { get { return string.IsNullOrEmpty(ErrorMessage); } }
        public DedStoreResponseType ResponseType { get; set; }
        public override string ToString()
        {
            return (Success ? "Successful" : "Failed: " + ErrorMessage);
        }
    }

    public class DedStoreResponse<T> : DedStoreResponse
    {
        public T ResponseItem { get; set; }
    }
}
