using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedStore.Tests.DummyTypes
{
    public class RubbishType
    {
        [DedStorePrimaryKey]
        public DateTime Id { get; set; }
        public string Name { get; set; }
    }
}
