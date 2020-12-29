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
using SmartTrade;
using SmartTrade.Components;
using System.Security.Principal;
using SmartTrade.BusinessObjects;
using System.Reflection;



namespace Monitor
{
    /// <summary>
    /// Получение данных  
    /// </summary>
    /// <param name="cmdstr">Название объекта</param>
    /// <param name="parameters">Параметры подключения</param>
    /// <param name="cmdType">Тип объекта</param>
    /// <param name="IsTempConnection">Если указано true - новое временное соединение, если null - то OutsideConnection, иначе - текущее постоянное соединение </param>
    /// <returns></returns>
    ///SQLHelper.GetDataSet (string cmdstr, SqlParameter[] parameters, CommandType cmdType,bool? IsTempConnection = false)

    /// <summary>
    /// Выполнение инструкции Transact-SQL и возвращение количества задействованных строк   
    /// </summary>
    /// <param name="cmdstr">инструкция Transact-SQL</param>
    /// <param name="parameters">Параметры подключения</param>
    /// <param name="IsTempConnection">Если указано true - новое временное соединение, если null - то OutsideConnection, иначе - текущее постоянное соединение </param>
    /// <returns>cmdstr, parameters, CommandType.Text, IsTempConnection</returns>
    ///SQLHelper.ExecuteNonQueryWithParametersText (cmdstr, parameters, CommandType.Text, IsTempConnection)

    /// <summary>
    /// Диалоговое окно 
    /// </summary>
    /// <param name=" Message"> Текст диалогового окна </param>
    /// <param name=" MB"> Кнопка подтверждения </param>
    /// <param name=" MI"> Иконка диалогового окна </param>
    ///_MessageBox.Show(String Message, MessageBoxButtons MB = MessageBoxButtons.OK, MessageBoxIcon MI = MessageBoxIcon.Information)
    public partial class Form1 : Form
    {
        protected IncrementalSearch<DataRowView> IS;
        public Form1()
        {
            InitializeComponent();




            IS = new IncrementalSearch<DataRowView>(this.upDGV, this.SearchBox);
            try
            {
                User.Instance().Login = WindowsIdentity.GetCurrent().Name;
                User.Instance().FullUserName = WindowsIdentity.GetCurrent().Name;
                User.Instance().UserGroup = 2;
                UserPermissionList.Instance().Clear();
                DataTable DT = SQLHelper.ExecuteReaderWithParametersSP("[User_GetPermissionList]", new SqlParameter[] { new SqlParameter("@UserLogin", User.Instance().Login) }, null);
                foreach (DataRow item in DT.Rows)
                    UserPermissionList.Instance().Add(Convert.ToInt32(item["RuleNum"]), Convert.ToInt32(item["Access"]));

                this.button2.Enabled = User.Instance().GetPermission(1);
                this.button4.Enabled = User.Instance().GetPermission(1);
            }
            catch (Exception ex)
            {
                _MessageBox.Show(ex.Message.ToString());
            }

            this.upDGV.CustomDicMenu.Items.Add("Отобрать текущее значение").Click += new EventHandler(CustomDictionaryMenu_Click0);
            this.upDGV.CustomDicMenu.Items.Add("Очистить фильтр").Click += new EventHandler(CustomDictionaryMenu_Click2);
        }

        private void CustomDictionaryMenu_Click2(object sender, EventArgs e)
        {
            upDGV.DataSource = CurrentDataSource;
            Condition = "";
        }

        DataTable CurrentDataSource = null;
        string Condition = "";
        private void CustomDictionaryMenu_Click0(object sender, EventArgs e)
        {
            if (this.upDGV.LastCellRightClicked == null || CurrentDataSource == null)
                return;

            this.upDGV.CurrentCell = this.upDGV.LastCellRightClicked;

            string[] ColumnValuePair = GetColumnValuePair(this.upDGV.CurrentCell.ColumnIndex, false);

            if (ColumnValuePair[1] != null)
            {
                if (ColumnValuePair[0] != string.Empty)
                {
                    Condition = Condition + (Condition == "" ? "" : " and ") + " (" + ColumnValuePair[0] + " = " + ColumnValuePair[1] + ") ";
                    try
                    {
                        upDGV.DataSource = CurrentDataSource.Select(Condition).CopyToDataTable();
                    }
                    catch (Exception)
                    {
                        Condition = "";
                    }
                    //this.Filtration(true, Condition);
                }
            }
        }

