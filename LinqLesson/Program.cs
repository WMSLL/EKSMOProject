using System;
using System.Linq;

namespace LinqLesson
{
    class Program
    {
       static string[] names = { "Tom", "Dick", "Mary", "Henry" };
        static void Main(string[] args)
        {
            foreach (var name in names)
            {
                if (name.ToUpper().Contains("R"))
                {
                    Console.WriteLine(name.ToUpper());
                }
            }

            var query = names.Where(x => x.Contains("r")).Select(x => x.ToUpper());
            foreach (var item in query)
            {
                Console.WriteLine(item);
            }

        }                
    }
}
