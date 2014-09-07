using System;
using DedStore.Attributes;

namespace DedStore.Tests.DummyTypes
{
    public class RubbishType
    {
        [DedStorePrimaryKey]
        public DateTime Id { get; set; }
        public string Name { get; set; }
    }
}
