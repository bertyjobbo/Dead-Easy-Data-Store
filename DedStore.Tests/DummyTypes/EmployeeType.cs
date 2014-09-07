

using DedStore.Attributes;

namespace DedStore.Tests.DummyTypes
{
    public class EmployeeType
    {
        [DedStorePrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
