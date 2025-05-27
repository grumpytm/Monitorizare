using ComponentFactory.Krypton.Toolkit;

namespace Monitorizare
{
    partial class Vizualizare
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
            TabControl = new MetroFramework.Controls.MetroTabControl();
            metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            ComboBoxSiloz = new KryptonComboBox();
            FilterDetails = new KryptonLabel();
            ComboBoxHala = new KryptonComboBox();
            ComboBoxBuncar = new KryptonComboBox();
            PerioadaGroupBox = new GroupBox();
            RefreshButton = new Button();
            LabelInterval = new Label();
            DateTimePickerMin = new KryptonDateTimePicker();
            DateTimePickerMax = new KryptonDateTimePicker();
            FiltrareGroupBox = new GroupBox();
            LabelBuncar = new Label();
            LabelFiltrare = new Label();
            LabelSiloz = new Label();
            LabelHala = new Label();
            DataGridView = new KryptonDataGridView();
            TabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ComboBoxSiloz).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ComboBoxHala).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ComboBoxBuncar).BeginInit();
            PerioadaGroupBox.SuspendLayout();
            FiltrareGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView).BeginInit();
            SuspendLayout();
            // 
            // TabControl
            // 
            TabControl.Controls.Add(metroTabPage1);
            TabControl.Controls.Add(metroTabPage2);
            TabControl.Location = new Point(27, 73);
            TabControl.Margin = new Padding(4, 3, 4, 3);
            TabControl.Name = "TabControl";
            TabControl.Padding = new Point(6, 8);
            TabControl.SelectedIndex = 0;
            TabControl.Size = new Size(446, 44);
            TabControl.TabIndex = 0;
            TabControl.TabStop = false;
            TabControl.SelectedIndexChanged += TabControl_SelectedIndexchanged;
            // 
            // metroTabPage1
            // 
            metroTabPage1.HorizontalScrollbarBarColor = true;
            metroTabPage1.HorizontalScrollbarSize = 12;
            metroTabPage1.Location = new Point(4, 35);
            metroTabPage1.Margin = new Padding(4, 3, 4, 3);
            metroTabPage1.Name = "metroTabPage1";
            metroTabPage1.Size = new Size(438, 5);
            metroTabPage1.Style = MetroFramework.MetroColorStyle.Lime;
            metroTabPage1.TabIndex = 0;
            metroTabPage1.Text = "Incarcari";
            metroTabPage1.VerticalScrollbarBarColor = true;
            metroTabPage1.VerticalScrollbarSize = 12;
            // 
            // metroTabPage2
            // 
            metroTabPage2.HorizontalScrollbarBarColor = true;
            metroTabPage2.HorizontalScrollbarSize = 12;
            metroTabPage2.Location = new Point(4, 35);
            metroTabPage2.Margin = new Padding(4, 3, 4, 3);
            metroTabPage2.Name = "metroTabPage2";
            metroTabPage2.Size = new Size(192, 61);
            metroTabPage2.Style = MetroFramework.MetroColorStyle.Lime;
            metroTabPage2.TabIndex = 0;
            metroTabPage2.Text = "Descarcari";
            metroTabPage2.VerticalScrollbarBarColor = true;
            metroTabPage2.VerticalScrollbarSize = 12;
            // 
            // ComboBoxSiloz
            // 
            ComboBoxSiloz.DropDownWidth = 20;
            ComboBoxSiloz.Location = new Point(46, 24);
            ComboBoxSiloz.Margin = new Padding(0);
            ComboBoxSiloz.Name = "ComboBoxSiloz";
            ComboBoxSiloz.PaletteMode = PaletteMode.ProfessionalSystem;
            ComboBoxSiloz.RightToLeft = RightToLeft.No;
            ComboBoxSiloz.Size = new Size(50, 21);
            ComboBoxSiloz.TabIndex = 2;
            ComboBoxSiloz.TabStop = true;
            // 
            // FilterDetails
            // 
            FilterDetails.Location = new Point(27, 602);
            FilterDetails.Margin = new Padding(0);
            FilterDetails.Name = "FilterDetails";
            FilterDetails.RightToLeft = RightToLeft.No;
            FilterDetails.Size = new Size(29, 20);
            FilterDetails.TabIndex = 0;
            FilterDetails.TabStop = false;
            FilterDetails.Values.Text = "n/a";
            FilterDetails.Visible = false;
            // 
            // ComboBoxHala
            // 
            ComboBoxHala.DropDownWidth = 20;
            ComboBoxHala.Location = new Point(139, 24);
            ComboBoxHala.Margin = new Padding(0);
            ComboBoxHala.Name = "ComboBoxHala";
            ComboBoxHala.PaletteMode = PaletteMode.ProfessionalSystem;
            ComboBoxHala.Size = new Size(50, 21);
            ComboBoxHala.TabIndex = 3;
            ComboBoxHala.TabStop = true;
            // 
            // ComboBoxBuncar
            // 
            ComboBoxBuncar.DropDownWidth = 20;
            ComboBoxBuncar.Location = new Point(244, 24);
            ComboBoxBuncar.Margin = new Padding(0);
            ComboBoxBuncar.Name = "ComboBoxBuncar";
            ComboBoxBuncar.PaletteMode = PaletteMode.ProfessionalSystem;
            ComboBoxBuncar.Size = new Size(50, 21);
            ComboBoxBuncar.TabIndex = 4;
            ComboBoxBuncar.TabStop = true;
            // 
            // PerioadaGroupBox
            // 
            PerioadaGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            PerioadaGroupBox.BackgroundImageLayout = ImageLayout.None;
            PerioadaGroupBox.CausesValidation = false;
            PerioadaGroupBox.Controls.Add(RefreshButton);
            PerioadaGroupBox.Controls.Add(LabelInterval);
            PerioadaGroupBox.Controls.Add(DateTimePickerMin);
            PerioadaGroupBox.Controls.Add(DateTimePickerMax);
            PerioadaGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            PerioadaGroupBox.Location = new Point(27, 123);
            PerioadaGroupBox.Name = "PerioadaGroupBox";
            PerioadaGroupBox.Size = new Size(308, 62);
            PerioadaGroupBox.TabIndex = 0;
            PerioadaGroupBox.TabStop = false;
            // 
            // RefreshButton
            // 
            RefreshButton.FlatAppearance.BorderSize = 0;
            RefreshButton.FlatStyle = FlatStyle.System;
            RefreshButton.Location = new Point(221, 20);
            RefreshButton.Name = "RefreshButton";
            RefreshButton.Size = new Size(75, 29);
            RefreshButton.TabIndex = 1;
            RefreshButton.TabStop = true;
            RefreshButton.Text = "Afiseaza";
            RefreshButton.UseVisualStyleBackColor = false;
            RefreshButton.Click += RefreshButton_Click;
            // 
            // LabelInterval
            // 
            LabelInterval.AutoSize = true;
            LabelInterval.Location = new Point(9, -3);
            LabelInterval.Name = "LabelInterval";
            LabelInterval.Padding = new Padding(5);
            LabelInterval.Size = new Size(103, 25);
            LabelInterval.TabIndex = 0;
            LabelInterval.Text = "Interval de timp:";
            LabelInterval.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DateTimePickerMin
            // 
            DateTimePickerMin.CalendarCloseOnTodayClick = true;
            DateTimePickerMin.CustomFormat = "dd.MM.yyyy";
            DateTimePickerMin.Format = DateTimePickerFormat.Custom;
            DateTimePickerMin.Location = new Point(8, 25);
            DateTimePickerMin.Margin = new Padding(4, 3, 4, 3);
            DateTimePickerMin.Name = "DateTimePickerMin";
            DateTimePickerMin.PaletteMode = PaletteMode.ProfessionalOffice2003;
            DateTimePickerMin.RightToLeft = RightToLeft.No;
            DateTimePickerMin.Size = new Size(99, 21);
            DateTimePickerMin.TabIndex = 0;
            DateTimePickerMin.TabStop = false;
            // 
            // DateTimePickerMax
            // 
            DateTimePickerMax.CalendarCloseOnTodayClick = true;
            DateTimePickerMax.CustomFormat = "dd.MM.yyyy";
            DateTimePickerMax.Format = DateTimePickerFormat.Custom;
            DateTimePickerMax.Location = new Point(115, 25);
            DateTimePickerMax.Margin = new Padding(4, 3, 4, 3);
            DateTimePickerMax.Name = "DateTimePickerMax";
            DateTimePickerMax.PaletteMode = PaletteMode.ProfessionalOffice2003;
            DateTimePickerMax.Size = new Size(99, 21);
            DateTimePickerMax.TabIndex = 0;
            DateTimePickerMax.TabStop = false;
            // 
            // FiltrareGroupBox
            // 
            FiltrareGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FiltrareGroupBox.BackgroundImageLayout = ImageLayout.None;
            FiltrareGroupBox.CausesValidation = false;
            FiltrareGroupBox.Controls.Add(LabelBuncar);
            FiltrareGroupBox.Controls.Add(LabelFiltrare);
            FiltrareGroupBox.Controls.Add(LabelSiloz);
            FiltrareGroupBox.Controls.Add(LabelHala);
            FiltrareGroupBox.Controls.Add(ComboBoxSiloz);
            FiltrareGroupBox.Controls.Add(ComboBoxHala);
            FiltrareGroupBox.Controls.Add(ComboBoxBuncar);
            FiltrareGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FiltrareGroupBox.Location = new Point(27, 191);
            FiltrareGroupBox.Name = "FiltrareGroupBox";
            FiltrareGroupBox.Size = new Size(308, 62);
            FiltrareGroupBox.TabIndex = 0;
            FiltrareGroupBox.TabStop = false;
            // 
            // LabelBuncar
            // 
            LabelBuncar.Location = new Point(194, 22);
            LabelBuncar.Name = "LabelBuncar";
            LabelBuncar.Size = new Size(47, 25);
            LabelBuncar.TabIndex = 0;
            LabelBuncar.Text = "Buncar:";
            LabelBuncar.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelFiltrare
            // 
            LabelFiltrare.AutoSize = true;
            LabelFiltrare.Location = new Point(9, -3);
            LabelFiltrare.Name = "LabelFiltrare";
            LabelFiltrare.Padding = new Padding(5);
            LabelFiltrare.Size = new Size(56, 25);
            LabelFiltrare.TabIndex = 0;
            LabelFiltrare.Text = "Filtrare:";
            LabelFiltrare.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelSiloz
            // 
            LabelSiloz.Location = new Point(8, 22);
            LabelSiloz.Name = "LabelSiloz";
            LabelSiloz.Size = new Size(35, 25);
            LabelSiloz.TabIndex = 0;
            LabelSiloz.Text = "Siloz:";
            LabelSiloz.TextAlign = ContentAlignment.MiddleRight;
            // 
            // LabelHala
            // 
            LabelHala.Location = new Point(101, 22);
            LabelHala.Name = "LabelHala";
            LabelHala.Size = new Size(35, 25);
            LabelHala.TabIndex = 0;
            LabelHala.Text = "Hala:";
            LabelHala.TextAlign = ContentAlignment.MiddleRight;
            // 
            // DataGridView
            // 
            DataGridView.AllowUserToAddRows = false;
            DataGridView.AllowUserToDeleteRows = false;
            DataGridView.AllowUserToResizeColumns = false;
            DataGridView.AllowUserToResizeRows = false;
            DataGridView.ColumnHeadersHeight = 30;
            DataGridView.GridStyles.Style = DataGridViewStyle.Mixed;
            DataGridView.GridStyles.StyleColumn = GridStyle.Sheet;
            DataGridView.Location = new Point(27, 264);
            DataGridView.Name = "DataGridView";
            DataGridView.PaletteMode = PaletteMode.ProfessionalSystem;
            DataGridView.RightToLeft = RightToLeft.No;
            DataGridView.RowHeadersVisible = false;
            DataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            DataGridView.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridView.RowTemplate.ReadOnly = true;
            DataGridView.RowTemplate.Resizable = DataGridViewTriState.False;
            DataGridView.ScrollBars = ScrollBars.Vertical;
            DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridView.ShowCellErrors = false;
            DataGridView.ShowCellToolTips = false;
            DataGridView.ShowEditingIcon = false;
            DataGridView.ShowRowErrors = false;
            DataGridView.Size = new Size(400, 323);
            DataGridView.TabIndex = 0;
            DataGridView.TabStop = false;
            DataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            // 
            // Vizualizare
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = MetroFramework.Drawing.MetroBorderStyle.FixedSingle;
            ClientSize = new Size(520, 640);
            Controls.Add(DataGridView);
            Controls.Add(FiltrareGroupBox);
            Controls.Add(PerioadaGroupBox);
            Controls.Add(FilterDetails);
            Controls.Add(TabControl);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Vizualizare";
            Padding = new Padding(23, 69, 23, 23);
            Resizable = false;
            Text = "Vizualizare date";
            TabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ComboBoxSiloz).EndInit();
            ((System.ComponentModel.ISupportInitialize)ComboBoxHala).EndInit();
            ((System.ComponentModel.ISupportInitialize)ComboBoxBuncar).EndInit();
            PerioadaGroupBox.ResumeLayout(false);
            PerioadaGroupBox.PerformLayout();
            FiltrareGroupBox.ResumeLayout(false);
            FiltrareGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl TabControl;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private MetroFramework.Controls.MetroTabPage metroTabPage2;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox ComboBoxSiloz;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel FilterDetails;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox ComboBoxHala;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox ComboBoxBuncar;
        private GroupBox FiltrareGroupBox;
        private GroupBox PerioadaGroupBox;
        private Button RefreshButton;
        private Label LabelInterval;
        private Label LabelFiltrare;
        private Label LabelSiloz;
        private Label LabelBuncar;
        private Label LabelHala;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView DataGridView;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePickerMax;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePickerMin;
    }
}