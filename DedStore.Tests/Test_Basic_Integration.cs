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
        // fields
        private Guid _guid1 = new Guid("5e49a264-ff10-4c39-9678-c39d5069a2b3");
        private Guid _guid2 = new Guid("13bc2d52-fdc2-4e56-962b-3bdd01ef7711");
        private Guid _guid3 = new Guid("919136d3-1bcd-4d30-93f8-264345557fea");
        private Guid _guid4 = new Guid("3c3e0990-6957-4ed7-b5f3-15dc6e3f5b08");

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
            const string filePath2 = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\INTPK_DedStore.Tests.DummyTypes.Person.deds";
            File.WriteAllText(filePath, json);
            File.WriteAllText(filePath2, 33.ToString());

            var person = new Person { Name = "Rob--" + Guid.NewGuid() };

            var ctx = new DedStoreContext();

            var table = ctx.GetTable<Person>();

            var result = table.Add(person);

            var result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
            Assert.IsTrue(person.Id == 34);
            Assert.IsTrue(File.ReadAllText(filePath2)=="34");

            person = new Person { Name = "Rob--" + Guid.NewGuid() };

            ctx = new DedStoreContext();

            table = ctx.GetTable<Person>();

            result = table.Add(person);

            result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
            Assert.IsTrue(person.Id == 35);
            Assert.IsTrue(File.ReadAllText(filePath2) == "35");
        }

        [TestMethod]
        public void SimpleAddAndCommit_GuidKey()
        {
            var pages = new List<IntranetPage>
            {
                new IntranetPage{Id=_guid1, Name = "Page A"},
                new IntranetPage{Id=_guid2, Name = "Page B"},
                new IntranetPage{Id=_guid3, Name = "Page C"}
            };
            var json = JsonConvert.SerializeObject(pages);
            const string filePath = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
            File.WriteAllText(filePath, json);

            var page = new IntranetPage { Name = "Rob--" + Guid.NewGuid(), Id = Guid.NewGuid() };

            var ctx = new DedStoreContext();

            var table = ctx.GetTable<IntranetPage>();

            var result = table.Add(page);

            var result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
        }

        [TestMethod]
        public void SimpleAddAndCommit_Fail_NonUniqueGuidKey()
        {
            
            var pages = new List<IntranetPage>
            {
                new IntranetPage{Id=_guid1, Name = "Page A"},
                new IntranetPage{Id=_guid2, Name = "Page B"},
                new IntranetPage{Id=_guid3, Name = "Page C"}
            };
            var json = JsonConvert.SerializeObject(pages);
            const string filePath = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
            File.WriteAllText(filePath, json);

            var page = new IntranetPage { Name = "Rob--" + Guid.NewGuid(), Id = _guid3 };

            var ctx = new DedStoreContext();

            var table = ctx.GetTable<IntranetPage>();

            var result = table.Add(page);

            var result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result2.Success);
            Assert.IsTrue(result2.ErrorMessage.Contains("unique"));
        }

        [TestMethod]
        public void SimpleAddAndCommit_StringKey()
        {
            var emps = new List<EmployeeType>
            {
                new EmployeeType{Id="Type1", Name = "Page A"},
                new EmployeeType{Id="Type2", Name = "Page B"},
                new EmployeeType{Id="Type3", Name = "Page C"}
            };
            var json = JsonConvert.SerializeObject(emps);
            const string filePath = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
            File.WriteAllText(filePath, json);

            var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };
            var ctx = new DedStoreContext();
            var table = ctx.GetTable<EmployeeType>();
            var result = table.Add(emp);
            var result2 = ctx.Commit();
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
        }

        [TestMethod]
        public void SimpleAddAndCommit_Fail_NonUniqueStringKey()
        {

            var emps = new List<EmployeeType>
            {
                new EmployeeType{Id="Type1", Name = "Page A"},
                new EmployeeType{Id="Type2", Name = "Page B"},
                new EmployeeType{Id="Type3", Name = "Page C"}
            };
            var json = JsonConvert.SerializeObject(emps);
            const string filePath = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
            File.WriteAllText(filePath, json);

            var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type2" };
            var ctx = new DedStoreContext();
            var table = ctx.GetTable<EmployeeType>();
            var result = table.Add(emp);
            var result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result2.Success);;
            Assert.IsTrue(result2.ErrorMessage.Contains("unique"));
        }

        [TestMethod]
        public void Test_Only_Int_String_AndGuid_AreAllowed()
        {
            var rubbishes = new List<RubbishType>
            {
                new RubbishType{Id=DateTime.Now, Name = "Page A"},
                new RubbishType{Id=DateTime.Now, Name = "Page B"},
                new RubbishType{Id=DateTime.Now, Name = "Page C"}
            };
            var json = JsonConvert.SerializeObject(rubbishes);
            const string filePath = @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.RubbishType.deds";
            File.WriteAllText(filePath, json);

            var page = new RubbishType { Name = "Rob--" + Guid.NewGuid(), Id = DateTime.Now };

            var ctx = new DedStoreContext();

            var table = ctx.GetTable<RubbishType>();

            var result = table.Add(page);

            var result2 = ctx.Commit();

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result2.Success);
            Assert.IsTrue(result2.ErrorMessage.ToLower().Contains("integer"));
            Assert.IsFalse(File.Exists(filePath));
        }


    }
}
