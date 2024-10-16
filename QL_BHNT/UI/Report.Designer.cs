namespace QL_BHNT.UI
{
    partial class Report
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart_TKSL = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.list_TKSL = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btn_TK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_TKSL_Năm = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbo_TKSL_Thang = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart_TKSL)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart_TKSL
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_TKSL.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart_TKSL.Legends.Add(legend2);
            this.chart_TKSL.Location = new System.Drawing.Point(34, 42);
            this.chart_TKSL.Name = "chart_TKSL";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart_TKSL.Series.Add(series2);
            this.chart_TKSL.Size = new System.Drawing.Size(437, 210);
            this.chart_TKSL.TabIndex = 0;
            this.chart_TKSL.Text = "chart1";
            // 
            // list_TKSL
            // 
            this.list_TKSL.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.list_TKSL.GridLines = true;
            this.list_TKSL.HideSelection = false;
            this.list_TKSL.Location = new System.Drawing.Point(36, 42);
            this.list_TKSL.Name = "list_TKSL";
            this.list_TKSL.Size = new System.Drawing.Size(414, 139);
            this.list_TKSL.TabIndex = 1;
            this.list_TKSL.UseCompatibleStateImageBehavior = false;
            this.list_TKSL.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Loại Mã Số";
            this.columnHeader1.Width = 161;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Số Lượng";
            this.columnHeader2.Width = 162;
            // 
            // btn_TK
            // 
            this.btn_TK.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btn_TK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_TK.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btn_TK.Location = new System.Drawing.Point(163, 326);
            this.btn_TK.Name = "btn_TK";
            this.btn_TK.Size = new System.Drawing.Size(178, 41);
            this.btn_TK.TabIndex = 2;
            this.btn_TK.Text = "Thống Kê";
            this.btn_TK.UseVisualStyleBackColor = false;
            this.btn_TK.Click += new System.EventHandler(this.btn_TK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.list_TKSL);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.groupBox1.Location = new System.Drawing.Point(548, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(554, 224);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Danh Sách Thống Kê";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_TKSL_Năm);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbo_TKSL_Thang);
            this.groupBox2.Controls.Add(this.btn_TK);
            this.groupBox2.Controls.Add(this.chart_TKSL);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Highlight;
            this.groupBox2.Location = new System.Drawing.Point(12, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(505, 384);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sơ Đồ";
            // 
            // txt_TKSL_Năm
            // 
            this.txt_TKSL_Năm.Location = new System.Drawing.Point(325, 276);
            this.txt_TKSL_Năm.Name = "txt_TKSL_Năm";
            this.txt_TKSL_Năm.Size = new System.Drawing.Size(130, 26);
            this.txt_TKSL_Năm.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Năm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tháng";
            // 
            // cbo_TKSL_Thang
            // 
            this.cbo_TKSL_Thang.FormattingEnabled = true;
            this.cbo_TKSL_Thang.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cbo_TKSL_Thang.Location = new System.Drawing.Point(114, 276);
            this.cbo_TKSL_Thang.Name = "cbo_TKSL_Thang";
            this.cbo_TKSL_Thang.Size = new System.Drawing.Size(121, 28);
            this.cbo_TKSL_Thang.TabIndex = 3;
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1196, 584);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "Report";
            this.Text = "Report";
            ((System.ComponentModel.ISupportInitialize)(this.chart_TKSL)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_TKSL;
        private System.Windows.Forms.ListView list_TKSL;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btn_TK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txt_TKSL_Năm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbo_TKSL_Thang;
    }
}