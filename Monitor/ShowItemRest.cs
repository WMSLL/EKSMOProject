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
    public partial class ShowItemRest : Form
    {

        public ShowItemRest()
        {
            InitializeComponent();
        }
        public ShowItemRest(String Item):this()
        {
            ItemInProcess.DataSource = SQLHelper.ExecuteReaderWithParametersText("SELECT  LOCATION as [Место],ID.DESCRIPTION as [Название],LI.ON_HAND_QTY as [В ячейке],LI.IN_TRANSIT_QTY as [Прин. зап.],LI.ALLOCATED_QTY as [Выд. зап.],ID.packItem as [Ст.] FROM [ExtDataBy1C].[dbo].[LocInv] LI inner join ItemDescr ID on  LI.ITEM = ID.ITEM where LI.ITEM=@ITEM", new SqlParameter[] { new SqlParameter("@ITEM", Item) }, null);
        }
    }
}
