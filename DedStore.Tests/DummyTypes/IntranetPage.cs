using System;
using DedStore.Attributes;


namespace DedStore.Tests.DummyTypes
{
    public class IntranetPage
    {
        [DedStorePrimaryKey]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
