namespace Monitor
{
    partial class ShowItemRest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ItemInProcess = new SmartTrade.UserControls.ExtDGVcs();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInProcess)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemInProcess
            // 
            this.ItemInProcess.AllowUserToAddRows = false;
            this.ItemInProcess.AllowUserToDeleteRows = false;
            this.ItemInProcess.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ItemInProcess.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ItemInProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Green;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemInProcess.DefaultCellStyle = dataGridViewCellStyle2;
            this.ItemInProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemInProcess.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ItemInProcess.GridColor = System.Drawing.Color.Black;
            this.ItemInProcess.Location = new System.Drawing.Point(0, 0);
            this.ItemInProcess.Margin = new System.Windows.Forms.Padding(0);
            this.ItemInProcess.Name = "ItemInProcess";
            this.ItemInProcess.PermitNumber = 0;
            this.ItemInProcess.ReadOnly = true;
            this.ItemInProcess.RowHeadersVisible = false;
            this.ItemInProcess.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ItemInProcess.ShowContextMenu = true;
            this.ItemInProcess.Size = new System.Drawing.Size(661, 420);
            this.ItemInProcess.TabIndex = 0;
            // 
            // ShowItemRest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 420);
            this.Controls.Add(this.ItemInProcess);
            this.Name = "ShowItemRest";
            this.Text = "Остаток товара ";
            ((System.ComponentModel.ISupportInitialize)(this.ItemInProcess)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SmartTrade.UserControls.ExtDGVcs ItemInProcess;
    }
}