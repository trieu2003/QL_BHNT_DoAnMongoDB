namespace QL_BHNT.UI
{
    partial class Customer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.datetime_ngaysinh = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_diachi = new System.Windows.Forms.TextBox();
            this.txt_email = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_sdt = new System.Windows.Forms.TextBox();
            this.txt_hoten = new System.Windows.Forms.TextBox();
            this.radio_Nu = new System.Windows.Forms.RadioButton();
            this.radio_nam = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_makh = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listKH = new System.Windows.Forms.ListView();
            this.MaKh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_lammoi = new System.Windows.Forms.Button();
            this.btn_xoaKH = new System.Windows.Forms.Button();
            this.btn_sua = new System.Windows.Forms.Button();
            this.btn_them = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radio_HovaTen = new System.Windows.Forms.RadioButton();
            this.btn_timkiem_kh = new System.Windows.Forms.Button();
            this.radio_MaKH = new System.Windows.Forms.RadioButton();
            this.btn_thoat = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox_MaDL = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.datetime_ngaysinh);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.txt_hoten);
            this.groupBox1.Controls.Add(this.radio_Nu);
            this.groupBox1.Controls.Add(this.radio_nam);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_makh);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.groupBox1.Location = new System.Drawing.Point(63, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(725, 406);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông Tin Khach Hàng";
            // 
            // datetime_ngaysinh
            // 
            this.datetime_ngaysinh.Location = new System.Drawing.Point(146, 109);
            this.datetime_ngaysinh.Name = "datetime_ngaysinh";
            this.datetime_ngaysinh.Size = new System.Drawing.Size(188, 26);
            this.datetime_ngaysinh.TabIndex = 11;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_MaDL);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txt_diachi);
            this.groupBox2.Controls.Add(this.txt_email);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txt_sdt);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.groupBox2.Location = new System.Drawing.Point(34, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(665, 206);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Thông tin liên lạc";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(52, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "Địa Chỉ";
            // 
            // txt_diachi
            // 
            this.txt_diachi.Location = new System.Drawing.Point(125, 92);
            this.txt_diachi.Multiline = true;
            this.txt_diachi.Name = "txt_diachi";
            this.txt_diachi.Size = new System.Drawing.Size(495, 36);
            this.txt_diachi.TabIndex = 16;
            // 
            // txt_email
            // 
            this.txt_email.Location = new System.Drawing.Point(388, 31);
            this.txt_email.Multiline = true;
            this.txt_email.Name = "txt_email";
            this.txt_email.Size = new System.Drawing.Size(232, 36);
            this.txt_email.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(324, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "Email:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(52, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 20);
            this.label6.TabIndex = 11;
            this.label6.Text = "SDT";
            // 
            // txt_sdt
            // 
            this.txt_sdt.Location = new System.Drawing.Point(121, 31);
            this.txt_sdt.Multiline = true;
            this.txt_sdt.Name = "txt_sdt";
            this.txt_sdt.Size = new System.Drawing.Size(188, 36);
            this.txt_sdt.TabIndex = 12;
            // 
            // txt_hoten
            // 
            this.txt_hoten.Location = new System.Drawing.Point(491, 42);
            this.txt_hoten.Multiline = true;
            this.txt_hoten.Name = "txt_hoten";
            this.txt_hoten.Size = new System.Drawing.Size(188, 36);
            this.txt_hoten.TabIndex = 9;
            // 
            // radio_Nu
            // 
            this.radio_Nu.AutoSize = true;
            this.radio_Nu.Location = new System.Drawing.Point(623, 116);
            this.radio_Nu.Name = "radio_Nu";
            this.radio_Nu.Size = new System.Drawing.Size(56, 24);
            this.radio_Nu.TabIndex = 8;
            this.radio_Nu.TabStop = true;
            this.radio_Nu.Text = "Nữ";
            this.radio_Nu.UseVisualStyleBackColor = true;
            // 
            // radio_nam
            // 
            this.radio_nam.AutoSize = true;
            this.radio_nam.Location = new System.Drawing.Point(500, 114);
            this.radio_nam.Name = "radio_nam";
            this.radio_nam.Size = new System.Drawing.Size(70, 24);
            this.radio_nam.TabIndex = 7;
            this.radio_nam.TabStop = true;
            this.radio_nam.Text = "Nam";
            this.radio_nam.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(391, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Giới Tính";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(381, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Họ và Tên";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ngày Sinh :";
            // 
            // txt_makh
            // 
            this.txt_makh.Location = new System.Drawing.Point(146, 39);
            this.txt_makh.Multiline = true;
            this.txt_makh.Name = "txt_makh";
            this.txt_makh.Size = new System.Drawing.Size(188, 36);
            this.txt_makh.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mã KH:";
            // 
            // listKH
            // 
            this.listKH.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.MaKh,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listKH.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.listKH.GridLines = true;
            this.listKH.HideSelection = false;
            this.listKH.Location = new System.Drawing.Point(32, 37);
            this.listKH.Name = "listKH";
            this.listKH.Size = new System.Drawing.Size(699, 123);
            this.listKH.TabIndex = 1;
            this.listKH.UseCompatibleStateImageBehavior = false;
            this.listKH.View = System.Windows.Forms.View.Details;
            this.listKH.SelectedIndexChanged += new System.EventHandler(this.listKH_SelectedIndexChanged);
            // 
            // MaKh
            // 
            this.MaKh.Text = "Mã KH";
            this.MaKh.Width = 123;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Họ Và Tên";
            this.columnHeader1.Width = 157;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Ngày Sinh";
            this.columnHeader2.Width = 155;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Giới Tính";
            this.columnHeader3.Width = 213;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listKH);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.groupBox3.Location = new System.Drawing.Point(237, 543);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(753, 180);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Danh Sách Khách Hàng";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label8.Location = new System.Drawing.Point(477, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(348, 32);
            this.label8.TabIndex = 12;
            this.label8.Text = "QUẢN LÝ KHÁCH HÀNG";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_lammoi);
            this.groupBox4.Controls.Add(this.btn_xoaKH);
            this.groupBox4.Controls.Add(this.btn_sua);
            this.groupBox4.Controls.Add(this.btn_them);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.groupBox4.Location = new System.Drawing.Point(817, 114);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(395, 354);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Chức năng";
            // 
            // btn_lammoi
            // 
            this.btn_lammoi.Location = new System.Drawing.Point(206, 261);
            this.btn_lammoi.Name = "btn_lammoi";
            this.btn_lammoi.Size = new System.Drawing.Size(118, 38);
            this.btn_lammoi.TabIndex = 6;
            this.btn_lammoi.Text = "Làm Mới";
            this.btn_lammoi.UseVisualStyleBackColor = true;
            this.btn_lammoi.Click += new System.EventHandler(this.btn_lammoi_Click);
            // 
            // btn_xoaKH
            // 
            this.btn_xoaKH.Location = new System.Drawing.Point(38, 259);
            this.btn_xoaKH.Name = "btn_xoaKH";
            this.btn_xoaKH.Size = new System.Drawing.Size(118, 38);
            this.btn_xoaKH.TabIndex = 5;
            this.btn_xoaKH.Text = "Xoá ";
            this.btn_xoaKH.UseVisualStyleBackColor = true;
            this.btn_xoaKH.Click += new System.EventHandler(this.btn_xoaKH_Click);
            // 
            // btn_sua
            // 
            this.btn_sua.Location = new System.Drawing.Point(206, 197);
            this.btn_sua.Name = "btn_sua";
            this.btn_sua.Size = new System.Drawing.Size(118, 38);
            this.btn_sua.TabIndex = 4;
            this.btn_sua.Text = "Sửa";
            this.btn_sua.UseVisualStyleBackColor = true;
            this.btn_sua.Click += new System.EventHandler(this.btn_sua_Click);
            // 
            // btn_them
            // 
            this.btn_them.Location = new System.Drawing.Point(38, 194);
            this.btn_them.Name = "btn_them";
            this.btn_them.Size = new System.Drawing.Size(118, 38);
            this.btn_them.TabIndex = 3;
            this.btn_them.Text = "Thêm";
            this.btn_them.UseVisualStyleBackColor = true;
            this.btn_them.Click += new System.EventHandler(this.btn_them_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radio_HovaTen);
            this.groupBox5.Controls.Add(this.btn_timkiem_kh);
            this.groupBox5.Controls.Add(this.radio_MaKH);
            this.groupBox5.Location = new System.Drawing.Point(49, 33);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(296, 117);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Tìm Kiếm";
            // 
            // radio_HovaTen
            // 
            this.radio_HovaTen.AutoSize = true;
            this.radio_HovaTen.Location = new System.Drawing.Point(147, 27);
            this.radio_HovaTen.Name = "radio_HovaTen";
            this.radio_HovaTen.Size = new System.Drawing.Size(115, 24);
            this.radio_HovaTen.TabIndex = 1;
            this.radio_HovaTen.TabStop = true;
            this.radio_HovaTen.Text = "Họ và Tên";
            this.radio_HovaTen.UseVisualStyleBackColor = true;
            this.radio_HovaTen.CheckedChanged += new System.EventHandler(this.radio_HovaTen_CheckedChanged);
            // 
            // btn_timkiem_kh
            // 
            this.btn_timkiem_kh.Location = new System.Drawing.Point(79, 64);
            this.btn_timkiem_kh.Name = "btn_timkiem_kh";
            this.btn_timkiem_kh.Size = new System.Drawing.Size(118, 38);
            this.btn_timkiem_kh.TabIndex = 2;
            this.btn_timkiem_kh.Text = "Tìm Kiếm";
            this.btn_timkiem_kh.UseVisualStyleBackColor = true;
            this.btn_timkiem_kh.Click += new System.EventHandler(this.btn_timkiem_kh_Click);
            // 
            // radio_MaKH
            // 
            this.radio_MaKH.AutoSize = true;
            this.radio_MaKH.Location = new System.Drawing.Point(20, 27);
            this.radio_MaKH.Name = "radio_MaKH";
            this.radio_MaKH.Size = new System.Drawing.Size(87, 24);
            this.radio_MaKH.TabIndex = 0;
            this.radio_MaKH.TabStop = true;
            this.radio_MaKH.Text = "Mã KH";
            this.radio_MaKH.UseVisualStyleBackColor = true;
            this.radio_MaKH.CheckedChanged += new System.EventHandler(this.radio_MaKH_CheckedChanged);
            // 
            // btn_thoat
            // 
            this.btn_thoat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_thoat.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_thoat.Location = new System.Drawing.Point(1118, 685);
            this.btn_thoat.Name = "btn_thoat";
            this.btn_thoat.Size = new System.Drawing.Size(118, 38);
            this.btn_thoat.TabIndex = 7;
            this.btn_thoat.Text = "Thoát";
            this.btn_thoat.UseVisualStyleBackColor = true;
            this.btn_thoat.Click += new System.EventHandler(this.btn_thoat_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 153);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 20);
            this.label9.TabIndex = 17;
            this.label9.Text = "Mã Đại Lý";
            // 
            // comboBox_MaDL
            // 
            this.comboBox_MaDL.FormattingEnabled = true;
            this.comboBox_MaDL.Location = new System.Drawing.Point(126, 153);
            this.comboBox_MaDL.Name = "comboBox_MaDL";
            this.comboBox_MaDL.Size = new System.Drawing.Size(494, 28);
            this.comboBox_MaDL.TabIndex = 18;
            // 
            // Customer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 735);
            this.Controls.Add(this.btn_thoat);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "Customer";
            this.Text = "Customer";
            this.Load += new System.EventHandler(this.Customer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_makh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_hoten;
        private System.Windows.Forms.RadioButton radio_Nu;
        private System.Windows.Forms.RadioButton radio_nam;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_diachi;
        private System.Windows.Forms.TextBox txt_email;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_sdt;
        private System.Windows.Forms.ListView listKH;
        private System.Windows.Forms.ColumnHeader MaKh;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.DateTimePicker datetime_ngaysinh;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_timkiem_kh;
        private System.Windows.Forms.RadioButton radio_HovaTen;
        private System.Windows.Forms.RadioButton radio_MaKH;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btn_them;
        private System.Windows.Forms.Button btn_lammoi;
        private System.Windows.Forms.Button btn_xoaKH;
        private System.Windows.Forms.Button btn_sua;
        private System.Windows.Forms.Button btn_thoat;
        private System.Windows.Forms.ComboBox comboBox_MaDL;
        private System.Windows.Forms.Label label9;
    }
}