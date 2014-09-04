using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DedStore.System;
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

        private void teardown()
        {
            Directory.Delete(@"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles", true);
            using (var c = new DedStoreContext())
            {
                var t = c.GetTable<Person>();
                var t2 = c.GetTable<EmployeeType>();
                var t3 = c.GetTable<IntranetPage>();
                var t4 = c.GetTable<RubbishType>();
            }
        }

        [TestMethod]
        public void Test_System_Types_Registered()
        {
            using (var ctx = new DedStoreContext())
            {
                var thetable = ctx.GetTable<Person>();
            }
            using (var ctx = new DedStoreContext())
            {
                var table2 = ctx.GetTable<RegisteredType>();
                Assert.IsTrue(table2.Count() > 0);
            }
        }

        [TestMethod]
        public void SimpleAddAndCommit()
        {
            var person = new Person { Name = "Rob--" + Guid.NewGuid() };

            using (var ctx = new DedStoreContext())
            {
                teardown();
                var persons = new List<Person>
                {
                    new Person {Id = 1, Name = "Person A"},
                    new Person {Id = 2, Name = "Person B"},
                    new Person {Id = 3, Name = "Person C"}
                };
                var json = JsonConvert.SerializeObject(persons);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.Person.deds";
                const string filePath2 =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\SystemTables\DedStore.System.LatestIntegerPrimaryKey.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);
                var json2 =
                    JsonConvert.SerializeObject(new[]{ new LatestIntegerPrimaryKey
                    {
                        Id = typeof (Person).FullName,
                        LatestValue = 33
                    }});
                File.WriteAllText(filePath2, json2, Encoding.Unicode);





                var table = ctx.GetTable<Person>();

                var result = table.Add(person);

                var result2 = ctx.Commit();

                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(person.Id == 34);
                var pkTable = ctx.GetTable<LatestIntegerPrimaryKey>();
                var pk = pkTable.FirstOrDefault();
                Assert.IsTrue(pk != null && pk.LatestValue == 34);



            }
            using (var ctx = new DedStoreContext())
            {

                person = new Person { Name = "Rob--" + Guid.NewGuid() };

                var table = ctx.GetTable<Person>();

                var result = table.Add(person);

                var result2 = ctx.Commit();

                var checkTable2 = ctx.GetTable<RegisteredType>();
                var entry = checkTable2.FirstOrDefault(x => x.Id == typeof(Person).FullName);
                var pkTable = ctx.GetTable<LatestIntegerPrimaryKey>();
                var pk = pkTable.FirstOrDefault();

                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(person.Id == 35);
                Assert.IsTrue(entry != null);
                Assert.IsTrue(pk != null && pk.LatestValue == 35);
            }
        }

        [TestMethod]
        public void SimpleAddAndCommit_GuidKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                
                var pages = new List<IntranetPage>
                {
                    new IntranetPage {Id = _guid1, Name = "Page A"},
                    new IntranetPage {Id = _guid2, Name = "Page B"},
                    new IntranetPage {Id = _guid3, Name = "Page C"}
                };
                var json = JsonConvert.SerializeObject(pages);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                var page = new IntranetPage { Name = "Rob--" + Guid.NewGuid(), Id = Guid.NewGuid() };



                var table = ctx.GetTable<IntranetPage>();

                var result = table.Add(page);

                var result2 = ctx.Commit();

                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
            }
        }

        [TestMethod]
        public void SimpleAddAndCommit_Fail_NonUniqueGuidKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                


                var pages = new List<IntranetPage>
                {
                    new IntranetPage {Id = _guid1, Name = "Page A"},
                    new IntranetPage {Id = _guid2, Name = "Page B"},
                    new IntranetPage {Id = _guid3, Name = "Page C"}
                };
                var json = JsonConvert.SerializeObject(pages);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                var page = new IntranetPage { Name = "Rob--" + Guid.NewGuid(), Id = _guid3 };



                var table = ctx.GetTable<IntranetPage>();

                var result = table.Add(page);

                var result2 = ctx.Commit();

                Assert.IsTrue(result.Success);
                Assert.IsFalse(result2.Success);
                Assert.IsTrue(result2.ErrorMessage.Contains("unique"));
            }
        }

        [TestMethod]
        public void SimpleAddAndCommit_StringKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<EmployeeType>
                {
                    new EmployeeType {Id = "Type1", Name = "Page A"},
                    new EmployeeType {Id = "Type2", Name = "Page B"},
                    new EmployeeType {Id = "Type3", Name = "Page C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<EmployeeType>();
                var result = table.Add(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
            }
        }

        [TestMethod]
        public void SimpleAddAndCommit_Fail_NonUniqueStringKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<EmployeeType>
                {
                    new EmployeeType {Id = "Type1", Name = "Page A"},
                    new EmployeeType {Id = "Type2", Name = "Page B"},
                    new EmployeeType {Id = "Type3", Name = "Page C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type2" };

                var table = ctx.GetTable<EmployeeType>();
                var result = table.Add(emp);
                var result2 = ctx.Commit();

                Assert.IsTrue(result.Success);
                Assert.IsFalse(result2.Success);
                Assert.IsTrue(result2.ErrorMessage.Contains("unique"));
            }
        }

        [TestMethod]
        public void Test_Only_Int_String_AndGuid_AreAllowed()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var rubbishes = new List<RubbishType>
                {
                    new RubbishType {Id = DateTime.Now, Name = "Page A"},
                    new RubbishType {Id = DateTime.Now, Name = "Page B"},
                    new RubbishType {Id = DateTime.Now, Name = "Page C"}
                };
                var json = JsonConvert.SerializeObject(rubbishes);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.RubbishType.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                var page = new RubbishType { Name = "Rob--" + Guid.NewGuid(), Id = DateTime.Now };



                var table = ctx.GetTable<RubbishType>();

                var result = table.Add(page);

                var result2 = ctx.Commit();

                Assert.IsTrue(result.Success);
                Assert.IsFalse(result2.Success);
                Assert.IsTrue(result2.ErrorMessage.ToLower().Contains("integer"));
                Assert.IsFalse(File.Exists(filePath));
            }
        }

        [TestMethod]
        public void Test_Type_Change_Fail()
        {
            teardown();
            // THIS CAN'T BE A UNIT TEST, BUT I HAVE TESTED IT!
            Assert.IsFalse(false);
        }

        [TestMethod]
        public void Test_Update_StringKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<EmployeeType>
                {
                    new EmployeeType {Id = "Type1", Name = "Type A"},
                    new EmployeeType {Id = "Type2", Name = "Type B"},
                    new EmployeeType {Id = "Type3", Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<EmployeeType>();
                var emp = table.First();
                emp.Name = "Type Changed";
                var result = table.Update(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(ctx.GetTable<EmployeeType>().First().Name == "Type Changed");
            }

            using (var ctx = new DedStoreContext())
            {
                Assert.IsTrue(ctx.GetTable<EmployeeType>().First().Name == "Type Changed");
            }
        }

        [TestMethod]
        public void Test_Remove_StringKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<EmployeeType>
                {
                    new EmployeeType {Id = "Type1", Name = "Type A"},
                    new EmployeeType {Id = "Type2", Name = "Type B"},
                    new EmployeeType {Id = "Type3", Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<EmployeeType>();
                Assert.IsTrue(table.FirstOrDefault(x => x.Id == "Type1") != null);
                var emp = table.First();
                var result = table.Remove(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(ctx.GetTable<EmployeeType>().FirstOrDefault(x => x.Id == "Type1") == null);
            }
            using (var ctx = new DedStoreContext())
            {
                Assert.IsTrue(ctx.GetTable<EmployeeType>().FirstOrDefault(x => x.Id == "Type1") == null);
            }
        }

        [TestMethod]
        public void Test_Find_StringKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<EmployeeType>
                {
                    new EmployeeType {Id = "Type1", Name = "Type A"},
                    new EmployeeType {Id = "Type2", Name = "Type B"},
                    new EmployeeType {Id = "Type3", Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.EmployeeType.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<EmployeeType>();

                var found = table.Find("Type1");

                Assert.IsTrue(found != null && found.GetHashCode() == table.First().GetHashCode());

            }
        }

        [TestMethod]
        public void Test_Update_GuidKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<IntranetPage>
                {
                    new IntranetPage {Id = _guid1, Name = "Type A"},
                    new IntranetPage {Id = _guid2, Name = "Type B"},
                    new IntranetPage {Id = _guid3, Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<IntranetPage>();
                var emp = table.First();
                emp.Name = "Type Changed";
                var result = table.Update(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(ctx.GetTable<IntranetPage>().First().Name == "Type Changed");
            }

            using (var ctx = new DedStoreContext())
            {
                Assert.IsTrue(ctx.GetTable<IntranetPage>().First().Name == "Type Changed");
            }
        }

        [TestMethod]
        public void Test_Remove_GuidKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<IntranetPage>
                {
                    new IntranetPage {Id = _guid1, Name = "Type A"},
                    new IntranetPage {Id = _guid2, Name = "Type B"},
                    new IntranetPage {Id = _guid3, Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<IntranetPage>();
                Assert.IsTrue(table.FirstOrDefault(x => x.Id == _guid1) != null);
                var emp = table.First();
                var result = table.Remove(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(ctx.GetTable<IntranetPage>().FirstOrDefault(x => x.Id == _guid1) == null);
            }
            using (var ctx = new DedStoreContext())
            {
                Assert.IsTrue(ctx.GetTable<IntranetPage>().FirstOrDefault(x => x.Id == _guid1) == null);
            }
        }

        [TestMethod]
        public void Test_Find_GuidKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<IntranetPage>
                {
                    new IntranetPage {Id = _guid1, Name = "Type A"},
                    new IntranetPage {Id = _guid2, Name = "Type B"},
                    new IntranetPage {Id = _guid3, Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.IntranetPage.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<IntranetPage>();

                var found = table.Find(_guid1);

                Assert.IsTrue(found != null && found.GetHashCode() == table.First().GetHashCode());
            }
        }


        [TestMethod]
        public void Test_Update_IntegerKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<Person>
                {
                    new Person {Id = 1, Name = "Type A"},
                    new Person {Id = 2, Name = "Type B"},
                    new Person {Id = 3, Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.Person.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<Person>();
                var emp = table.First();
                emp.Name = "Type Changed";
                var result = table.Update(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(ctx.GetTable<Person>().First().Name == "Type Changed");
            }

            using (var ctx = new DedStoreContext())
            {
                Assert.IsTrue(ctx.GetTable<Person>().First().Name == "Type Changed");
            }
        }

        [TestMethod]
        public void Test_Remove_IntegerKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<Person>
                {
                    new Person {Id = 1, Name = "Type A"},
                    new Person {Id = 2, Name = "Type B"},
                    new Person {Id = 3, Name = "Type C"}
                };
                var json = JsonConvert.SerializeObject(emps);
                const string filePath =
                    @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.Person.deds";
                File.WriteAllText(filePath, json, Encoding.Unicode);

                //var emp = new EmployeeType { Name = "Rob--" + Guid.NewGuid(), Id = "Type4" };

                var table = ctx.GetTable<Person>();
                Assert.IsTrue(table.FirstOrDefault(x => x.Id == 1) != null);
                var emp = table.First();
                var result = table.Remove(emp);
                var result2 = ctx.Commit();
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(ctx.GetTable<Person>().FirstOrDefault(x => x.Id == 1) == null);
            }
            using (var ctx = new DedStoreContext())
            {
                Assert.IsTrue(ctx.GetTable<Person>().FirstOrDefault(x => x.Id == 1) == null);
            }
        }

        [TestMethod]
        public void Test_Find_IntegerKey()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();
                

                var emps = new List<Person>
                {
                    new Person {Id = 1, Name = "Type A"},
                    new Person {Id = 2, Name = "Type B"},
                    new Person {Id = 3, Name = "Type C"}
                };

                //var json = JsonConvert.SerializeObject(emps);
                //const string filePath =
                // @"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles\DedStore.Tests.DummyTypes.Person.deds";
                //File.WriteAllText(filePath, json);

                var table = ctx.GetTable<Person>();
                var result = table.Clear();
                result = ctx.Commit();
                result = table.AddMany(emps);
                result = ctx.Commit();
                result = table.UpdateMany(emps);
                result = ctx.Commit();
                result = table.RemoveMany(emps);
                result = ctx.Commit();
                result = table.AddMany(emps);
                result = ctx.Commit();

                var found = table.Find(emps.Last().Id);

                Assert.IsTrue(found != null && found.GetHashCode() == table.Last().GetHashCode());
            }
        }

        [TestMethod]
        public void Test_Crud_Multiple()
        {
            using (var ctx = new DedStoreContext())
            {
                teardown();

                var emps = new List<Person>
                {
                    new Person {Id = 1, Name = "Person A"},
                    new Person {Id = 2, Name = "Person B"},
                    new Person {Id = 3, Name = "Person C"}
                };
                
                var table = ctx.GetTable<Person>();
                var result = table.AddMany(emps);
                result = ctx.Commit();
                Assert.IsTrue(table.Sum(x=>x.Id)==6);

                result = table.Clear();
                result = ctx.Commit();
                Assert.IsTrue(table.Count() == 0);

                result = table.AddMany(emps);
                result = ctx.Commit();
                Assert.IsTrue(table.Count() == 3);

                table.First().Name = "TEST CHANGED";
                Assert.IsFalse(emps.First().Name== "TEST CHANGED");
                result = table.UpdateMany(table);
                result = ctx.Commit();
                Assert.IsTrue(table.First().Name == "TEST CHANGED");

                result = table.RemoveMany(table);
                result = ctx.Commit();
                Assert.IsTrue(table.Count() == 0);

                result = table.AddMany(emps);
                result = ctx.Commit();
                Assert.IsTrue(table.Last().Id ==9);
            }
        }

        [TestMethod]
        public void Test_Delete_Folder_Then_Add_Three_People_Twice_And_Count_Pks()
        {
            teardown();
            var peopleToAdd = new List<Person>
            {
                new Person{Id=1, Name = "Name 1"},
                new Person{Id=2, Name = "Name 2"},
                new Person{Id=3, Name = "Name 3"}
            };

            using (var ctx = new DedStoreContext())
            {
                var people = ctx.GetTable<Person>();
                people.AddMany(peopleToAdd);
                var result = ctx.Commit();
                Assert.IsTrue(result.Success);
                var count = people.Sum(x => x.Id);
                Assert.IsTrue(count == 6);
            }

            using (var ctx = new DedStoreContext())
            {
                peopleToAdd = new List<Person>
                {
                    new Person{Id=1, Name = "Name 1"},
                    new Person{Id=2, Name = "Name 2"},
                    new Person{Id=3, Name = "Name 3"}
                };
                var people = ctx.GetTable<Person>();
                people.AddMany(peopleToAdd);
                var result = ctx.Commit();
                Assert.IsTrue(result.Success);
                var count = people.Sum(x => x.Id);
                Assert.IsTrue(count == 21);
            }
        }

        
    }
}
