using System;
using System.IO;
using System.Linq;
using DedStore.Tests.DummyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DedStore.Tests
{
    [TestClass]
    public class Test_Async_Integration
    {
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
        public void Test_Async_GetAndCommit()
        {
            teardown();
            using (var ctx = new DedStoreContext())
            {
                var table = ctx.GetTableAsync<Person>();
                table.Wait();
                table.Result.Add(new Person {Name = "Shithead"});
                var result = ctx.CommitAsync();
                result.Wait();
                Assert.IsTrue(result.Result.Success);
                Assert.IsTrue(table.Result.Count() == 1);
                Assert.IsTrue(table.Result.First().Name == "Shithead");

            }
        }
    }
}
