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
                for (var i = 0; i < 10; i++)
                {
                    deds.Table<MyCustomType>().Create(new MyCustomType {Name = "Rob" + i, Date = DateTime.Now});
                }

                foreach (var result in deds.Table<MyCustomType>()) 
                {
                    Console.WriteLine("{0} - {1}", result.Id, result.Name);
                }
            }

            Console.ReadKey();
        }
    }
}
