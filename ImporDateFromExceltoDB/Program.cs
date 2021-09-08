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
                ReadExecss readExecss = new ReadExecss();
                Console.WriteLine("Start");
                var dir = new DirectoryInfo(Folder2); // папка с файлами 
                foreach (FileInfo file in dir.GetFiles())
                {
                    DateTimeOffset date = DateTimeOffset.Now;
                    var typeFile = Path.GetExtension(file.FullName);


                    var filename = Path.GetFileNameWithoutExtension(file.Name);
                    if (typeFile == ".xlsx" )
                    {


                        Console.WriteLine($"считываем EXCEL");
                        readExecss.ReadExel(Folder2 + filename + ".xlsx");
                        var dataRage = readExecss.Data;
                        foreach (DataRow dr in dataRage.Rows)
                        {
                            var sqlExpression = $@"if Exists(Select *From [EKS_OrderSSCCChildrenWorld] where item='{dr[0]}' and [code]='{dr[1]}' ) begin
                                                                                                           return
                                                                                                           end 
                                                                               else
                                                                               begin
                                                                               insert into [EKS_OrderSSCCChildrenWorld] ([item],	[code],[OrderId] ) values ('{dr[0]}','{dr[1]}','{dr[2]}')
                                                                               end";
                            SqlCommand command = new SqlCommand(sqlExpression, sqlConnect);
                            command.ExecuteNonQuery();
                        }
                        Console.WriteLine($"kill process EXCEL");
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
                        Console.WriteLine($"start transfer file");
                        try
                        {
                            File.Move(Folder2 + filename + ".xlsx", newFolder + date.ToString().Replace(":", "_").Replace(" ", "") + ".xlsx");
                            Console.WriteLine(Folder2 + filename + ".xlsx" + " Transfer complict");
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            Console.WriteLine($"In folder {Folder2 + filename + ".xlsx"} file not faund");
                        }
                    }
                }
                Console.WriteLine("Stop");
                Console.ReadKey();
            }
        }
    }

}
