using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating");
            DateTime dt = Osrs.Reflection.NameReflectionUtils.CreateInstance<DateTime>(new Osrs.Reflection.TypeNameReference("System.Core", "System.DateTime"));
            Console.WriteLine("Created "+dt.ToString());
            Console.ReadLine();
        }
    }
}
