using System;
using System.IO;
using System.Linq;
using DedStore.Tests.DummyTypes;
using DedStore.Tests.DummyTypes.KeysAndAllThat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DedStore.Tests
{
    [TestClass]
    public class Test_Keys_And_All_That
    {
        private void teardown()
        {
            Directory.Delete(@"C:\Projects\Git\Deds\DedStore.Tests\DedStoreFiles", true);
            using (var c = new DedStoreContext())
            {

            }
        }

        [TestMethod]
        public void Test_EmployeeHasDepartment()
        {
            teardown();

            using (var ctx = new DedStoreContext())
            {
                var depts = ctx.GetTable<Department>();
                depts.AddMany(new[] { new Department { Name = "Dept1" }, new Department { Name = "Dept2" } });
                ctx.Commit();

                var employees = ctx.GetTable<Employee>();

                employees.Add(new Employee
                {
                    Name = "Rob J",
                    DepartmentId = depts.First().Id
                });

                ctx.Commit();

                employees = ctx.GetTable<Employee>();
                var emp = employees.First();

                Assert.IsTrue(emp.Department != null, "Dept not null");
                Assert.IsTrue(emp.Department.Id == 1, "Dept id = 1");
                Assert.IsTrue(emp.Department.Name == "Dept1", "Dept name = 'dept1'");
            }
        }
    }
}
