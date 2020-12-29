using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartTrade
{
    public partial class FrmInputBox : Form
    {
        public FrmInputBox()
        {
            InitializeComponent();
        }

        public FrmInputBox(string text, string caption, string message, bool isPassword)
            : this()
        {
            txtInput.Text = text;
            Text = caption;
            lblMessage.Text = message;
            if (isPassword)
            {
                txtInput.PasswordChar = '*';
            }
            else
            {
                txtInput.PasswordChar = (char)0;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            value = txtInput.Lines;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            value = txtInput.Lines;
            Close();
        }

        private string[] value;
        public string[] Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }

    static public class InputBox
    {
        static public DialogResult Show(string text, string caption, string message, out string[] output)
        {
            FrmInputBox frmInputBox = new FrmInputBox(text, caption, message,false);
            DialogResult dr = frmInputBox.ShowDialog();
            output = frmInputBox.Value;
            return dr;
        }
        static public DialogResult Show(string caption, string message, out string[] output)
        {
             return Show(String.Empty, caption, message,out output);
        }

        static public DialogResult ShowPassword(string caption, string message, out string[] output)
        {
            FrmInputBox frmInputBox = new FrmInputBox("", caption, message,true);
            DialogResult dr = frmInputBox.ShowDialog();
            output = frmInputBox.Value;
            return dr;
        }
    }
}