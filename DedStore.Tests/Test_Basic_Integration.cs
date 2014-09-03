using System;
using System.Collections.Generic;
using System.IO;
using DedStore.Tests.DummyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DedStore.Tests
{
    [TestClass]
    public class Test_Basic_Integration
    {
        [TestMethod]
        public void SimpleAddAndCommit()
        {
            var persons = new List<Person>
            {
                new Person{Id=1, Name = "Person A"},
                new Person{Id=2, Name = "Person B"},
                new Person{Id=3, Name = "Person C"}
            };
            var json = JsonConvert.SerializeObject(persons);
            const string filePath = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.Person.deds";
            File.WriteAllText(filePath, json);

            var person = new Person { Name = "Rob--" + Guid.NewGuid() };

            var ctx = new DedStoreContext();

            var table = ctx.GetTable<Person>();

            var result = table.Add(person);

            var result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
            Assert.IsTrue(person.Id > 0);
        }
    }
}
