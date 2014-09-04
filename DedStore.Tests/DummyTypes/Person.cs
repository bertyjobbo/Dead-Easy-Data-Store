

namespace DedStore.Tests.DummyTypes
{
    public class Person
    {
        [DedStorePrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
