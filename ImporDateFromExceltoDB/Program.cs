﻿using System.Data;
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
        static string newFolder = @"\\w-srvfile\wailberis\old_wailberis\";
        static SqlConnection sqlConnect = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            sqlConnect.Open();
            Timer timer = new Timer(TimerCallback, null, 0, 300_000);
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
                    if (typeFile == ".xlsx" || typeFile == ".xls")
                    {
                        Console.WriteLine($"Read EXCEL");
                        readExecss.ReadExel(Folder2 + filename + typeFile);
                        var dataRage = readExecss.Data;
                        foreach (DataRow dr in dataRage.Rows)
                        {
                            var t0 = dr[0].ToString();
                            var t1 = dr[1].ToString();
                            var t2 = dr[2].ToString();
                            var sqlExpression = $@"if Exists(Select *From [EKS_OrderSSCCChildrenWorld] where item=LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE('{t0}', CHAR(10), ''), CHAR(13), ''), CHAR(9), ''), CHAR(160), ''))) and [code]=LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE('{t1}', CHAR(10), ''), CHAR(13), ''), CHAR(9), ''), CHAR(160), ''))) ) begin
                                                                                                           return
                                                                                                           end 
                                                                               else
                                                                               begin
                                                                               insert into [EKS_OrderSSCCChildrenWorld] ([item],	[code],[OrderId] ) values (LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE('{t0}', CHAR(10), ''), CHAR(13), ''), CHAR(9), ''), CHAR(160), ''))),
                                                                              LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE('{t1}', CHAR(10), ''), CHAR(13), ''), CHAR(9), ''), CHAR(160), ''))) ,LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE('{t2}', CHAR(10), ''), CHAR(13), ''), CHAR(9), ''), CHAR(160), ''))) )
                                                                               end";
                            Console.WriteLine($"item: '{t0}', Code: '{t1}' ,Orders: '{t2}' ");
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
                                    Console.WriteLine($"kill process EXCEL Successfully");
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine($"kill process EXCEL FAILED!!!!");
                                }
                            }
                        }
                        Console.WriteLine($"start transfer file");
                        try
                        {
                            File.Move(Folder2 + filename + typeFile, newFolder + filename + date.ToString().Replace(":", "_").Replace(" ", "") + typeFile);
                            Console.WriteLine(Folder2 + filename + typeFile + " Transfer complict");
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            Console.WriteLine($"In folder {Folder2 + filename + typeFile} file not faund");
                        }
                    }
                }
                Console.WriteLine("Stop");
                Console.ReadKey();
            }
        }
    }

}
