using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore.Tests.DummyTypes
{
    public class Person
    {
        [DedStorePrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
