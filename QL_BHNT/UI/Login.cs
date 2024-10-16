using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QL_BHNT.DAO;

namespace QL_BHNT.UI
{
    public partial class Login : Form
    {
        private readonly EmployeeDAO _employeeDAO;
        public Login()
        {
            InitializeComponent();
            _employeeDAO = new EmployeeDAO(); // Khởi tạo EmployeeDAO
        }

        private async void btn_dangnhap_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ các TextBox
            string username = txt_username.Text.Trim();
            string password = txt_password.Text.Trim();

            // Xác thực thông tin đăng nhập

            bool isAuthenticated = await _employeeDAO.AuthenticateAsync(username, password);

            if (isAuthenticated)
            {
                // Thông báo đăng nhập thành công
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Thực hiện hành động sau khi đăng nhập thành công (ví dụ: mở form chính)
                // Thực hiện hành động sau khi đăng nhập thành công (ví dụ: mở form chính)
                this.Hide(); // Ẩn form đăng nhập
                FormMain mainForm = new FormMain(username); // Tạo form chính và truyền username vào
                mainForm.Show(); // Hiển thị form chính
            }
            else
            {
                // Thông báo đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
