using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DedStore.Attributes;

namespace DedStore.Tests.DummyTypes.KeysAndAllThat
{
    public class Office
    {
        [DedStorePrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }


    }
}
