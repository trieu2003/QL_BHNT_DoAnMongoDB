using QL_BHNT.DAO;
using QL_BHNT.Modal;
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
    public partial class Customer : Form
    {
        private CustomerDAO _customerDAO;
        private AgentDAO _agentDAO; // Thêm AgentDAO
        public Customer()
        {
            InitializeComponent();
            _customerDAO = new CustomerDAO();
            _agentDAO = new AgentDAO(); // Khởi tạo AgentDAO
            // LoadCustomerListView();
        }

        private async void Customer_Load(object sender, EventArgs e)
        {
            txt_makh.Visible = false;
            // Gọi hàm LoadCustomerListView khi form được tải
            await LoadCustomerListView();
            await LoadAgentIdsToComboBox();
        }
        private List<CustomerModal> _allCustomers = new List<CustomerModal>(); // Biến toàn cục lưu danh sách khách hàng
        private async Task LoadCustomerListView()
        {
            try
            {
                listKH.Items.Clear();
                _allCustomers = await _customerDAO.GetCustomerListAsync(); // Lưu danh sách khách hàng vào biến toàn cục

                if (_allCustomers == null || _allCustomers.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu khách hàng.");
                    return;
                }

                foreach (var customer in _allCustomers)
                {
                    ListViewItem item = new ListViewItem(customer.CustomerId);
                    item.SubItems.Add(customer.FullName);
                    item.SubItems.Add(customer.DateOfBirth.ToString("dd/MM/yyyy")); // Định dạng ngày tháng năm
                    item.SubItems.Add(customer.Gender);

                    listKH.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}");
            }
        }

        private async Task LoadAgentIdsToComboBox()
        {
            try
            {
                // Lấy danh sách mã đại lý từ AgentDAO
                List<string> agentIds = await _agentDAO.GetAgentIdsAsync();

                // Xóa các item hiện tại của ComboBox
                comboBox_MaDL.Items.Clear();

                // Thêm các mã đại lý vào ComboBox
                foreach (var agentId in agentIds)
                {
                    comboBox_MaDL.Items.Add(agentId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load mã đại lý: {ex.Message}");
            }
        }


        private void ShowAllFields()
        {
            // Hiển thị lại tất cả các TextBox và Label
            txt_makh.Visible = true;
            txt_hoten.Visible = true;
            datetime_ngaysinh.Visible = true;
            radio_nam.Visible = true;
            radio_Nu.Visible = true;
            txt_sdt.Visible = true;
            txt_email.Visible = true;
            txt_diachi.Visible = true;
            comboBox_MaDL.Visible = true;

        }

        private async void ClearInputFields()
        {
            txt_makh.Clear();
            txt_hoten.Clear();
            datetime_ngaysinh.Value = DateTime.Now;
            radio_nam.Checked = true;
            txt_sdt.Clear();
            txt_email.Clear();
            txt_diachi.Clear();
            await LoadCustomerListView();
        }

        private bool ValidateFields()
        {
            // Kiểm tra các trường không được bỏ trống
            if (string.IsNullOrWhiteSpace(txt_hoten.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_sdt.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_email.Text))
            {
                MessageBox.Show("Vui lòng nhập email.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_diachi.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ.");
                return false;
            }

            // Kiểm tra định dạng họ và tên (không chứa số)
            if (txt_hoten.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Họ và tên không được chứa số.");
                return false;
            }

            // Kiểm tra ngày sinh phải lớn hơn 18 năm tính từ ngày hiện tại
            if ((DateTime.Now.Year - datetime_ngaysinh.Value.Year) < 18)
            {
                MessageBox.Show("Khách hàng phải trên 18 tuổi.");
                return false;
            }

            // Kiểm tra định dạng email hợp lệ
            try
            {
                var email = new System.Net.Mail.MailAddress(txt_email.Text);
                if (email.Address != txt_email.Text)
                {
                    MessageBox.Show("Định dạng email không hợp lệ.");
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Định dạng email không hợp lệ.");
                return false;
            }

            // Kiểm tra số điện thoại chỉ chứa số và phải có 10 chữ số
            if (!txt_sdt.Text.All(char.IsDigit) || txt_sdt.Text.Length != 10)
            {
                MessageBox.Show("Số điện thoại phải là 10 chữ số.");
                return false;
            }

            return true; // Tất cả các trường hợp đều hợp lệ
        }
        private async Task<string> GenerateCustomerIdAsync()
        {
            var existingCustomers = await _customerDAO.GetCustomerListAsync();

            int newIdNumber = 1; // Giá trị mặc định

            if (existingCustomers.Count > 0)
            {
                var maxId = existingCustomers
                    .Where(c => c.CustomerId.StartsWith("KH"))
                    .Select(c => c.CustomerId.Substring(2)) // Bỏ "KH" và lấy phần số
                    .Select(id => int.TryParse(id, out int number) ? number : 0) // Chuyển đổi về int
                    .DefaultIfEmpty(0) // Trả về 0 nếu không có phần tử nào
                    .Max();

                newIdNumber = maxId + 1; // Tăng lên 1
            }

            string newCustomerId = $"KH{newIdNumber:D3}";

            // Kiểm tra mã khách hàng trong MongoDB
            while (await _customerDAO.CustomerIdExistsAsync(newCustomerId))
            {
                newIdNumber++;
                newCustomerId = $"KH{newIdNumber:D3}"; // Sinh mã mới nếu mã đã tồn tại
            }

            return newCustomerId; // Trả về mã khách hàng không trùng lặp
        }
        private async void listKH_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào đang được chọn hay không
            if (listKH.SelectedItems.Count > 0)
            {
                // Lấy mã khách hàng từ cột đầu tiên (CustomerId)
                string selectedCustomerId = listKH.SelectedItems[0].SubItems[0].Text;

                try
                {
                    // Tạo đối tượng CustomerDAO
                    CustomerDAO customerDAO = new CustomerDAO();

                    // Gọi hàm lấy thông tin khách hàng dựa trên mã khách hàng
                    var customer = await customerDAO.GetCustomerInfoByCustomerIdAsync(selectedCustomerId);

                    // Nếu tìm thấy khách hàng, hiển thị thông tin lên form
                    if (customer != null)
                    {
                        txt_makh.Text = customer.CustomerId;
                        txt_hoten.Text = customer.FullName;
                        datetime_ngaysinh.Value = customer.DateOfBirth;

                        // Chọn radio button giới tính
                        if (customer.Gender.ToLower() == "nam")
                        {
                            radio_nam.Checked = true;
                            radio_Nu.Checked = false;
                        }
                        else if (customer.Gender.ToLower() == "nu")
                        {
                            radio_nam.Checked = false;
                            radio_Nu.Checked = true;
                        }

                        txt_sdt.Text = customer.ContactInformation.Phone;
                        txt_email.Text = customer.ContactInformation.Email;
                        txt_diachi.Text = customer.ContactInformation.Address;

                        // Hiển thị mã đãi lý lên comboBox_MaDL
                        comboBox_MaDL.SelectedItem = customer.AgentId; // Giả sử AgentId là mã đãi lý

                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin khách hàng.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin khách hàng: {ex.Message}");
                }
            }
        }

        private void btn_timkiem_kh_Click(object sender, EventArgs e)
        {
            try
            {
                listKH.Items.Clear(); // Xóa dữ liệu cũ trong ListView

                if (radio_MaKH.Checked)
                {
                    // Tìm kiếm theo Mã KH
                    string searchCustomerId = txt_makh.Text;

                    if (string.IsNullOrWhiteSpace(searchCustomerId))
                    {
                        MessageBox.Show("Vui lòng nhập Mã KH để tìm kiếm.");
                        return;
                    }

                    // Lọc danh sách khách hàng đã tải theo Mã KH
                    var customer = _allCustomers.FirstOrDefault(c => c.CustomerId.Equals(searchCustomerId, StringComparison.OrdinalIgnoreCase));

                    if (customer != null)
                    {
                        ListViewItem item = new ListViewItem(customer.CustomerId);
                        item.SubItems.Add(customer.FullName);
                        item.SubItems.Add(customer.DateOfBirth.ToString("dd/MM/yyyy"));
                        item.SubItems.Add(customer.Gender);

                        listKH.Items.Add(item);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng với Mã KH đã nhập.");
                    }
                }
                else if (radio_HovaTen.Checked)
                {
                    // Tìm kiếm theo Họ và Tên
                    string searchFullName = txt_hoten.Text;

                    if (string.IsNullOrWhiteSpace(searchFullName))
                    {
                        MessageBox.Show("Vui lòng nhập Họ và Tên để tìm kiếm.");
                        return;
                    }

                    // Lọc danh sách khách hàng đã tải theo Họ và Tên gần đúng
                    var customers = _allCustomers
                        .Where(c => c.FullName.IndexOf(searchFullName, StringComparison.OrdinalIgnoreCase) >= 0) // Tìm kiếm gần đúng
                        .ToList();

                    if (customers != null && customers.Count > 0)
                    {
                        foreach (var customer in customers)
                        {
                            ListViewItem item = new ListViewItem(customer.CustomerId);
                            item.SubItems.Add(customer.FullName);
                            item.SubItems.Add(customer.DateOfBirth.ToString("dd/MM/yyyy"));
                            item.SubItems.Add(customer.Gender);

                            listKH.Items.Add(item);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng với Họ và Tên gần đúng đã nhập.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
            }

            // Hiện lại tất cả các trường sau khi tìm kiếm
            ShowAllFields();
        }

        private async void btn_them_Click(object sender, EventArgs e)
        {
            // Kiểm tra trường thông tin
            if (!ValidateFields())
            {
                MessageBox.Show("Vui lòng kiểm tra các trường thông tin.");
                return;
            }

            // Tạo mã khách hàng mới
            string customerId = await GenerateCustomerIdAsync();

            // Tạo đối tượng khách hàng mới
            // Tạo đối tượng khách hàng mới
            CustomerModal newCustomer = new CustomerModal
            {
                CustomerId = customerId, // Sử dụng mã khách hàng tự tạo
                FullName = txt_hoten.Text,
                DateOfBirth = datetime_ngaysinh.Value,
                Gender = radio_nam.Checked ? "Nam" : "Nữ",
                ContactInformation = new ContactInfo
                {
                    Phone = txt_sdt.Text,
                    Email = txt_email.Text,
                    Address = txt_diachi.Text
                },
                Status = "active",
                AgentId = comboBox_MaDL.SelectedItem.ToString(),

                // Khởi tạo các mảng rỗng cho policies, claims và payments
                Policies = new List<PolicyModal>(),
                Claims = new List<Claim>(),
                Payments = new List<PaymentModal>()
            };

            try
            {
                // Thêm khách hàng mới
                await _customerDAO.AddCustomerAsync(newCustomer);
                await LoadCustomerListView();
                MessageBox.Show("Thêm khách hàng thành công!");
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}");
            }


            try
            {
                // Thêm khách hàng mới mà không cần kiểm tra mã KH
                await _customerDAO.AddCustomerAsync(newCustomer);
                await LoadCustomerListView();
                MessageBox.Show("Thêm khách hàng thành công!");
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}");
            }
        }

        private async void btn_sua_Click(object sender, EventArgs e)
        {
            if (listKH.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để sửa.");
                return;
            }

            if (!ValidateFields())
            {
                MessageBox.Show("Vui lòng kiểm tra các trường thông tin.");
                return;
            }

            // Lấy mã khách hàng từ form (không cho phép sửa mã khách hàng)
            string customerId = txt_makh.Text;

            string selectedAgentId = comboBox_MaDL.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedAgentId))
            {
                MessageBox.Show("Vui lòng chọn mã đại lý.");
                return;
            }

            // Tạo đối tượng khách hàng với thông tin đã chỉnh sửa
            CustomerModal updatedCustomer = new CustomerModal
            {
                CustomerId = customerId, // Mã KH không đổi
                FullName = txt_hoten.Text,
                DateOfBirth = datetime_ngaysinh.Value,
                Gender = radio_nam.Checked ? "Nam" : "Nữ",
                ContactInformation = new ContactInfo
                {
                    Phone = txt_sdt.Text,
                    Email = txt_email.Text,
                    Address = txt_diachi.Text
                },
                Status = "active",// Cập nhật trạng thái là active
                AgentId = selectedAgentId
            };

            try
            {
                // Gọi DAO để cập nhật thông tin khách hàng trong cơ sở dữ liệu
                await _customerDAO.UpdateCustomerAsync(updatedCustomer);

                // Cập nhật lại ListView sau khi sửa
                await LoadCustomerListView();

                MessageBox.Show("Cập nhật thông tin khách hàng thành công!");

                // Làm sạch các trường nhập liệu
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin khách hàng: {ex.Message}");
            }
        }

        private async void btn_xoaKH_Click(object sender, EventArgs e)
        {
            if (listKH.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xóa.");
                return;
            }

            // Lấy mã khách hàng từ hàng được chọn
            string selectedCustomerId = listKH.SelectedItems[0].SubItems[0].Text;

            // Hiển thị thông báo xác nhận trước khi xóa
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?",
                                                        "Xác nhận xóa",
                                                        MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    // Cập nhật trạng thái khách hàng thành "inactive"
                    await _customerDAO.UpdateCustomerStatusAsync(selectedCustomerId, "inactive");

                    // Cập nhật lại danh sách khách hàng trong ListView
                    await LoadCustomerListView();

                    MessageBox.Show("Xóa khách hàng thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}");
                }
            }
        }

        private void btn_lammoi_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radio_MaKH_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_MaKH.Checked)
            {
                txt_makh.Visible = true;
                txt_hoten.Visible = false;
                datetime_ngaysinh.Visible = false;
                radio_nam.Visible = false;
                radio_Nu.Visible = false;
                txt_sdt.Visible = false;
                txt_email.Visible = false;
                txt_diachi.Visible = false;
                comboBox_MaDL.Visible = false;
            }

        }

        private void radio_HovaTen_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_HovaTen.Checked)
            {
                txt_hoten.Visible = true;
                txt_makh.Visible = false;
                datetime_ngaysinh.Visible = false;
                radio_nam.Visible = false;
                radio_Nu.Visible = false;
                txt_sdt.Visible = false;
                txt_email.Visible = false;
                txt_diachi.Visible = false;
                comboBox_MaDL.Visible = false;

            }
        }
    }
}
