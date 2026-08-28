using OfficeOpenXml;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;


namespace ImporDateFromExceltoDB
{
    class Program
    {
        static string connectionString = @"Data Source=W-SRVSQL04;Initial Catalog=ILS;Persist Security Info=True;User ID=szkoadmin;Password=#t1h2u3$;";
        static string Folder2 = @$"\\whs.local\DFS\services_wms\Wildberries\";
        static string newFolder = @$"\\whs.local\DFS\services_wms\Wildberries\old_wailberis\";

        static SqlConnection sqlConnect = new SqlConnection(connectionString);

        static void Main(string[] args)
        {
            // Устанавливаем лицензию для EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            sqlConnect.Open();

            while (true)
            {
                TimerCallback();
                Thread.Sleep(300_000); // 5 минут
            }
        }

        static void TimerCallback()
        {
            // Убиваем процессы Excel (на всякий случай)
            KillExcelProcesses();

            DateTimeOffset dateStart = DateTimeOffset.Now;
            Console.WriteLine($"Start  {dateStart}");

            var dir = new DirectoryInfo(Folder2);

            try
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    string typeFile = "";
                    string filename = "";

                    try
                    {
                        typeFile = Path.GetExtension(file.FullName);
                        filename = Path.GetFileNameWithoutExtension(file.Name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ошибка получения имени файла: {e.Message}");
                        continue;
                    }

                    if (typeFile == ".xlsx" || typeFile == ".xls")
                    {
                        Console.WriteLine($"Read EXCEL: {file.Name}");

                        string fullPath = Folder2 + filename + typeFile;

                        // Читаем Excel через EPPlus
                        var dataTable = ReadExcelWithEPPlus(fullPath);

                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            Console.WriteLine($"Загружено {dataTable.Rows.Count} записей");

                            // Обрабатываем каждую строку
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                try
                                {
                                    var BarcodesCorob = dr["BarcodesCorob"]?.ToString() ?? "";
                                    var SupllyID = dr["SupllyID"]?.ToString() ?? "";
                                    var DataDeparture = dr["DataDeparture"] != DBNull.Value
                                        ? ((DateTime)dr["DataDeparture"]).ToString("yyyy-MM-dd HH:mm:ss")
                                        : "";
                                    var WareHouse = dr["WareHouse"]?.ToString() ?? "";
                                    var BarcodesForPrint = dr["BarcodesForPrint"]?.ToString() ?? "";
                                    var orders = dr["orders"]?.ToString() ?? "";

                                    // Проверяем обязательные поля
                                    if (string.IsNullOrEmpty(BarcodesCorob) ||
                                        string.IsNullOrEmpty(SupllyID) ||
                                        string.IsNullOrEmpty(DataDeparture))
                                    {
                                        Console.WriteLine($"Пропущена запись: не хватает данных");
                                        continue;
                                    }

                                    // Формируем запрос к ХП
                                    var sqlExpression = $@"ILS.DBO.[900_045_InsertDateInTableWBDBO_SSC] 
                                    @BarCopdeForPrint = N'{EscapeSql(BarcodesForPrint)}',
                                    @SupllyID = N'{EscapeSql(SupllyID)}',
                                    @OrderId = N'{EscapeSql(orders)}',
                                    @BarcodesCorob = N'{EscapeSql(BarcodesCorob)}',
                                    @DataDeparture = N'{EscapeSql(DataDeparture)}',
                                    @WareHouse = N'{EscapeSql(WareHouse)}'";

                                    Console.WriteLine($"Обработка: ШК='{BarcodesCorob}', Поставка='{SupllyID}', Дата='{DataDeparture}'");

                                    using (SqlCommand command = new SqlCommand(sqlExpression, sqlConnect))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ошибка при обработке строки: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Нет данных в файле {file.Name}");
                        }

                        // Переносим файл в архив
                        try
                        {
                            string sourcePath = Folder2 + filename + typeFile;
                            string destPath = newFolder + filename + typeFile;

                            Console.WriteLine($"Перемещение файла: {sourcePath} -> {destPath}");

                            // Создаем папку если её нет
                            Directory.CreateDirectory(newFolder);

                            File.Move(sourcePath, destPath, true);
                            Console.WriteLine($"Файл {filename + typeFile} перемещен в архив");
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine($"Файл {filename + typeFile} не найден для перемещения");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка перемещения файла: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Общая ошибка: {e.Message}");
                Console.WriteLine($"StackTrace: {e.StackTrace}");
            }

            // Снова убиваем Excel процессы
            KillExcelProcesses();

            DateTimeOffset dateStop = DateTimeOffset.Now;
            Console.WriteLine($"Stop {dateStop}");
            Console.WriteLine("----------------------------------------");
        }

        /// <summary>
        /// Чтение Excel файла с помощью EPPlus
        /// </summary>
        static DataTable ReadExcelWithEPPlus(string filePath)
        {
            DataTable myTable = new DataTable("MyDataTable");

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Файл {filePath} не найден");
                    return myTable;
                }

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    // Проверяем наличие данных
                    if (worksheet.Dimension == null)
                    {
                        Console.WriteLine("Файл пуст или не содержит данных");
                        return myTable;
                    }

                    int rows = worksheet.Dimension.Rows;
                    int cols = worksheet.Dimension.Columns;

                    Console.WriteLine($"Строк: {rows}, Колонок: {cols}");

                    // Создаем колонки DataTable
                    myTable.Columns.Add("BarcodesCorob", typeof(string));
                    myTable.Columns.Add("SupllyID", typeof(string));
                    myTable.Columns.Add("DataDeparture", typeof(DateTime));
                    myTable.Columns.Add("WareHouse", typeof(string));
                    myTable.Columns.Add("BarcodesForPrint", typeof(string));
                    myTable.Columns.Add("orders", typeof(string));

                    // Проходим по строкам (начиная со 2-й, т.к. 1-я - заголовки)
                    for (int i = 2; i <= rows; i++)
                    {
                        try
                        {
                            DataRow myNewRow = myTable.NewRow();
                            string orders = "";

                            // Проверяем наличие колонки 6 (ORDERid)
                            if (cols < 6)
                            {
                                orders = "!!!";
                            }
                            else
                            {
                                var orderCell = worksheet.Cells[i, 6].Value;
                                orders = orderCell?.ToString() ?? "!!!";
                            }

                            // Получаем значения ячеек
                            var cell1 = worksheet.Cells[i, 1].Value; // ШК короба
                            var cell2 = worksheet.Cells[i, 2].Value; // номер поставки
                            var cell3 = worksheet.Cells[i, 3].Value; // Дата поставки
                            var cell4 = worksheet.Cells[i, 4].Value; // склад
                            var cell5 = worksheet.Cells[i, 5].Value; // ШК для печати

                            // Проверяем обязательные поля
                            Console.WriteLine($" Проверяем обязательные поля Строка {cell1}: cell2 {cell2} orders{orders}");
                            if (cell1 != null && cell2 != null && !string.IsNullOrEmpty(orders))
                            {
                               

                                myNewRow["BarcodesCorob"] = cell1.ToString();
                                myNewRow["SupllyID"] = cell2.ToString();

                                // ⭐ Обработка даты
                                myNewRow["DataDeparture"] = ParseExcelDate(cell3);

                                myNewRow["WareHouse"] = cell4?.ToString() ?? "";
                                myNewRow["BarcodesForPrint"] = cell5?.ToString() ?? "";
                                myNewRow["orders"] = orders;

                                myTable.Rows.Add(myNewRow);
                            }
                            else
                            {
                                Console.WriteLine($"Строка {i}: пропущена (нет обязательных данных)");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка в строке {i}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения Excel: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            return myTable;
        }

        /// <summary>
        /// Парсинг даты из Excel
        /// </summary>
        static DateTime ParseExcelDate(object cellValue)
        {
            if (cellValue == null)
                return DateTime.MinValue;

            // Если уже DateTime
            if (cellValue is DateTime dateTime)
            {
                // Проверяем на "нулевую" дату 1900-01-01
                if (dateTime.Year == 1900 && dateTime.Month == 1 && dateTime.Day == 1)
                    return DateTime.MinValue;

                return dateTime;
            }

            // Если число (серийный номер Excel)
            if (cellValue is double numericValue)
            {
                if (numericValue <= 0)
                    return DateTime.MinValue;

                try
                {
                    var date = DateTime.FromOADate(numericValue);

                    if (date.Year == 1900 && date.Month == 1 && date.Day == 1)
                        return DateTime.MinValue;

                    return date;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

            // Если строка
            if (cellValue is string dateString)
            {
                if (string.IsNullOrWhiteSpace(dateString))
                    return DateTime.MinValue;

                // Пробуем парсить ISO формат "2026-08-29 00:00:00"
                if (DateTime.TryParse(dateString, out DateTime parsedDate))
                {
                    if (parsedDate.Year == 1900 && parsedDate.Month == 1 && parsedDate.Day == 1)
                        return DateTime.MinValue;

                    return parsedDate;
                }

                // Пробуем другие форматы
                string[] formats = new[]
                {
                "dd.MM.yyyy",
                "dd.MM.yyyy HH:mm:ss",
                "dd/MM/yyyy",
                "yyyy-MM-dd",
                "yyyy-MM-dd HH:mm:ss",
                "MM/dd/yyyy",
                "d.M.yyyy"
            };

                foreach (var format in formats)
                {
                    if (DateTime.TryParseExact(
                        dateString.Trim(),
                        format,
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime parsed))
                    {
                        if (parsed.Year == 1900 && parsed.Month == 1 && parsed.Day == 1)
                            return DateTime.MinValue;

                        return parsed;
                    }
                }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Экранирование SQL-инъекций (простая очистка)
        /// </summary>
        static string EscapeSql(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Заменяем опасные символы
            return value
                .Replace("'", "''")  // Экранируем одинарные кавычки
                .Replace("\"", "\"\""); // Экранируем двойные кавычки
        }

        /// <summary>
        /// Убийство процессов Excel
        /// </summary>
        static void KillExcelProcesses()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при завершении Excel процессов: {ex.Message}");
            }
        }

    }

}
