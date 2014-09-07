using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DedStore.Attributes;

namespace DedStore.Tests.DummyTypes.KeysAndAllThat
{
    public class Achievement
    {
        [DedStorePrimaryKey]
        public int Id { get; set; }

        public string Detail { get; set; }

        
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
