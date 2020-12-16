using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
namespace ImporDateFromExceltoDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string ConnectionString = "Data Source =172.27.1.26; Initial Catalog = ILS; User ID =manh; Password =1q2w#E$R";
            var sqlExpression = "insert into ils.dbo.TestImportExel (item,code) values('TTTT','12312312')";
            SqlConnection sqlConnect = new SqlConnection(ConnectionString);
            sqlConnect.Open();          

            SqlCommand command = new SqlCommand(sqlExpression, sqlConnect);
            command.ExecuteNonQuery();

          
        }
    }
}
