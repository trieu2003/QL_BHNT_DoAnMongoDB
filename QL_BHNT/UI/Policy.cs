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
    public partial class Policy : Form
    {
        private PolicyDAO _policyDAO;
        private CustomerDAO _customerDAO;
        public Policy()
        {
            InitializeComponent();
            _policyDAO = new PolicyDAO();
            _customerDAO = new CustomerDAO();
        }


        private void bnt_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Policy_Load(object sender, EventArgs e)
        {
            LoadPolicyListView();
        }
        private async void LoadPolicyListView()
        {
            txt_maso_HD.Enabled = false;  // Xóa mã số hợp đồng
            txt_hoten.Enabled = false; // Xóa họ tên
            datetime_ngaysinh.Enabled = false;  // Đặt lại ngày sinh về mặc định
            radio_nam.Enabled = false;  // Bỏ chọn giới tính nam
            radio_nu.Enabled = false;
            try
            {
                // Bước 1: Lấy danh sách khách hàng từ cơ sở dữ liệu
                var customerList = await _customerDAO.GetCustomerListAsync(); // Hoặc phương thức thích hợp để lấy danh sách chính sách

                // Bước 2: Xóa sạch ListView trước khi thêm dữ liệu mới
                list_DS_Policy.Items.Clear(); // Thay đổi tên list view nếu cần

                // Bước 3: Duyệt qua từng khách hàng và thêm thông tin chính sách vào ListView
                foreach (var customer in customerList)
                {
                    // Duyệt qua từng chính sách của khách hàng
                    foreach (var policy in customer.Policies) // Giả sử mỗi customer có thuộc tính Policies
                    {
                        // Tạo một dòng mới trong ListView
                        var listViewItem = new ListViewItem(customer.CustomerId); // MaKH

                       
                        listViewItem.SubItems.Add(policy.PolicyNumber); // Số hợp đồng (policy_number)
                        listViewItem.SubItems.Add(policy.StartDate.ToShortDateString()); // Ngày bắt đầu
                        listViewItem.SubItems.Add(policy.EndDate.ToShortDateString()); // Ngày kết thúc
                        listViewItem.SubItems.Add(policy.PremiumAmount.ToString()); // Số tiền phí (định dạng với dấu phân cách hàng nghìn)
                        listViewItem.SubItems.Add(policy.CoverageAmount.ToString()); // Số tiền bảo hiểm (định dạng với dấu phân cách hàng nghìn)
                        listViewItem.SubItems.Add(policy.Status); // Trạng thái chính sách

                        // Bước 4: Thêm dòng vào ListView
                        list_DS_Policy.Items.Add(listViewItem);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách chính sách: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void list_DS_Policy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn hay không
            if (list_DS_Policy.SelectedItems.Count > 0)
            {
                // Lấy giá trị từ các trường cần thiết
                // Lấy mã giao dịch từ mục đã chọn
                string customerId = list_DS_Policy.SelectedItems[0].SubItems[0].Text;
               
                string policyNumber = list_DS_Policy.SelectedItems[0].SubItems[1].Text; // Giả sử bạn lưu số hợp đồng ở cột đầu tiên

                // Gọi hàm GetCustomerInfo để lấy thông tin khách hàng và hợp đồng

                var customerInfoList = await _policyDAO.GetCustomerInfo(customerId, policyNumber);

                // Kiểm tra xem có thông tin nào không
                if (customerInfoList.Count > 0)
                {
                    // Lấy thông tin đầu tiên (vì chỉ cần một khách hàng cho một hợp đồng)
                    var customerInfo = customerInfoList[0];

                    // Cập nhật các trường thông tin
                    txt_makh.Text = customerInfo.CustomerId;
                    txt_hoten.Text = customerInfo.FullName;
                    datetime_ngaysinh.Value = customerInfo.DateOfBirth;
                    radio_nam.Checked = customerInfo.Gender == "Nam"; // Điều chỉnh theo giá trị giới tính của bạn
                    radio_nu.Checked = customerInfo.Gender == "Nữ"; // Điều chỉnh theo giá trị giới tính của bạn
                    txt_Hoten_NHT.Text = customerInfo.BeneficiaryFullName ?? string.Empty;
                    txt_QH.Text = customerInfo.BeneficiaryRelationship ?? string.Empty;
                    txt_sdt_NHT.Text = customerInfo.BeneficiaryPhone ?? string.Empty;
                    txt_email_NHT.Text = customerInfo.BeneficiaryEmail ?? string.Empty;
                    txt_maso_HD.Text = customerInfo.PolicyNumber;
                    cob_loaiHD.SelectedItem = customerInfo.PolicyType; // Giả sử bạn đã thêm các loại hợp đồng vào ComboBox
                    dateTime_BD.Value = customerInfo.StartDate;
                    dateTime_KT.Value = customerInfo.EndDate;
                    txt_premium_amount.Text = customerInfo.PremiumAmount.ToString();
                    txt_coverage_amount.Text = customerInfo.CoverageAmount.ToString();
                    datetime_hạnnop.Value = customerInfo.NextPaymentDate ?? DateTime.Now; // Hoặc giá trị mặc định
                    txt_trangthai.Text = customerInfo.Status ?? string.Empty;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy thông tin
                    MessageBox.Show("Không tìm thấy thông tin khách hàng hoặc hợp đồng.");
                }
            }
        }

        private async void txt_makh_TextChanged(object sender, EventArgs e)
        {
           
        }

        private async void txt_makh_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra xem phím nhấn có phải là Enter không
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // Ngăn chặn hành động mặc định của Enter
                e.SuppressKeyPress = true; // Ngăn tiếng beep khi nhấn Enter

                // Kiểm tra xem textbox có giá trị hay không
                if (!string.IsNullOrWhiteSpace(txt_makh.Text))
                {
                    // Lấy giá trị mã khách hàng từ textbox
                    string customerId = txt_makh.Text.Trim();

                    // Gọi hàm GetCustomerInfoByCustomerIdAsync để lấy thông tin khách hàng
                    var customerInfo = await _customerDAO.GetCustomerInfoByCustomerIdAsync(customerId);

                    // Kiểm tra xem thông tin khách hàng có tồn tại không
                    if (customerInfo != null)
                    {
                        // Cập nhật các trường thông tin tương ứng
                        txt_hoten.Text = customerInfo.FullName;
                        datetime_ngaysinh.Value = customerInfo.DateOfBirth;

                        // Cập nhật radio button cho giới tính
                        radio_nam.Checked = customerInfo.Gender == "Nam"; // Giả sử bạn có "Nam" và "Nữ"
                        radio_nu.Checked = customerInfo.Gender == "Nữ";   // Điều chỉnh theo giá trị giới tính của bạn
                    }
                    else
                    {
                        // Hiển thị thông báo không tìm thấy khách hàng
                        MessageBox.Show("Không tìm thấy thông tin khách hàng. Vui lòng kiểm tra lại mã khách hàng để tạo hợp đồng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Xóa thông tin các trường khác (tùy chọn)
                        txt_hoten.Clear();
                        datetime_ngaysinh.Value = DateTime.Now; // Đặt về giá trị mặc định, nếu cần
                        radio_nam.Checked = false;
                        radio_nu.Checked = false;
                    }

                    // Đảm bảo rằng giá trị trong txt_makh không bị thay đổi
                    txt_makh.Text = customerId;
                    txt_makh.Select(txt_makh.Text.Length, 0); // Đặt con trỏ ở cuối TextBox, không chọn gì cả
                }
            }
        }



        private async void bnt_themHD_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ form
            string customerId = txt_makh.Text; // Mã khách hàng
            string fullName = txt_hoten.Text; // Tên khách hàng
            DateTime dateOfBirth = datetime_ngaysinh.Value; // Ngày sinh
            string gender = radio_nam.Checked ? "Nam" : "Nữ"; // Giới tính

            // Thông tin người thụ hưởng
            Beneficiary beneficiary = new Beneficiary
            {
                FullName = txt_Hoten_NHT.Text, // Tên người thụ hưởng
                Relationship = txt_QH.Text, // Mối quan hệ
                ContactInformation = new ContactInfo
                {
                    Phone = txt_sdt_NHT.Text, // Số điện thoại người thụ hưởng
                    Email = txt_email_NHT.Text // Email người thụ hưởng
                }
            };

            // Thông tin hợp đồng
            string policyType = cob_loaiHD.SelectedItem.ToString(); // Loại hợp đồng
            string policyNumber = txt_maso_HD.Text; // Mã số hợp đồng
            DateTime startDate = dateTime_BD.Value; // Ngày bắt đầu
            DateTime endDate = dateTime_KT.Value; // Ngày kết thúc
            decimal premiumAmount = decimal.Parse(txt_premium_amount.Text); // Số tiền bảo hiểm
            decimal coverageAmount = decimal.Parse(txt_coverage_amount.Text); // Số tiền bảo hiểm
            DateTime paymentDueDate = datetime_hạnnop.Value; // Hạn nộp
            string status = txt_trangthai.Text; // Trạng thái

            // Gọi hàm để thêm hợp đồng cho khách hàng
            bool result = await _policyDAO.AddPolicyForCustomerAsync(customerId, policyType, premiumAmount, coverageAmount, beneficiary);

            // Thông báo cho người dùng về kết quả
            if (result)
            {
                MessageBox.Show("Hợp đồng đã được thêm thành công!");
                LoadPolicyListView();
            }
            else
            {
                MessageBox.Show("Không tìm thấy khách hàng hoặc có lỗi xảy ra!");
            }
        }

        private async void btn_XoaKH_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn hay không
            if (list_DS_Policy.SelectedItems.Count > 0)
            {
                string customerId = list_DS_Policy.SelectedItems[0].SubItems[0].Text; // Mã khách hàng
                string policyNumber = list_DS_Policy.SelectedItems[0].SubItems[1].Text; // Số hợp đồng

                // Gọi hàm để xóa hợp đồng cho khách hàng
                bool result = await _policyDAO.DeletePolicyForCustomerAsync(customerId, policyNumber);

                // Thông báo cho người dùng về kết quả
                if (result)
                {
                    MessageBox.Show("Hợp đồng đã được xóa thành công!");
                    LoadPolicyListView(); // Làm mới danh sách hợp đồng
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hợp đồng hoặc có lỗi xảy ra!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần xóa.");
            }
        }

        private async void bnt_suaHD_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn hay không
            if (list_DS_Policy.SelectedItems.Count > 0)
            {
                string customerId = list_DS_Policy.SelectedItems[0].SubItems[0].Text; // Mã khách hàng
                string policyNumber = list_DS_Policy.SelectedItems[0].SubItems[1].Text; // Số hợp đồng

                // Lấy thông tin từ form để cập nhật
                string policyType = cob_loaiHD.SelectedItem.ToString(); // Loại hợp đồng
                decimal premiumAmount = decimal.Parse(txt_premium_amount.Text); // Số tiền phí
                decimal coverageAmount = decimal.Parse(txt_coverage_amount.Text); // Số tiền bảo hiểm
                string status = txt_trangthai.Text; // Trạng thái

                // Tạo đối tượng Beneficiary
                Beneficiary beneficiary = new Beneficiary
                {
                    FullName = txt_Hoten_NHT.Text,
                    Relationship = txt_QH.Text,
                    ContactInformation = new ContactInfo
                    {
                        Phone = txt_sdt_NHT.Text,
                        Email = txt_email_NHT.Text
                    }
                };

                // Gọi hàm để sửa hợp đồng cho khách hàng
                bool result = await _policyDAO.UpdatePolicyForCustomerAsync(customerId, policyNumber, policyType, premiumAmount, coverageAmount, status, beneficiary);

                // Thông báo cho người dùng về kết quả
                if (result)
                {
                    MessageBox.Show("Hợp đồng đã được sửa thành công!");
                    LoadPolicyListView(); // Làm mới danh sách hợp đồng
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hợp đồng hoặc có lỗi xảy ra!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần sửa.");
            }
        }
        private void ClearFormFields()
        {
            txt_makh.Clear(); // Xóa mã khách hàng
            txt_hoten.Clear(); // Xóa họ tên
            datetime_ngaysinh.Value = DateTime.Now; // Đặt lại ngày sinh về mặc định
            radio_nam.Checked = false; // Bỏ chọn giới tính nam
            radio_nu.Checked = false; // Bỏ chọn giới tính nữ
            txt_Hoten_NHT.Clear(); // Xóa tên người thụ hưởng
            txt_QH.Clear(); // Xóa mối quan hệ
            txt_sdt_NHT.Clear(); // Xóa số điện thoại người thụ hưởng
            txt_email_NHT.Clear(); // Xóa email người thụ hưởng
            txt_maso_HD.Clear(); // Xóa mã số hợp đồng
            cob_loaiHD.SelectedIndex = -1; // Bỏ chọn loại hợp đồng
            dateTime_BD.Value = DateTime.Now; // Đặt lại ngày bắt đầu về mặc định
            dateTime_KT.Value = DateTime.Now; // Đặt lại ngày kết thúc về mặc định
            txt_premium_amount.Clear(); // Xóa số tiền bảo hiểm
            txt_coverage_amount.Clear(); // Xóa số tiền bảo hiểm
            datetime_hạnnop.Value = DateTime.Now; // Đặt lại hạn nộp về mặc định
            txt_trangthai.Clear(); // Xóa trạng thái
            txt_maso_HD.Enabled = false;  // Xóa mã số hợp đồng
            txt_hoten.Enabled = false; // Xóa họ tên
            datetime_ngaysinh.Enabled = false;  // Đặt lại ngày sinh về mặc định
            radio_nam.Enabled = false;  // Bỏ chọn giới tính nam
            radio_nu.Enabled = false;
        }

        private void bnt_new_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu ở tất cả các trường trên UI
            ClearFormFields();
            LoadPolicyListView(); // Gọi lại hàm để làm mới danh sách hợp đồng
        }

        private void radio_timkiem_MaKH_CheckedChanged(object sender, EventArgs e)
        {
            // Nếu radio button mã khách hàng được chọn
            if (radio_timkiem_MaKH.Checked)
            {
                // Mở các input liên quan đến mã khách hàng
                txt_makh.Enabled = false;  // Giả sử txtMaKH là textbox nhập mã khách hàng
               
                txt_hoten.Enabled=true; // Xóa họ tên
                datetime_ngaysinh.Enabled = false;  // Đặt lại ngày sinh về mặc định
                radio_nam.Enabled = false;  // Bỏ chọn giới tính nam
                radio_nu.Enabled = false;  // Bỏ chọn giới tính nữ
                txt_Hoten_NHT.Enabled = false;  // Xóa tên người thụ hưởng
                txt_QH.Enabled = false;  // Xóa mối quan hệ
                txt_sdt_NHT.Enabled = false;  // Xóa số điện thoại người thụ hưởng
                txt_email_NHT.Enabled = false;  // Xóa email người thụ hưởng
                txt_maso_HD.Enabled = false;  // Xóa mã số hợp đồng
                cob_loaiHD.Enabled = false;  // Bỏ chọn loại hợp đồng
                dateTime_BD.Enabled = false;  // Đặt lại ngày bắt đầu về mặc định
                dateTime_KT.Enabled = false;  // Đặt lại ngày kết thúc về mặc định
                txt_premium_amount.Enabled = false; // Xóa số tiền bảo hiểm
                txt_coverage_amount.Enabled = false;  // Xóa số tiền bảo hiểm
                datetime_hạnnop.Enabled = false;  // Đặt lại hạn nộp về mặc định
                txt_trangthai.Enabled = false; // Xóa trạng thái
            }
        }

        private void radio_timkiem_loaiHD_CheckedChanged(object sender, EventArgs e)
        {
            // Nếu radio button loại hợp đồng được chọn
            if (radio_timkiem_loaiHD.Checked)
            {
                // Mở các input liên quan đến loại hợp đồng
                //txtLoaiHD.Enabled = true; // Giả sử txtLoaiHD là textbox nhập loại hợp đồng
                //txtMaKH.Enabled = false; // Khóa textbox mã khách hàng
                //txtMaKH.Clear(); // Xóa nội dung nếu có
                                 // Mở các input liên quan đến mã khách hàng
                txt_makh.Enabled = false;  // Giả sử txtMaKH là textbox nhập mã khách hàng

                txt_hoten.Enabled = false; // Xóa họ tên
                datetime_ngaysinh.Enabled = false;  // Đặt lại ngày sinh về mặc định
                radio_nam.Enabled = false;  // Bỏ chọn giới tính nam
                radio_nu.Enabled = false;  // Bỏ chọn giới tính nữ
                txt_Hoten_NHT.Enabled = false;  // Xóa tên người thụ hưởng
                txt_QH.Enabled = false;  // Xóa mối quan hệ
                txt_sdt_NHT.Enabled = false;  // Xóa số điện thoại người thụ hưởng
                txt_email_NHT.Enabled = false;  // Xóa email người thụ hưởng
                txt_maso_HD.Enabled = false;  // Xóa mã số hợp đồng
                cob_loaiHD.Enabled = true;  // Bỏ chọn loại hợp đồng
                dateTime_BD.Enabled = false;  // Đặt lại ngày bắt đầu về mặc định
                dateTime_KT.Enabled = false;  // Đặt lại ngày kết thúc về mặc định
                txt_premium_amount.Enabled = false; // Xóa số tiền bảo hiểm
                txt_coverage_amount.Enabled = false;  // Xóa số tiền bảo hiểm
                datetime_hạnnop.Enabled = false;  // Đặt lại hạn nộp về mặc định
                txt_trangthai.Enabled = false; // Xóa trạng thái
            }
        }


        private async void btn_timkiem_Click(object sender, EventArgs e)
        {
            if (radio_timkiem_loaiHD.Checked)
            {
                // Tìm kiếm theo loại hợp đồng
                string policyType = cob_loaiHD.Text.Trim();
                if (!string.IsNullOrEmpty(policyType))
                {
                    try
                    {
                        var policies = await _policyDAO.GetPoliciesByPolicyTypeAsync(policyType);
                        DisplaySearchResultsAsync(policies);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập loại hợp đồng.");
                }
            }
            else if (radio_timkiem_MaKH.Checked) // Giả sử bạn có radio button cho tìm kiếm theo tên khách hàng
            {
                // Tìm kiếm theo tên khách hàng
                string customerName = txt_hoten.Text.Trim();
                if (!string.IsNullOrEmpty(customerName))
                {
                    try
                    {
                        var policies = await _policyDAO.GetPoliciesByCustomerNameAsync(customerName);
                        DisplaySearchResultsAsync(policies);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập tên khách hàng.");
                }
            }
         
        }

       private async Task DisplaySearchResultsAsync(List<PolicyModal> policies)
{
            // Lấy danh sách khách hàng từ cơ sở dữ liệu
            var customerList = await _customerDAO.GetCustomerListAsync();
            // Xóa dữ liệu hiện có trong ListView
            list_DS_Policy.Items.Clear();

            // Kiểm tra nếu kết quả không null và có phần tử
            if (policies != null && policies.Count > 0)
            {
                foreach (var customer in customerList)
                {
                    foreach (var policy in policies)
                    {
                        // Tạo một ListViewItem mới
                        var listViewItem = new ListViewItem(customer.CustomerId); // Mã KH

                        // Thêm các thông tin cần hiển thị
                        listViewItem.SubItems.Add(policy.PolicyNumber); // Mã số HD
                        listViewItem.SubItems.Add(policy.StartDate.ToShortDateString()); // Ngày bắt đầu
                        listViewItem.SubItems.Add(policy.EndDate.ToShortDateString()); // Ngày kết thúc
                        listViewItem.SubItems.Add(policy.PremiumAmount.ToString()); // Số tiền phí
                        listViewItem.SubItems.Add(policy.CoverageAmount.ToString()); // Số tiền bảo hiểm
                        listViewItem.SubItems.Add(policy.Status); // Trạng thái

                        // Thêm ListViewItem vào ListView
                        list_DS_Policy.Items.Add(listViewItem);
                    }
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy kết quả.");
            }

        }

        private void radio_timkiem_sdt_CheckedChanged(object sender, EventArgs e)
        {
            //// Nếu radio button loại hợp đồng được chọn
            //if (radio_timkiem_sdt.Checked)
            //{
            //    // Mở các input liên quan đến loại hợp đồng
            //    //txtLoaiHD.Enabled = true; // Giả sử txtLoaiHD là textbox nhập loại hợp đồng
            //    //txtMaKH.Enabled = false; // Khóa textbox mã khách hàng
            //    //txtMaKH.Clear(); // Xóa nội dung nếu có
            //    // Mở các input liên quan đến mã khách hàng
            //    txt_makh.Enabled = false;  // Giả sử txtMaKH là textbox nhập mã khách hàng

            //    txt_hoten.Enabled = false; // Xóa họ tên
            //    datetime_ngaysinh.Enabled = false;  // Đặt lại ngày sinh về mặc định
            //    radio_nam.Enabled = false;  // Bỏ chọn giới tính nam
            //    radio_nu.Enabled = false;
            //    txt_Hoten_NHT.Enabled = false;  // Xóa tên người thụ hưởng
            //    txt_QH.Enabled = false;  // Xóa mối quan hệ
            //    txt_sdt_NHT.Enabled = true;  // Xóa số điện thoại người thụ hưởng
            //    txt_email_NHT.Enabled = false;  // Xóa email người thụ hưởng
            //    txt_maso_HD.Enabled = false;  // Xóa mã số hợp đồng
            //    cob_loaiHD.Enabled = false;  // Bỏ chọn loại hợp đồng
            //    dateTime_BD.Enabled = false;  // Đặt lại ngày bắt đầu về mặc định
            //    dateTime_KT.Enabled = false;  // Đặt lại ngày kết thúc về mặc định
            //    txt_premium_amount.Enabled = false; // Xóa số tiền bảo hiểm
            //    txt_coverage_amount.Enabled = false;  // Xóa số tiền bảo hiểm
            //    datetime_hạnnop.Enabled = false;  // Đặt lại hạn nộp về mặc định
            //    txt_trangthai.Enabled = false; // Xóa trạng thái
            //}
        }

       

        private void btn_thoattimkiem_Click(object sender, EventArgs e)
        {
            txt_makh.Enabled = true;  // Giả sử txtMaKH là textbox nhập mã khách hàng

            txt_hoten.Enabled = false; // Xóa họ tên
            datetime_ngaysinh.Enabled = false;  // Đặt lại ngày sinh về mặc định
            radio_nam.Enabled = false;  // Bỏ chọn giới tính nam
            radio_nu.Enabled = false;  // Bỏ chọn giới tính nữ
            txt_Hoten_NHT.Enabled = true;  // Xóa tên người thụ hưởng
            txt_QH.Enabled = true;  // Xóa mối quan hệ
            txt_sdt_NHT.Enabled = true;  // Xóa số điện thoại người thụ hưởng
            txt_email_NHT.Enabled = true;  // Xóa email người thụ hưởng
            txt_maso_HD.Enabled = false;  // Xóa mã số hợp đồng
            cob_loaiHD.Enabled = true;  // Bỏ chọn loại hợp đồng
            dateTime_BD.Enabled = true;  // Đặt lại ngày bắt đầu về mặc định
            dateTime_KT.Enabled = true;  // Đặt lại ngày kết thúc về mặc định
            txt_premium_amount.Enabled = true; // Xóa số tiền bảo hiểm
            txt_coverage_amount.Enabled = true;  // Xóa số tiền bảo hiểm
            datetime_hạnnop.Enabled = true;  // Đặt lại hạn nộp về mặc định
            txt_trangthai.Enabled = true; // Xóa trạng thái
        }
    }
}