        private string[] GetColumnValuePair(int ColumnIndex, bool SQLStyle = false)
        {
            string[] ColumnValuePair = new string[2];

            if (SQLStyle)
            {
                ColumnValuePair[0] = string.Empty;
                PropertyInfo pInfo = upDGV.Columns[ColumnIndex].DataGridView.Rows[0].DataBoundItem.GetType().GetProperty(upDGV.Columns[ColumnIndex].DataPropertyName);
                foreach (FieldNameAttribute attr in Attribute.GetCustomAttributes(pInfo, typeof(FieldNameAttribute)))
                    ColumnValuePair[0] = attr.FieldName;
            }
            else
                ColumnValuePair[0] = "[" + upDGV.Columns[ColumnIndex].DataPropertyName + "]";

            ColumnValuePair[1] = this.upDGV.CurrentCell.Value == null ? "null" : this.upDGV.CurrentCell.Value.ToString();
            if (this.upDGV.Columns[ColumnIndex].ValueType.IsValueType && this.upDGV.CurrentCell.Value != DBNull.Value)
            {
                if (this.upDGV.Columns[ColumnIndex].ValueType == typeof(DateTime))
                {
                    if (SQLStyle)
                    {
                        ColumnValuePair[0] = " Convert(Date," + ColumnValuePair[0] + ")";
                        ColumnValuePair[1] = "Convert(Date,'" + ColumnValuePair[1] + "')";
                    }
                    else
                    {
                        ColumnValuePair[0] = "Convert(Substring(Convert(" + ColumnValuePair[0] + ",System.String),1,10),System.DateTime)";
                        ColumnValuePair[1] = "Convert(Substring('" + ColumnValuePair[1] + "',1,10),System.DateTime)";
                    }
                }
                else
                    ColumnValuePair[1] = ColumnValuePair[1].Replace(',', '.');
            }
            else
                if (this.upDGV.CurrentCell.Value != null)
                ColumnValuePair[1] = "'" + ColumnValuePair[1].Replace("'", "''") + "'";

            if (/*SQLStyle &&*/ this.upDGV.CurrentCell.Value == DBNull.Value)
            {
                ColumnValuePair[0] = "isnull(" + ColumnValuePair[0] + ",'')";
                ColumnValuePair[1] = "''";

            }


            return ColumnValuePair;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void UpdateList()
        {

            DataSet ds = SQLHelper.GetDataSet("Service_Replenishment_FirstOrder2", new SqlParameter[]{
              //  new SqlParameter("@Runrepl",0),
              //  new SqlParameter("@RunComplectation",1),
                new SqlParameter("@UserName",User.Instance().Login)}, CommandType.StoredProcedure, null);

         

            string Qtywork;
            DataTable dd = SQLHelper.ExecuteReaderText("Select qtywork From ils.dbo.EKS_SpecifyNumberTasks s where Zone='Mez' ", null);
            Qtywork = dd.Rows[0][0].ToString();

            DataTable qtyOpenWork = SQLHelper.ExecuteReaderText("Select count(*) From ils.dbo.WORK_INSTRUCTION where WORK_type like N'Отбор - мезонин%' and WORK_type not like N'Отбор - мезонин - % интер' and INSTRUCTION_TYPE='Header' and CONDITION='Open' and HOLD_CODE is null", null);
            string QtyOpenWork = qtyOpenWork.Rows[0][0].ToString();

            button3.Text = "Указать кол-во заданий";
            button3.Text = button3.Text + " " + Qtywork+"/"+ QtyOpenWork;


            ds.Tables[1].ResetReadOnly();
            // ds.Tables[1].DefaultView.RowFilter = "Клиент like '%библио%' ";
            CurrentDataSource = ds.Tables[1];
            upDGV.DataSource = ds.Tables[1];//.DefaultView;//ds.Tables[1];//.DefaultView.RowFilter = "Клиент like '%библио%' ";
                                            //  leftDGV.DataSource = ds.Tables[0];
                                            //  rightDGV.DataSource = ds.Tables[2];           
                                            // var datasum = ds.Tables[1];
                                            // // dttt.Columns.Remove("erp_order");
                                            // // var datasum = dttt;
                                            //
                                            // var rfdf = datasum.AsEnumerable();
                                            // (from s in rfdf
                                            //  where s.Field<string>("Место отбора") == "А"
                                            //  select s).Single<DataRow>().SetField("Место отбора", "цувцу");
                                            //  rfdf.CopyToDataTable();
                                            //        var sumcolumn = rfdf
                                            //                             .GroupBy(g => g.Field<string>("Место отбора"))
                                            //                             .Select(g =>
                                            //                             {
                                            //                                 var row = datasum.NewRow();                                            
                                            //                                 row["Место отбора"] = g.Key;
                                            //                                 row["собрать_строк"] = g.Sum(r => r.Field<int>("собрать_строк"));
                                            //                                 row["Собрать"] = g.Sum(r => r.Field<int>("Собрать"));
                                            //                                 return row;
                                            //                             }   ).CopyToDataTable();  
                                            // sumcolumn.Columns.Remove("erp_order");//Клиент НомерЗапуска	AtFirsTime
                                            // sumcolumn.Columns.Remove("Клиент");
                                            // sumcolumn.Columns.Remove("НомерЗапуска");
                                            // sumcolumn.Columns.Remove("AtFirsTime");//Адрес	отгрузка
                                            // sumcolumn.Columns.Remove("Адрес");
                                            // sumcolumn.Columns.Remove("отгрузка");  //										
                                            // sumcolumn.Columns.Remove("Литраж");
                                            // sumcolumn.Columns.Remove("Подпитать_строк");
                                            // sumcolumn.Columns.Remove("Нельзя подпитать");
                                            // sumcolumn.Columns.Remove("Перемещается");
                                            // sumcolumn.Columns.Remove("PartialAssembly");
                                            // sumcolumn.Columns.Remove("Полное_пополн");
                                            // sumcolumn.Columns.Remove("Собрать_пачек");
                                            // sumcolumn.Columns.Remove("Подпитать_штук");
                                            // sumcolumn.Columns.Remove("Подпитать_пач");
                                            // sumcolumn.Columns.Remove("РучнаяОбработка"); //           
                                            // sumcolumn.Columns.Remove("товар_перемещение");
                                            // sumcolumn.Columns.Remove("Shipment_ID");
                                            // sumcolumn.Columns.Remove("INTERNAL_SHIPMENT_NUM");
                                            // sumcolumn.Columns.Remove("Нельзя_подпитать_товар");
                                            // sumcolumn.Columns.Remove("Комментарий");
                                            // sumcolumn.Columns.Remove("ПодпитатьТовар");
                                            // sumcolumn.Columns.Remove("Приоритеты");
                                            // sumcolumn.Columns.Remove("Время поступления");            
                                            // dataGridView1.DataSource = sumcolumn;

           // int sum = 0;
           // for (int i = 0; i < upDGV.Rows.Count; i++)
           // {
           //     sum += Convert.ToInt32(upDGV[2, i].Value);
           // }

            DataTable infoDataDGV1 = SQLHelper.ExecuteReaderSP("ils.[dbo].[900_003_ForMonitorWinFormRep]", null);
            dataGridView1.ReadOnly =true;
            dataGridView1.AllowUserToAddRows =false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DataSource = infoDataDGV1;

            var  DTForSumAndCountOrders= SQLHelper.ExecuteReaderText("Select ExtDataBy1C.[dbo].[GetInfoOrdersInPool] ()",null);
            label1.Text = DTForSumAndCountOrders.Rows[0].ItemArray[0].ToString();
            label2.Text = SQLHelper.ExecuteReaderText("Select * From ils.[dbo].[Get_InfoReplItemInPool]()", null).Rows[0].ItemArray[0].ToString();
            label3.Text = "Задания для УПШ (Открытые): "+SQLHelper.ExecuteReaderText("select ExtDataBy1C.[dbo].[GetQueueLenUpZone]()", null).Rows[0].ItemArray[0].ToString();
            foreach (DataGridViewColumn col in upDGV.Columns)
            {
                if (col.DataPropertyName != "AtFirsTime" && col.DataPropertyName != "PartialAssembly")
                {
                    col.ReadOnly = true;
                }
                if (col.DataPropertyName == "AtFirsTime")
                {
                    col.HeaderText = "Срочность";
                }
                if (col.DataPropertyName == "PartialAssembly")
                {
                    col.HeaderText = "Частичная сборка";
                }
                if (col.DataPropertyName == "erp_order")
                {
                    col.HeaderText = "Заказ";
                }
            }
            upDGV.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            upDGV.AllowUserToOrderColumns = true;
            upDGV.AllowUserToResizeColumns = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in ((DataTable)upDGV.DataSource).Rows)
            {
                if (row.RowState == DataRowState.Modified)
                {
                    SQLHelper.ExecuteNonQueryWithParametersText("insert ZakazPriority(Shipment_ID,PriorityState,IsPartialComplectation) values(@Shipment_ID,@PriorityState,@IsPartialComplectation)",
                                                                     new SqlParameter[]{
                                                                        new SqlParameter("@Shipment_ID", row["Shipment_ID"]),
                                                                        new SqlParameter("@PriorityState", row["AtFirsTime"]),
                                                                        new SqlParameter("@IsPartialComplectation", row["PartialAssembly"])
                                                                        }
                                                                     , null);
                }
            }
            UpdateList();
        }

