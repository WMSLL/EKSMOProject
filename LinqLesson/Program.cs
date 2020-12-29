using System;
using System.Linq;

namespace LinqLesson
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] FootbolClub = { "Бавария","Манчестер сити","Ливерпуль","Реал Мадрит","ПСЖ","Барселона"};

            var itemFC = from t in FootbolClub
                         where t.ToUpper().StartsWith("Б")
                         orderby t
                         select t;
            foreach (var item in itemFC)
            {
                Console.WriteLine(item);
            }
        }
    }
}
