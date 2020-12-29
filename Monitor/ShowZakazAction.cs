using SmartTrade.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monitor
{
    /// <summary>
    /// Инструкция Transact-SQL и тип подключения 
    /// </summary>
    /// <param name="cmdstr">инструкция Transact-SQL</param>
    /// <param name="IsTempConnection">Если указано true - новое временное соединение, если null - то OutsideConnection, иначе - текущее постоянное соединение </param>
    /// <returns>cmdstr, new SqlParameter[] { }, CommandType.Text, IsTempConnection</returns>
	///SQLHelper.ExecuteReaderText (string cmdstr, bool? IsTempConnection = false)
    public partial class ShowZakazAction : Form
    {

        public ShowZakazAction()
        {
            InitializeComponent();
        }
        public ShowZakazAction(String Item):this()
        {
            ItemInProcess.DataSource = SQLHelper.ExecuteReaderText(@"select Logaction as [Действие],ModifiedDate as [Дата] from RobotLog with(nolock) where 
RobotName not like '%StoreHouse_ASSIGN_WORK%'
and 
RobotName not like '%KZ_Unioperation%'
and 
RobotName not like '%StoreHouse_Complete_WORK_STEP1%' and Logaction like '%" + Item+"%' order by ModifiedDate", null);
        }
    }
}
