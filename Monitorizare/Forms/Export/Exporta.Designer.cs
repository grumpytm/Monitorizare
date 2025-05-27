namespace Monitorizare
{
    partial class Exporta
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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            ComboBoxExportType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            PerioadaGroupBox = new GroupBox();
            DataGridViewIncarcare = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            LabelLoading = new Label();
            ButtonExport = new Button();
            groupBox1 = new GroupBox();
            DataGridViewDescarcare = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            LabelUnloading = new Label();
            Export = new GroupBox();
            LabelExportType = new Label();
            TimeLoading = new GroupBox();
            LabelTimeFrameLoading = new Label();
            DateTimePickerLoadingAfter = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            DateTimePickerLoadingBefore = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            LabelTimeFrameUnloading = new Label();
            TimeUnloading = new GroupBox();
            DateTimePickerUnloadingAfter = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            DateTimePickerUnloadingBefore = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            ((System.ComponentModel.ISupportInitialize)ComboBoxExportType).BeginInit();
            PerioadaGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewIncarcare).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewDescarcare).BeginInit();
            Export.SuspendLayout();
            TimeLoading.SuspendLayout();
            TimeUnloading.SuspendLayout();
            SuspendLayout();
            // 
            // ComboBoxExportType
            // 
            ComboBoxExportType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxExportType.DropDownWidth = 20;
            ComboBoxExportType.Location = new Point(10, 29);
            ComboBoxExportType.Margin = new Padding(0);
            ComboBoxExportType.Name = "ComboBoxExportType";
            ComboBoxExportType.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            ComboBoxExportType.RightToLeft = RightToLeft.No;
            ComboBoxExportType.Size = new Size(100, 21);
            ComboBoxExportType.TabIndex = 0;
            ComboBoxExportType.TabStop = false;
            // 
            // PerioadaGroupBox
            // 
            PerioadaGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            PerioadaGroupBox.BackgroundImageLayout = ImageLayout.None;
            PerioadaGroupBox.CausesValidation = false;
            PerioadaGroupBox.Controls.Add(DataGridViewIncarcare);
            PerioadaGroupBox.Controls.Add(LabelLoading);
            PerioadaGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            PerioadaGroupBox.Location = new Point(26, 72);
            PerioadaGroupBox.Name = "PerioadaGroupBox";
            PerioadaGroupBox.Size = new Size(366, 371);
            PerioadaGroupBox.TabIndex = 1;
            PerioadaGroupBox.TabStop = false;
            // 
            // DataGridViewIncarcare
            // 
            DataGridViewIncarcare.AllowUserToAddRows = false;
            DataGridViewIncarcare.AllowUserToDeleteRows = false;
            DataGridViewIncarcare.AllowUserToResizeColumns = false;
            DataGridViewIncarcare.AllowUserToResizeRows = false;
            DataGridViewIncarcare.ColumnHeadersHeight = 30;
            DataGridViewIncarcare.GridStyles.Style = ComponentFactory.Krypton.Toolkit.DataGridViewStyle.Mixed;
            DataGridViewIncarcare.GridStyles.StyleColumn = ComponentFactory.Krypton.Toolkit.GridStyle.Sheet;
            DataGridViewIncarcare.Location = new Point(13, 24);
            DataGridViewIncarcare.Name = "DataGridViewIncarcare";
            DataGridViewIncarcare.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            DataGridViewIncarcare.RightToLeft = RightToLeft.No;
            DataGridViewIncarcare.RowHeadersVisible = false;
            DataGridViewIncarcare.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewIncarcare.RowsDefaultCellStyle = dataGridViewCellStyle1;
            DataGridViewIncarcare.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewIncarcare.RowTemplate.ReadOnly = true;
            DataGridViewIncarcare.RowTemplate.Resizable = DataGridViewTriState.False;
            DataGridViewIncarcare.ScrollBars = ScrollBars.Vertical;
            DataGridViewIncarcare.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridViewIncarcare.ShowCellErrors = false;
            DataGridViewIncarcare.ShowCellToolTips = false;
            DataGridViewIncarcare.ShowEditingIcon = false;
            DataGridViewIncarcare.ShowRowErrors = false;
            DataGridViewIncarcare.Size = new Size(345, 330);
            DataGridViewIncarcare.TabIndex = 1;
            DataGridViewIncarcare.TabStop = false;
            DataGridViewIncarcare.ColumnHeaderMouseClick += DataGridViewIncarcare_ColumnHeaderMouseClick;
            // 
            // LabelLoading
            // 
            LabelLoading.AutoSize = true;
            LabelLoading.Location = new Point(10, -4);
            LabelLoading.Name = "LabelLoading";
            LabelLoading.Padding = new Padding(5);
            LabelLoading.Size = new Size(133, 25);
            LabelLoading.TabIndex = 0;
            LabelLoading.Text = "Ultima zi de incarcare:";
            LabelLoading.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ButtonExport
            // 
            ButtonExport.FlatAppearance.BorderSize = 0;
            ButtonExport.FlatStyle = FlatStyle.System;
            ButtonExport.Location = new Point(815, 476);
            ButtonExport.Name = "ButtonExport";
            ButtonExport.Size = new Size(75, 29);
            ButtonExport.TabIndex = 0;
            ButtonExport.TabStop = false;
            ButtonExport.Text = "Exporta";
            ButtonExport.UseVisualStyleBackColor = false;
            ButtonExport.Click += ExportButton_Click;
            // 
            // groupBox1
            // 
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.BackgroundImageLayout = ImageLayout.None;
            groupBox1.CausesValidation = false;
            groupBox1.Controls.Add(DataGridViewDescarcare);
            groupBox1.Controls.Add(LabelUnloading);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            groupBox1.Location = new Point(404, 72);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(501, 371);
            groupBox1.TabIndex = 89;
            groupBox1.TabStop = false;
            // 
            // DataGridViewDescarcare
            // 
            DataGridViewDescarcare.AllowUserToAddRows = false;
            DataGridViewDescarcare.AllowUserToDeleteRows = false;
            DataGridViewDescarcare.AllowUserToResizeColumns = false;
            DataGridViewDescarcare.AllowUserToResizeRows = false;
            DataGridViewDescarcare.ColumnHeadersHeight = 30;
            DataGridViewDescarcare.GridStyles.Style = ComponentFactory.Krypton.Toolkit.DataGridViewStyle.Mixed;
            DataGridViewDescarcare.GridStyles.StyleColumn = ComponentFactory.Krypton.Toolkit.GridStyle.Sheet;
            DataGridViewDescarcare.Location = new Point(13, 24);
            DataGridViewDescarcare.Name = "DataGridViewDescarcare";
            DataGridViewDescarcare.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            DataGridViewDescarcare.RightToLeft = RightToLeft.No;
            DataGridViewDescarcare.RowHeadersVisible = false;
            DataGridViewDescarcare.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewDescarcare.RowsDefaultCellStyle = dataGridViewCellStyle2;
            DataGridViewDescarcare.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewDescarcare.RowTemplate.ReadOnly = true;
            DataGridViewDescarcare.RowTemplate.Resizable = DataGridViewTriState.False;
            DataGridViewDescarcare.ScrollBars = ScrollBars.Vertical;
            DataGridViewDescarcare.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridViewDescarcare.ShowCellErrors = false;
            DataGridViewDescarcare.ShowCellToolTips = false;
            DataGridViewDescarcare.ShowEditingIcon = false;
            DataGridViewDescarcare.ShowRowErrors = false;
            DataGridViewDescarcare.Size = new Size(475, 330);
            DataGridViewDescarcare.TabIndex = 2;
            DataGridViewDescarcare.TabStop = false;
            DataGridViewDescarcare.ColumnHeaderMouseClick += DataGridViewDescarcare_ColumnHeaderMouseClick;
            // 
            // LabelUnloading
            // 
            LabelUnloading.AutoSize = true;
            LabelUnloading.Location = new Point(10, -4);
            LabelUnloading.Name = "LabelUnloading";
            LabelUnloading.Padding = new Padding(5);
            LabelUnloading.Size = new Size(141, 25);
            LabelUnloading.TabIndex = 0;
            LabelUnloading.Text = "Ultima zi de descarcare:";
            LabelUnloading.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Export
            // 
            Export.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Export.BackgroundImageLayout = ImageLayout.None;
            Export.CausesValidation = false;
            Export.Controls.Add(ComboBoxExportType);
            Export.Controls.Add(LabelExportType);
            Export.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Export.Location = new Point(26, 450);
            Export.Name = "Export";
            Export.Size = new Size(119, 64);
            Export.TabIndex = 91;
            Export.TabStop = false;
            // 
            // LabelExportType
            // 
            LabelExportType.AutoSize = true;
            LabelExportType.Location = new Point(5, -1);
            LabelExportType.Name = "LabelExportType";
            LabelExportType.Padding = new Padding(5);
            LabelExportType.Size = new Size(99, 25);
            LabelExportType.TabIndex = 0;
            LabelExportType.Text = "Tipul exportarii:";
            LabelExportType.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TimeLoading
            // 
            TimeLoading.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TimeLoading.BackgroundImageLayout = ImageLayout.None;
            TimeLoading.CausesValidation = false;
            TimeLoading.Controls.Add(LabelTimeFrameLoading);
            TimeLoading.Controls.Add(DateTimePickerLoadingAfter);
            TimeLoading.Controls.Add(DateTimePickerLoadingBefore);
            TimeLoading.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            TimeLoading.Location = new Point(150, 450);
            TimeLoading.Name = "TimeLoading";
            TimeLoading.Size = new Size(182, 64);
            TimeLoading.TabIndex = 92;
            TimeLoading.TabStop = false;
            TimeLoading.Visible = false;
            // 
            // LabelTimeFrameLoading
            // 
            LabelTimeFrameLoading.AutoSize = true;
            LabelTimeFrameLoading.Location = new Point(5, 0);
            LabelTimeFrameLoading.Name = "LabelTimeFrameLoading";
            LabelTimeFrameLoading.Padding = new Padding(5);
            LabelTimeFrameLoading.Size = new Size(154, 25);
            LabelTimeFrameLoading.TabIndex = 0;
            LabelTimeFrameLoading.Text = "Interval de timp incarcare:";
            LabelTimeFrameLoading.TextAlign = ContentAlignment.MiddleCenter;
            LabelTimeFrameLoading.Visible = false;
            // 
            // DateTimePickerLoadingAfter
            // 
            DateTimePickerLoadingAfter.CalendarCloseOnTodayClick = true;
            DateTimePickerLoadingAfter.CustomFormat = "dd.MM.yyyy";
            DateTimePickerLoadingAfter.Format = DateTimePickerFormat.Custom;
            DateTimePickerLoadingAfter.Location = new Point(95, 29);
            DateTimePickerLoadingAfter.Margin = new Padding(4, 3, 4, 3);
            DateTimePickerLoadingAfter.Name = "DateTimePickerLoadingAfter";
            DateTimePickerLoadingAfter.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            DateTimePickerLoadingAfter.Size = new Size(80, 21);
            DateTimePickerLoadingAfter.TabIndex = 94;
            DateTimePickerLoadingAfter.TabStop = false;
            // 
            // DateTimePickerLoadingBefore
            // 
            DateTimePickerLoadingBefore.CalendarCloseOnTodayClick = true;
            DateTimePickerLoadingBefore.CustomFormat = "dd.MM.yyyy";
            DateTimePickerLoadingBefore.Format = DateTimePickerFormat.Custom;
            DateTimePickerLoadingBefore.Location = new Point(7, 29);
            DateTimePickerLoadingBefore.Margin = new Padding(4, 3, 4, 3);
            DateTimePickerLoadingBefore.Name = "DateTimePickerLoadingBefore";
            DateTimePickerLoadingBefore.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            DateTimePickerLoadingBefore.RightToLeft = RightToLeft.No;
            DateTimePickerLoadingBefore.Size = new Size(80, 21);
            DateTimePickerLoadingBefore.TabIndex = 93;
            DateTimePickerLoadingBefore.TabStop = false;
            // 
            // LabelTimeFrameUnloading
            // 
            LabelTimeFrameUnloading.AutoSize = true;
            LabelTimeFrameUnloading.Location = new Point(5, 0);
            LabelTimeFrameUnloading.Name = "LabelTimeFrameUnloading";
            LabelTimeFrameUnloading.Padding = new Padding(5);
            LabelTimeFrameUnloading.Size = new Size(162, 25);
            LabelTimeFrameUnloading.TabIndex = 0;
            LabelTimeFrameUnloading.Text = "Interval de timp descarcare:";
            LabelTimeFrameUnloading.TextAlign = ContentAlignment.MiddleCenter;
            LabelTimeFrameUnloading.Visible = false;
            // 
            // TimeUnloading
            // 
            TimeUnloading.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TimeUnloading.BackgroundImageLayout = ImageLayout.None;
            TimeUnloading.CausesValidation = false;
            TimeUnloading.Controls.Add(DateTimePickerUnloadingAfter);
            TimeUnloading.Controls.Add(LabelTimeFrameUnloading);
            TimeUnloading.Controls.Add(DateTimePickerUnloadingBefore);
            TimeUnloading.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            TimeUnloading.Location = new Point(340, 450);
            TimeUnloading.Name = "TimeUnloading";
            TimeUnloading.Size = new Size(184, 64);
            TimeUnloading.TabIndex = 95;
            TimeUnloading.TabStop = false;
            TimeUnloading.Visible = false;
            // 
            // DateTimePickerUnloadingAfter
            // 
            DateTimePickerUnloadingAfter.CalendarCloseOnTodayClick = true;
            DateTimePickerUnloadingAfter.CustomFormat = "dd.MM.yyyy";
            DateTimePickerUnloadingAfter.Format = DateTimePickerFormat.Custom;
            DateTimePickerUnloadingAfter.Location = new Point(95, 29);
            DateTimePickerUnloadingAfter.Margin = new Padding(4, 3, 4, 3);
            DateTimePickerUnloadingAfter.Name = "DateTimePickerUnloadingAfter";
            DateTimePickerUnloadingAfter.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            DateTimePickerUnloadingAfter.Size = new Size(80, 21);
            DateTimePickerUnloadingAfter.TabIndex = 94;
            DateTimePickerUnloadingAfter.TabStop = false;
            // 
            // DateTimePickerUnloadingBefore
            // 
            DateTimePickerUnloadingBefore.CalendarCloseOnTodayClick = true;
            DateTimePickerUnloadingBefore.CustomFormat = "dd.MM.yyyy";
            DateTimePickerUnloadingBefore.Format = DateTimePickerFormat.Custom;
            DateTimePickerUnloadingBefore.Location = new Point(7, 29);
            DateTimePickerUnloadingBefore.Margin = new Padding(4, 3, 4, 3);
            DateTimePickerUnloadingBefore.Name = "DateTimePickerUnloadingBefore";
            DateTimePickerUnloadingBefore.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalOffice2003;
            DateTimePickerUnloadingBefore.RightToLeft = RightToLeft.No;
            DateTimePickerUnloadingBefore.Size = new Size(80, 21);
            DateTimePickerUnloadingBefore.TabIndex = 93;
            DateTimePickerUnloadingBefore.TabStop = false;
            // 
            // Exporta
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(915, 525);
            Controls.Add(TimeUnloading);
            Controls.Add(TimeLoading);
            Controls.Add(Export);
            Controls.Add(groupBox1);
            Controls.Add(ButtonExport);
            Controls.Add(PerioadaGroupBox);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Exporta";
            Padding = new Padding(23, 69, 23, 23);
            Resizable = false;
            ShadowType = MetroFormShadowType.Flat;
            Text = "Exportare date";
            ((System.ComponentModel.ISupportInitialize)ComboBoxExportType).EndInit();
            PerioadaGroupBox.ResumeLayout(false);
            PerioadaGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewIncarcare).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewDescarcare).EndInit();
            Export.ResumeLayout(false);
            Export.PerformLayout();
            TimeLoading.ResumeLayout(false);
            TimeLoading.PerformLayout();
            TimeUnloading.ResumeLayout(false);
            TimeUnloading.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private GroupBox PerioadaGroupBox;
        private Label LabelLoading;
        private Button ButtonExport;
        private GroupBox groupBox1;
        private Label LabelUnloading;
        private GroupBox Export;
        private Label LabelExportType;
        private GroupBox TimeLoading;
        private Label LabelTimeFrameLoading;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePickerLoadingBefore;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePickerLoadingAfter;
        private Label LabelTimeFrameUnloading;
        private GroupBox TimeUnloading;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePickerUnloadingBefore;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePickerUnloadingAfter;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView DataGridViewIncarcare;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView DataGridViewDescarcare;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox ComboBoxExportType;
    }
}