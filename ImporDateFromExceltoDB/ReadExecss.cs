using System;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
namespace ImporDateFromExceltoDB
{
    
    class ReadExecss
    {
        public  DataTable Data { get; set; }
       

        public void ReadExel(string filename)
        {
            Excel.Application application = new Excel.Application();
            DataTable myTable = new DataTable("MyDataTable");
            if (application == null)
            {
                Console.WriteLine("EXCEL not installed");
                Data= myTable;
            }
            try
            {
                Excel.Workbook excelBook = application.Workbooks.Open(filename);
                Excel._Worksheet excelSheet = application.Sheets[1];
                Excel.Range excelRange = excelSheet.UsedRange;
                int rows = excelRange.Rows.Count;
                int cols = excelRange.Columns.Count;
                myTable.Columns.Add("Barcodes", typeof(string));
                myTable.Columns.Add("Zakaz", typeof(string));
                myTable.Columns.Add("Order", typeof(string));
                for (int i = 2; i <= rows; i++)
                {
                    DataRow myNewRow = myTable.NewRow();
                    myNewRow["Barcodes"] = excelRange.Cells.Value2[i, 1].ToString(); // .ToString(); //string
                    myNewRow["Zakaz"] = excelRange.Cells.Value2[i, 2].ToString();
                    myNewRow["Order"] = excelRange.Cells.Value2[i, 3].ToString();
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
            Data= myTable;
        }
    }
}
