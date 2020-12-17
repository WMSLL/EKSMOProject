using System.Data;
using System;
using System.IO;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;

namespace ImporDateFromExceltoDB
{
    class Program
    {
      static  string connectionString = "Data Source =172.27.1.26; Initial Catalog = ILS; User ID =manh; Password =1q2w#E$R";
        static string fileName = @"Коледино.xlsx";
        static string folder = @"C:\Проэкты MS VS\";
        static string newFolder= @"C:\Проэкты MS VS\OLD_wildberis\";
        static void Main(string[] args)
        {
           
            Timer t = new Timer(TimerCallback, null, 0, 10000);

            Console.ReadLine();
            

            static void TimerCallback(Object o)
            {
                SqlConnection sqlConnect = new SqlConnection(connectionString);
                sqlConnect.Open();
                var dataRage = ReadExel();

               
              //  File.Delete(folder + fileName);
                foreach (DataRow dr in dataRage.Rows)
                {
                    // Console.WriteLine($"values({dr[0]},{dr[1]}");
                    var sqlExpression = $"insert into ils.dbo.TestImportExel (item,code) values('{dr[0]}','{dr[1]}')";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnect);
                    command.ExecuteNonQuery();

                }

                sqlConnect.Close();
                File.Move(folder + fileName, newFolder);
                Console.WriteLine("Таймер тикнул");
                Console.ReadKey();

            }



            static DataTable ReadExel()
            {
                Excel.Application application = new Excel.Application();

                DataTable myTable = new DataTable("MyDataTable");
                if (application == null)
                {
                    Console.WriteLine("EXCEL not installed");
                    return myTable;
                }
                Excel.Workbook excelBook = application.Workbooks.Open(folder+fileName);
                Excel._Worksheet excelSheet = application.Sheets[1];
                Excel.Range excelRange = excelSheet.UsedRange;
                int rows = excelRange.Rows.Count;
                int cols = excelRange.Columns.Count;
                myTable.Columns.Add("FirstName", typeof(string));
                myTable.Columns.Add("LastName", typeof(string));
                for (int i = 2; i <= rows; i++)
                {
                    DataRow myNewRow = myTable.NewRow();
                    myNewRow["FirstName"] = excelRange.Cells.Value2[i, 1]; // .ToString(); //string
                    myNewRow["LastName"] = excelRange.Cells.Value2[i, 2];
                    myTable.Rows.Add(myNewRow);
                }

                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("Excel");
                foreach (var p in processes)
                {
                    if (!string.IsNullOrEmpty(p.ProcessName))
                    {
                        try
                        {
                            p.Kill();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }

                return myTable;
            }
        }
    }
}
