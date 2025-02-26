using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsLessons
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Создаем DataTable и добавляем данные
            DataTable table = new DataTable();
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("Item", typeof(string));
            table.Columns.Add("Price", typeof(decimal));

            table.Rows.Add("Fruits", "Apple", 1.2m);
            table.Rows.Add("Fruits", "Banana", 0.5m);
            table.Rows.Add("Vegetables", "Carrot", 0.8m);
            table.Rows.Add("Fruits", "Orange", 1.0m);
            table.Rows.Add("Vegetables", "Potato", 0.3m);
            table.Rows.Add("Fruits", "Grapes", 2.0m);
            table.Rows.Add("Vegetables", "Tomato", 0.9m);

            // Группировка данных
            var groupedData = from row in table.AsEnumerable()
                              group row by row.Field<string>("Category") into grp
                              select new
                              {
                                  Category = grp.Key,
                                  Items = grp.ToList()
                              };

            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Category", typeof(string));
            resultTable.Columns.Add("Item", typeof(string));
            resultTable.Columns.Add("Price", typeof(decimal));

            // Заполнение результирующей таблицы
            foreach (var group in groupedData)
            {
                foreach (var item in group.Items)
                {
                    resultTable.Rows.Add(group.Category, item["Item"], item["Price"]);
                }
            }

            // Привязка данных к DataGridView
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = resultTable;
        }
    }
}