        private void upDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > -1 && upDGV.Columns[e.ColumnIndex].DataPropertyName == "товар_перемещение" && e.RowIndex > -1 && upDGV[e.ColumnIndex, e.RowIndex].Value.ToString() != "")
            {
                (new ShowItemProcess(upDGV[e.ColumnIndex, e.RowIndex].Value.ToString())).ShowDialog();
            }

            if (e.ColumnIndex > -1 && upDGV.Columns[e.ColumnIndex].DataPropertyName == "Нельзя_подпитать_товар" && e.RowIndex > -1 && upDGV[e.ColumnIndex, e.RowIndex].Value.ToString() != "")
            {
                (new ShowItemRest(upDGV[e.ColumnIndex, e.RowIndex].Value.ToString())).ShowDialog();
            }

            if (e.ColumnIndex > -1 && upDGV.Columns[e.ColumnIndex].DataPropertyName == "erp_order" && e.RowIndex > -1 && upDGV[e.ColumnIndex, e.RowIndex].Value.ToString() != "")
            {
                (new ShowZakazAction(upDGV[e.ColumnIndex, e.RowIndex].Value.ToString())).ShowDialog();
            }
        }

        private void upDGV_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

            if (e.RowIndex >= 0)
                if (e.ColumnIndex >= 0)
                {
                    if (upDGV.Rows[e.RowIndex].Cells["PartialAssembly"].Value.ToString().Trim() == "2")
                    {
                        e.CellStyle.BackColor = Color.Yellow;
                        e.CellStyle.ForeColor = Color.Black;
                    }

                    else
                    {
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.ForeColor = Color.Black;
                    }


                }

        }


        string[] strarr;
        private void button3_Click(object sender, EventArgs e)
        {

            DialogResult dresult = InputBox.Show("Укажите кол-во открытых заданий на мезонине", "Кол-во заданий", out strarr);
            if (dresult == System.Windows.Forms.DialogResult.OK)
            {

                string UserBarcode = strarr.FirstOrDefault(r => r.Length > 0);
                int num;
                bool isNum = int.TryParse(UserBarcode, out num);
                if (isNum)
                {

                    string Users = Environment.UserName;
                    DataTable tdd = SQLHelper.ExecuteReaderWithParametersSP("ils.[dbo].[EKS_SpecifyNumberTasks_SP]", new SqlParameter[] { new SqlParameter("@qtywork", UserBarcode),
                                                                                                                                          new SqlParameter("@zone", "MEZ") ,
                                                                                                                                          new SqlParameter("@usersname",Users ) }, null);



                    if (tdd != null)
                    {

                        _MessageBox.Show("Указано кол во заданий" + UserBarcode.ToString());
                        UpdateList();
                    }
                }
                else
                    _MessageBox.Show("Указывайте только число");
            }
            else
                _MessageBox.Show("Кол-во заданий не указано");
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            if (upDGV.CurrentRow != null)
            {
                if (upDGV.CurrentRow != null)
                {
                    string Result;
                    string[] strarr;
                    Result = upDGV.CurrentRow.Cells["Клиент"].Value.ToString();
                    DialogResult dresult = InputBox.Show("Укажите для "+ Result + " приоритет", "Укажите приоритеты в виде (20,23,33)", out strarr);
                    if (dresult == System.Windows.Forms.DialogResult.OK)
                    {
                        string UserBarcode = strarr.FirstOrDefault(r => r.Length > 0);

                        int num;
                        bool parsed = Int32.TryParse(UserBarcode, out num);

                        if (parsed)

                        {

                            if (UserBarcode == null || UserBarcode.Length == 0)
                            {
                                UserBarcode = " ";
                            }


                            if (Result != null)
                            {
                                if (_MessageBox.Show("Приоритет: " + UserBarcode.ToString(), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                {

                                    DataTable tdd = SQLHelper.ExecuteReaderWithParametersSP("ils.dbo.EKS_UpPrirityCustomerName", new SqlParameter[] { new SqlParameter("@customer_name", Result), new SqlParameter("@prioritet", UserBarcode) }, null);
                                    DataGridView Grid = new DataGridView();
                                    Grid.DataSource = tdd;

                                    int msg2 = Convert.ToInt32(tdd.Rows[0][0]);


                                    string msgprior;
                                    msgprior = Convert.ToString(tdd.Rows[0][1]);
                                    if (msg2 == 0)
                                    {
                                        _MessageBox.Show("У данного клиента уже указан приоритет  " + msgprior);
                                    }

                                    if (msg2 == 3)
                                    {
                                        _MessageBox.Show("Укажите приоритет в виде (20,23,33) , Указанный приоритет " + msgprior +" не верный");
                                    }

                                    if (msg2 == 1)
                                    {


                                        if (tdd != null)
                                        {

                                            _MessageBox.Show("Указанные приортеты " + UserBarcode.ToString());
                                        }
                                        UpdateList();
                                    }
                                }
                            }

                        }
                        else
                        {
                            _MessageBox.Show("Укажите приоритет в виде (20,23,33) ");
                            
                        }
                    }
                    else
                        _MessageBox.Show("Укажите приоритет");
                    

                }
            }
        }
    }
}
