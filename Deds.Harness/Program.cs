using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Deds.Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var deds = new DedsConnection())
            {
                var table = deds.Table<MyCustomType>();
                foreach (var myCustomType in table)
                {
                    Console.WriteLine(myCustomType.Id);
                }
                
                // create new
                var newOne = new MyCustomType
                {
                    Date = DateTime.Now, Name = "Test add"
                };

                table.Create(newOne);
                deds.Commit();
                table = deds.Table<MyCustomType>();
                var found = table.Find(table.Last().Id);
                Console.WriteLine("Just added {0}", found.Date.ToString("F"));



                //for (var i = 0; i < 10; i++)
                //{
                //    deds.Table<MyCustomType>().Create(new MyCustomType { Name = "Rob" + i, Date = DateTime.Now });
                //}

                //var res = deds.SaveChanges();
                //if (!res.Success)
                //{
                //    Console.Write(res.ErrorMessage);
                //}
                //else
                //{
                //    foreach (var result in deds.Table<MyCustomType>())
                //    {
                //        Console.WriteLine("{0} - {1}", result.Id, result.Name);
                //    }
                //}
            }

            Console.ReadKey();
        }
    }
}
