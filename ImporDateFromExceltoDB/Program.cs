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
        static string connectionString = "Data Source =172.27.1.26; Initial Catalog = ILS; User ID =manh; Password =1q2w#E$R";
        static string fileName = @"C:\Проэкты MS VS\Коледино.xlsx";
        static string newFolder = @"C:\Проэкты MS VS\OLD_wildberis\Коледино";
        static SqlConnection sqlConnect = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            sqlConnect.Open();
            Timer t = new Timer(TimerCallback, null, 0, 20000);
            Console.ReadLine();
            static void TimerCallback(Object o)
            {
                DateTimeOffset date = DateTimeOffset.Now;
                var dataRage = ReadExel();
                foreach (DataRow dr in dataRage.Rows)
                {
                    var sqlExpression = $"insert into ils.dbo.TestImportExel (item,code) values('{dr[0]}','{dr[1]}')";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnect);
                    command.ExecuteNonQuery();
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
                try
                {
                    File.Move(fileName, newFolder + date.ToString().Replace(":", "_").Replace(" ", "") + ".xlsx");
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine($"В папке {fileName} нет файла");
                }
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
                try
                {
                    Excel.Workbook excelBook = application.Workbooks.Open(fileName);
                    Excel._Worksheet excelSheet = application.Sheets[1];
                    Excel.Range excelRange = excelSheet.UsedRange;
                    int rows = excelRange.Rows.Count;
                    int cols = excelRange.Columns.Count;
                    myTable.Columns.Add("FirstName", typeof(string));
                    myTable.Columns.Add("LastName", typeof(string));
                    for (int i = 2; i <= rows; i++)
                    {
                        DataRow myNewRow = myTable.NewRow();
                        myNewRow["FirstName"] = excelRange.Cells.Value2[i, 1].ToString(); // .ToString(); //string
                        myNewRow["LastName"] = excelRange.Cells.Value2[i, 2].ToString();
                        myTable.Rows.Add(myNewRow);
                    }
                    if (excelBook != null)
                    {
                        excelBook.Close(false, Type.Missing, Type.Missing);
                        application.Workbooks.Close();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelBook);
                        application.Quit();
                        GC.Collect();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                    }
                }
                catch (System.Runtime.InteropServices.COMException)
                {

                    Console.WriteLine($"В папке {fileName} нет файла");
                }
                return myTable;
            }
        }
    }
}
