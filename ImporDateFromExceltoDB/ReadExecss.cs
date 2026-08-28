using OfficeOpenXml;
using System;
using System.Data;
using System.IO;

namespace ImporDateFromExceltoDB
{
    class ReadExecss
    {
        public DataTable Data { get; set; }

        public void ReadExel(string filename)
        {
            DataTable myTable = new DataTable("MyDataTable");

            // Устанавливаем лицензию для EPPlus (обязательно!)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                // Проверяем существование файла
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Файл {filename} не найден");
                    Data = myTable;
                    return;
                }

                using (var package = new ExcelPackage(new FileInfo(filename)))
                {
                    // Получаем первый лист
                    var worksheet = package.Workbook.Worksheets[0];

                    // Определяем используемый диапазон
                    int rows = worksheet.Dimension?.Rows ?? 0;
                    int cols = worksheet.Dimension?.Columns ?? 0;

                    // Проверяем, есть ли данные
                    if (rows == 0 || cols == 0)
                    {
                        Console.WriteLine("Файл пуст или не содержит данных");
                        Data = myTable;
                        return;
                    }

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
                        DataRow myNewRow = myTable.NewRow();
                        string orders = "";

                        // Проверяем наличие колонки 6 (ORDERid)
                        if (cols < 6)
                        {
                            orders = "!!!";
                        }
                        else if (cols == 6)
                        {
                            var orderCell = worksheet.Cells[i, 6].Value;
                            if (orderCell != null)
                            {
                                orders = orderCell.ToString();
                            }
                        }

                        // Получаем значения ячеек
                        var cell1 = worksheet.Cells[i, 1].Value; // ШК короба
                        var cell2 = worksheet.Cells[i, 2].Value; // номер поставки
                        var cell3 = worksheet.Cells[i, 3].Value; // Дата поставки
                        var cell4 = worksheet.Cells[i, 4].Value; // склад
                        var cell5 = worksheet.Cells[i, 5].Value; // ШК для печати

                        // Проверяем обязательные поля (1, 2 и orders)
                        if (cell1 != null && cell2 != null && orders != null)
                        {
                            // Заполняем строку
                            myNewRow["BarcodesCorob"] = cell1.ToString();
                            myNewRow["SupllyID"] = cell2.ToString();

                            // ⭐ Обработка даты
                            myNewRow["DataDeparture"] = GetDateValue(cell3);

                            myNewRow["WareHouse"] = cell4?.ToString() ?? "";
                            myNewRow["BarcodesForPrint"] = cell5?.ToString() ?? "";
                            myNewRow["orders"] = orders;

                            myTable.Rows.Add(myNewRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении Excel: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            Data = myTable;
        }

        /// <summary>
        /// Преобразование значения ячейки в DateTime
        /// </summary>
        private DateTime GetDateValue(object cellValue)
        {
            if (cellValue == null)
                return DateTime.MinValue;

            // Вариант 1: Уже DateTime
            if (cellValue is DateTime dateTime)
            {
                // Проверяем на "нулевую" дату 1900-01-01
                if (dateTime.Year == 1900 && dateTime.Month == 1 && dateTime.Day == 1)
                    return DateTime.MinValue;

                return dateTime;
            }

            // Вариант 2: Число (серийный номер Excel)
            if (cellValue is double numericValue)
            {
                if (numericValue <= 0)
                    return DateTime.MinValue;

                try
                {
                    var date = DateTime.FromOADate(numericValue);

                    // Проверяем на "нулевую" дату
                    if (date.Year == 1900 && date.Month == 1 && date.Day == 1)
                        return DateTime.MinValue;

                    return date;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

            // Вариант 3: Строка
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
    }
    //class ReadExecss
    //{
    //    public DataTable Data { get; set; }

    //    public void ReadExel(string filename)
    //    {
    //        Excel.Application application = new Excel.Application();
    //        DataTable myTable = new DataTable("MyDataTable");
    //        if (application == null)
    //        {
    //            Console.WriteLine("EXCEL not installed");
    //            Data = myTable;
    //        }
    //        try
    //        {
    //            Excel.Workbook excelBook = application.Workbooks.Open(filename);
    //            Excel._Worksheet excelSheet = application.Sheets[1];
    //            Excel.Range excelRange = excelSheet.UsedRange;
    //            int rows = excelRange.Rows.Count;
    //            int cols = excelRange.Columns.Count;
    //            //myTable.Columns.Add("Barcodes", typeof(string));
    //            //myTable.Columns.Add("Zakaz", typeof(string));
    //            //myTable.Columns.Add("Order", typeof(string));
    //            myTable.Columns.Add("BarcodesCorob", typeof(string));
    //            myTable.Columns.Add("SupllyID", typeof(string));
    //            myTable.Columns.Add("DataDeparture", typeof(DateTime));
    //            myTable.Columns.Add("WareHouse", typeof(string));
    //            myTable.Columns.Add("BarcodesForPrint", typeof(string));
    //            myTable.Columns.Add("orders", typeof(string));
    //            for (int i = 2; i <= rows; i++)
    //            {
    //                DataRow myNewRow = myTable.NewRow();
    //                string orders = "";

    //                if (cols < 6)
    //                {
    //                    orders = "!!!";
    //                }
    //                if (cols == 6)
    //                {
    //                    if (excelRange.Cells.Value2[i, 6] != null)
    //                    {
    //                        orders = excelRange.Cells.Value2[i, 6].ToString();
    //                    }
    //                }

    //                if (excelRange.Cells.Value2[i, 1] != null && excelRange.Cells.Value2[i, 2] != null && orders != null)
    //                {
    //                    myNewRow["BarcodesCorob"] = excelRange.Cells.Value2[i, 1].ToString(); // .ToString(); //string
    //                    myNewRow["SupllyID"] = excelRange.Cells.Value2[i, 2].ToString();
    //                    myNewRow["DataDeparture"] = excelRange.Cells.Value2[i, 3].ToString(); ;
    //                    myNewRow["WareHouse"] = excelRange.Cells.Value2[i, 4].ToString(); ;
    //                    myNewRow["BarcodesForPrint"] = excelRange.Cells.Value2[i, 5].ToString(); ;
    //                    myNewRow["orders"] = orders;

    //                    myTable.Rows.Add(myNewRow);
    //                }



    //            }
    //            if (excelBook != null)
    //            {
    //                excelBook.Close(false, Type.Missing, Type.Missing);
    //                application.Workbooks.Close();
    //                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelBook);
    //                application.Quit();
    //                GC.Collect();
    //                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
    //            }
    //        }

    //        catch (System.Runtime.InteropServices.COMException)
    //        {
    //            Console.WriteLine($"В папке {filename} нет файла");
    //        }
    //        Data = myTable;
    //    }

    //}
}
