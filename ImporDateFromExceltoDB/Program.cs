using System.Data;
using System;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace ImporDateFromExceltoDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string ConnectionString = "Data Source =172.27.1.26; Initial Catalog = ILS; User ID =manh; Password =1q2w#E$R";

            SqlConnection sqlConnect = new SqlConnection(ConnectionString);
            sqlConnect.Open();


           var dataRage= ReadExel();

            foreach (DataRow dr in dataRage.Rows)
            {
               // Console.WriteLine($"values({dr[0]},{dr[1]}");
               var sqlExpression = $"insert into ils.dbo.TestImportExel (item,code) values('{dr[0]}','{dr[1]}')";
               SqlCommand command = new SqlCommand(sqlExpression, sqlConnect);
               command.ExecuteNonQuery();
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
                Excel.Workbook excelBook = application.Workbooks.Open(@"C:\Проэкты MS VS\Коледино.xlsx");
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
                return myTable;
            }
        }
    }
}
