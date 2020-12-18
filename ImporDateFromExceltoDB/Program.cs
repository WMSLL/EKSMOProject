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


        // static string connectionString = "Data Source =172.27.1.25; Initial Catalog = ILS; User ID =manh; Password =OfLpod8d"; 
        static string connectionString = @"Data Source=172.27.1.25\SRVSQL;Initial Catalog=ILS;Persist Security Info=True;User ID=szkoadmin;Password=#t1h2u3$;";
        static string Folder2 = @"\\w-srvfile\wailberis\";
        static string newFolder = @"\\w-srvfile\wailberis\old_wailberis\Коледино";
        static SqlConnection sqlConnect = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            sqlConnect.Open();
            Timer timer = new Timer(TimerCallback, null, 0, 1_800_000);
           
            Console.ReadLine();
            static void TimerCallback(Object o)
            {
                
                Console.WriteLine("Start");
                var dir = new DirectoryInfo(Folder2); // папка с файлами 

                foreach (FileInfo file in dir.GetFiles())
                {
                    if (file.Name!=null)
                    {
                        
                    }
                    //Console.WriteLine();
                    DateTimeOffset date = DateTimeOffset.Now;

                    Console.WriteLine($"считываем EXCEL");
                    var filename = Path.GetFileNameWithoutExtension(file.Name);

                    var dataRage = ReadExel(Folder2 + filename + ".xlsx");


                    foreach (DataRow dr in dataRage.Rows)
                    {
                        // var sqlExpression = $"insert into ils.dbo.TestImportExel (item,code) values('{dr[0]}','{dr[1]}')";
                        var sqlExpression = $@"if Exists(Select *From [EKS_OrderSSCCChildrenWorld] where item='{dr[0]}' and [code]='{dr[1]}' ) begin
                                                                                                           return
                                                                                                           end 
                                                                               else
                                                                               begin
                                                                               insert into [EKS_OrderSSCCChildrenWorld] ([item],	[code] ) values ('{dr[0]}','{dr[1]}')
                                                                               end";
                        SqlCommand command = new SqlCommand(sqlExpression, sqlConnect);
                        command.ExecuteNonQuery();
                    }
                    //Thread.Sleep(5000);
                    Console.WriteLine($"Убиваем процесс EXCEL");
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
                   // Thread.Sleep(3000);
                    Console.WriteLine($"Начинаем перемещать файл");
                    try
                    {
                        File.Move(Folder2 + filename + ".xlsx", newFolder + date.ToString().Replace(":", "_").Replace(" ", "") + ".xlsx");
                        Console.WriteLine(Folder2 + filename + ".xlsx"+" Перемещен");
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        Console.WriteLine($"В папке {Folder2 + filename + ".xlsx"} нет файла");
                    }
                }
                Console.WriteLine("Стоп");
                Console.ReadKey();
            }
            static DataTable ReadExel(string filename)
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
                    Excel.Workbook excelBook = application.Workbooks.Open(filename);
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

                    Console.WriteLine($"В папке {filename} нет файла");
                }
                return myTable;
            }
        }
    }
}
