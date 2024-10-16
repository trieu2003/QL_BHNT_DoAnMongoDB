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
    public partial class Agent : Form
    {
        private AgentDAO _agentDAO;
        private CustomerDAO _customerDAO;

        public Agent()
        {
            InitializeComponent();

            _agentDAO = new AgentDAO(); 
            _customerDAO  = new CustomerDAO();
        }

        private void Agent_Load(object sender, EventArgs e)
        {
            LoadAgentListView();
        }
        private async Task LoadAgentListView()
        {
            try
            {
                listView_DaiLy.Items.Clear(); // Xóa dữ liệu cũ trong ListView

                // Lấy danh sách đại lý
                var agents = await _agentDAO.GetAgentListAsync();

                if (agents == null || agents.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu đại lý.");
                    return;
                }

                // Duyệt qua từng đại lý để lấy danh sách khách hàng
                foreach (var agent in agents)
                {
                    // Lấy danh sách khách hàng có status là 'active' theo agentId
                    var activeCustomers = await _customerDAO.GetActiveCustomersByAgentIdAsync(agent.AgentId);

                    // Nếu có khách hàng active, thêm vào ListView
                    foreach (var customer in activeCustomers)
                    {
                        ListViewItem item = new ListViewItem(customer.CustomerId);  // Mã KH
                        item.SubItems.Add(agent.AgentId);                            // Mã Đại Lý
                        item.SubItems.Add(agent.Name);                              // Tên Đại Lý
                        item.SubItems.Add(agent.ContactInfo.Address);               // Địa chỉ

                        listView_DaiLy.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu đại lý: {ex.Message}");
            }
        }

        private void listView_DaiLy_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private async Task DisplayAgentAndCustomerInfo(string customerId)
        {
            try
            {
                // Lấy thông tin khách hàng dựa trên mã KH
                var customer = await _customerDAO.GetCustomerInfoByCustomerIdAsync(customerId);
                if (customer != null)
                {
                    // Cập nhật thông tin khách hàng vào các TextBox
                    txt_makh.Text = customer.CustomerId;
                    txt_hoten.Text = customer.FullName;
                    datetime_ngaysinh.Text = customer.DateOfBirth.ToShortDateString();

                    // Xử lý giới tính khách hàng
                    if (!string.IsNullOrEmpty(customer.Gender))
                    {
                        string genderLowerCase = customer.Gender.ToLower();

                        if (genderLowerCase == "nam")
                        {
                            radio_nam.Checked = true;
                            radio_Nu.Checked = false;
                        }
                        else if (genderLowerCase == "nữ" || genderLowerCase == "nu")
                        {
                            radio_nam.Checked = false;
                            radio_Nu.Checked = true;
                        }
                        else
                        {
                            radio_nam.Checked = false;
                            radio_Nu.Checked = false;
                        }
                    }
                    else
                    {
                        radio_nam.Checked = false;
                        radio_Nu.Checked = false;
                    }

                    // Lấy thông tin đại lý tương ứng
                    var agent = await _agentDAO.GetAgentInfoByAgentIdAsync(customer.AgentId);
                    if (agent != null)
                    {
                        // Cập nhật thông tin đại lý vào các TextBox
                        txt_MaDL.Text = agent.AgentId;
                        txt_TenDL.Text = agent.Name;
                        txt_DiaChi.Text = agent.ContactInfo.Address;
                        txt_Email.Text = agent.ContactInfo.Email;
                        txt_SDT.Text = agent.ContactInfo.Phone;
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin khách hàng.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị thông tin: {ex.Message}");
            }
        }

        private async void listView_DaiLy_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {

                var customerId = e.Item.SubItems[0].Text;

                // Hiển thị thông tin khách hàng và đại lý tương ứng
                await DisplayAgentAndCustomerInfo(customerId);
            }

        }

        private async void bnt_capnhat_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã KH và mã Đại Lý trước khi cập nhật
                if (string.IsNullOrEmpty(txt_makh.Text) || string.IsNullOrEmpty(txt_MaDL.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng và đại lý để cập nhật.");
                    return;
                }

                AgentModal updatedAgent = new AgentModal
                {
                    AgentId = txt_MaDL.Text, // Không thay đổi mã đại lý
                    Name = txt_TenDL.Text,
                    ContactInfo = new AgentModal.AgentContactInfo
                    {
                        Address = txt_DiaChi.Text,
                        Email = txt_Email.Text,
                        Phone = txt_SDT.Text
                    }
                };

                // Cập nhật thông tin đại lý trong CSDL
                bool result = await _agentDAO.UpdateAgentInfoAsync(updatedAgent);

                if (result)
                {
                    MessageBox.Show("Cập nhật thông tin đại lý thành công.");
                    LoadAgentListView(); // Tải lại danh sách đại lý sau khi cập nhật
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin đại lý thất bại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin đại lý: {ex.Message}");
            }
        }
        private async Task TimKiemTheoDiaChi(string diaChi)
        {
            try
            {
                listView_DaiLy.Items.Clear(); // Xóa dữ liệu cũ

                // Tìm kiếm đại lý theo địa chỉ
                var agents = await _agentDAO.GetAgentsByAddressAsync(diaChi);

                if (agents != null && agents.Count > 0)
                {
                    foreach (var agent in agents)
                    {
                        ListViewItem item = new ListViewItem(agent.AgentId);
                        item.SubItems.Add(agent.Name);
                        item.SubItems.Add(agent.ContactInfo.Address);
                        listView_DaiLy.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đại lý nào với địa chỉ này.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm đại lý theo địa chỉ: {ex.Message}");
            }
        }

        private async Task TimKiemTheoTenDaiLy(string tenDaiLy)
        {
            try
            {
                listView_DaiLy.Items.Clear(); // Xóa dữ liệu cũ

                // Tìm kiếm đại lý theo tên
                var agents = await _agentDAO.GetAgentsByNameAsync(tenDaiLy);

                if (agents != null && agents.Count > 0)
                {
                    foreach (var agent in agents)
                    {
                        ListViewItem item = new ListViewItem(agent.AgentId);
                        item.SubItems.Add(agent.Name);
                        item.SubItems.Add(agent.ContactInfo.Address);
                        listView_DaiLy.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đại lý nào với tên này.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm đại lý theo tên: {ex.Message}");
            }
        }

        private async Task TimKiemTheoTenKhachHang(string tenKhachHang)
        {
            try
            {
                listView_DaiLy.Items.Clear(); // Xóa dữ liệu cũ

                // Tìm kiếm khách hàng theo tên
                var customers = await _customerDAO.GetCustomersByNameAsync(tenKhachHang);

                if (customers != null && customers.Count > 0)
                {
                    foreach (var customer in customers)
                    {
                        // Lấy thông tin đại lý tương ứng với khách hàng
                        var agent = await _agentDAO.GetAgentInfoByAgentIdAsync(customer.AgentId);

                        if (agent != null)
                        {
                            ListViewItem item = new ListViewItem(customer.CustomerId);
                            item.SubItems.Add(agent.AgentId);
                            item.SubItems.Add(agent.Name);
                            item.SubItems.Add(agent.ContactInfo.Address);
                            listView_DaiLy.Items.Add(item);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng nào với tên này.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khách hàng theo tên: {ex.Message}");
            }
        }

        private void radio_TK_DC_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_DiaChi.Checked)
            {
                txt_DiaChi.Visible = true; // Hiển thị TextBox địa chỉ
                txt_TenDL.Visible = false; // Ẩn TextBox tên đại lý
                txt_hoten.Visible = false; // Ẩn TextBox tên khách hàng
            }
        }

        private void radio_TK_TenDL_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_TK_TenDL.Checked)
            {
                txt_DiaChi.Visible = false; // Ẩn TextBox địa chỉ
                txt_TenDL.Visible = true;   // Hiển thị TextBox tên đại lý
                txt_hoten.Visible = false;  // Ẩn TextBox tên khách hàng
            }
        }

        private void radio_TK_TenKH_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_TK_TenKH.Checked)
            {
                txt_DiaChi.Visible = false; // Ẩn TextBox địa chỉ
                txt_TenDL.Visible = false;  // Ẩn TextBox tên đại lý
                txt_hoten.Visible = true;   // Hiển thị TextBox tên khách hàng
            }
        }

        private async void btn_tk_daily_Click(object sender, EventArgs e)
        {
            if (radio_DiaChi.Checked)
            {
                // Tìm kiếm theo địa chỉ
                await TimKiemTheoDiaChi(txt_DiaChi.Text);
            }
            else if (radio_TK_TenDL.Checked)
            {
                // Tìm kiếm theo tên đại lý
                await TimKiemTheoTenDaiLy(txt_TenDL.Text);
            }
            else if (radio_TK_TenKH.Checked)
            {
                // Tìm kiếm theo tên khách hàng
                await TimKiemTheoTenKhachHang(txt_hoten.Text);
            }
        }
    }
}
