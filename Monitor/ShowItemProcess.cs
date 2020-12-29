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
    /// Отправляет инструкцию Transact-SQL в соединение 
    /// </summary>
    /// <param name="cmdstr">инструкция Transact-SQL</param>
	/// <param name="parameters">Параметры подключения</param>
    /// <param name="IsTempConnection">Если указано true - новое временное соединение, если null - то OutsideConnection, иначе - текущее постоянное соединение </param>
    /// <returns>cmdstr, parameters, CommandType.Text, IsTempConnection</returns>
	///SQLHelper.ExecuteReaderWithParametersText (string cmdstr, SqlParameter[] parameters, bool? IsTempConnection = false)
    public partial class ShowItemProcess : Form
    {

        public ShowItemProcess()
        {
            InitializeComponent();
        }
        public ShowItemProcess(String Item):this()
        {
            ItemInProcess.DataSource = SQLHelper.ExecuteReaderWithParametersText("select * from Statistics_GetItemMoving(@Item) order by INTERNAL_INSTRUCTION_NUM", new SqlParameter[] { new SqlParameter("@Item", Item) }, null);
        }
    }
}
