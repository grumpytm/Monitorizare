namespace Monitorizare
{
    partial class Migrations
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridView = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)DataGridView).BeginInit();
            SuspendLayout();
            // 
            // DataGridView
            // 
            DataGridView.AllowUserToAddRows = false;
            DataGridView.AllowUserToDeleteRows = false;
            DataGridView.AllowUserToResizeColumns = false;
            DataGridView.AllowUserToResizeRows = false;
            DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            DataGridView.ColumnHeadersHeight = 30;
            DataGridView.GridStyles.Style = ComponentFactory.Krypton.Toolkit.DataGridViewStyle.Mixed;
            DataGridView.GridStyles.StyleColumn = ComponentFactory.Krypton.Toolkit.GridStyle.Sheet;
            DataGridView.Location = new Point(26, 72);
            DataGridView.MultiSelect = true;
            DataGridView.Name = "DataGridView";
            DataGridView.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            DataGridView.RightToLeft = RightToLeft.No;
            DataGridView.RowHeadersVisible = false;
            DataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DataGridView.RowTemplate.ReadOnly = true;
            DataGridView.RowTemplate.Resizable = DataGridViewTriState.False;
            DataGridView.ScrollBars = ScrollBars.Vertical;
            DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridView.ShowCellErrors = false;
            DataGridView.ShowCellToolTips = false;
            DataGridView.ShowEditingIcon = false;
            DataGridView.ShowRowErrors = false;
            DataGridView.Size = new Size(451, 280);
            DataGridView.TabIndex = 1;
            DataGridView.TabStop = false;
            // 
            // Migrations
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 450);
            Controls.Add(DataGridView);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Migrations";
            Padding = new Padding(23, 69, 23, 23);
            Resizable = false;
            Text = "Migratii";
            ((System.ComponentModel.ISupportInitialize)DataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView DataGridView;
    }
}