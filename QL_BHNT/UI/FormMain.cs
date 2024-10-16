using QL_BHNT.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_BHNT.UI
{
    public partial class FormMain : Form
    {
        private string _username;
        public FormMain(string username)
        {
            InitializeComponent();
            _username = username; // Lưu lại username để sử dụng
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            // Gọi DAO để lấy thông tin nhân viên theo username
            EmployeeDAO employeeDAO = new EmployeeDAO();

            try
            {
                // Lấy thông tin employeeId và position dựa trên username
                var employeeInfo = await employeeDAO.GetEmployeeInfoByUsernameAsync(_username);

                // Hiển thị thông tin lên các TextBox
                txt_emp_id.Text = employeeInfo.EmployeeId;
                txt_role.Text = employeeInfo.Position;

                // Phân quyền hiển thị menu
                if (employeeInfo.Position.ToLower() == "Admin") // Kiểm tra vị trí là Admin
                {
                   

                    menuStrip1.Visible = false; // Ẩn menuStrip1
                    menuStrip2.Visible = true;  // Hiện menuStrip2
                }
                else
                {
                    menuStrip1.Visible = true;  // Hiện menuStrip1
                    menuStrip2.Visible = false;  // Ẩn menuStrip2
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy thông tin nhân viên: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void quảnLýYêuCầuBồiThườngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Claims claims = new Claims();
            claims.Show();
        }

        private void báoCáoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report rp = new Report();
            rp.Show();
        }

        private void thốngKêToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Lấy EmployeeId từ TextBox hiện tại trong FormMain
            string employeeId = txt_emp_id.Text;

            // Kiểm tra giá trị employeeId trước khi truyền sang form Payment
            if (!string.IsNullOrEmpty(employeeId))
            {
                // Mở form Payment và truyền EmployeeId
                Payment paymentForm = new Payment(employeeId);
                paymentForm.Show();
            }
            else
            {
              //  MessageBox.Show("EmployeeId không được tìm thấy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void quảnLýGiaoDịchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Khởi tạo form quản lý giao dịch
            Transaction transactionForm = new Transaction();

            // Hiển thị form quản lý giao dịch
            transactionForm.Show();
        }

        private void quảnLýKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer ct = new Customer();
            ct.Show();
        }

        private void cậpNhậpĐạiLýKHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Agent ag = new Agent();
            ag.Show();
        }

        private void quảnLýHợpĐồngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Policy py = new Policy();
            py.Show();
        }

        private void thôngTinCáNhânToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void quảnLýKháchHàngToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            Customer ct = new Customer();
            ct.Show();

        }

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Lấy EmployeeId từ TextBox hiện tại trong FormMain
            string employeeId = txt_emp_id.Text;

            // Kiểm tra giá trị employeeId trước khi truyền sang form Payment
            if (!string.IsNullOrEmpty(employeeId))
            {
                // Mở form Payment và truyền EmployeeId
                Payment paymentForm = new Payment(employeeId);
                paymentForm.Show();
            }
            else
            {
                //  MessageBox.Show("EmployeeId không được tìm thấy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void quảnLýĐạiLýToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Agent ag = new Agent();
            ag.Show();
        }

        private void quảnLýHợpĐồngToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Policy py = new Policy();
            py.Show();
        }

        private void quảnLýYêuCầuBồiThườngToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Claims claims = new Claims();
            claims.Show();
        }

        private void quảnLýGiaoDịchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Khởi tạo form quản lý giao dịch
            Transaction transactionForm = new Transaction();

            // Hiển thị form quản lý giao dịch
            transactionForm.Show();
        }
    }
}
