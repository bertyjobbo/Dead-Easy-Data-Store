using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DedStore.Attributes;
using Newtonsoft.Json;

namespace DedStore.Tests.DummyTypes.KeysAndAllThat
{
    /// <summary>
    /// Employee
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Id
        /// </summary>
        [DedStorePrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Achievments
        /// </summary>
        [DedStoreOneToMany]
        [JsonIgnore]
        public IEnumerable<Achievement> Achievements { get; set; }

        /// <summary>
        /// Clients
        /// </summary>
        [DedStoreManyToManyAttribute]
        [JsonIgnore]
        public IEnumerable<Office> Offices { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        [DedStoreOneToOne]
        [JsonIgnore]
        public Department Department { get; set; }

        /// <summary>
        /// Job title id
        /// </summary>
        public int DepartmentId { get; set; }


    }
}
