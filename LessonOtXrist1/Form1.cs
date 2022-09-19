using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LessonOtXrist1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {            
           DialogResult result =  MessageBox.Show( "Текст сообщения", "Заголовок",MessageBoxButtons.YesNo,MessageBoxIcon.Asterisk);

            if (result== DialogResult.Yes)
            {
                MessageBox.Show("Нажали ДА");
            }
            if (result == DialogResult.No)
            {
                MessageBox.Show("Нажали НЕТ");
            }

        }
    }
}
