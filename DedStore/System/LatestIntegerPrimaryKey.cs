using System;
using DedStore.Attributes;

namespace DedStore.System
{
    public class LatestIntegerPrimaryKey
    {
        [DedStorePrimaryKey]
        public string Id { get; set; }
        public int LatestValue { get; set; }
    }
}
