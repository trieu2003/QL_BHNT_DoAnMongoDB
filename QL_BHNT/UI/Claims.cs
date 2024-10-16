using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QL_BHNT.DAO;
using QL_BHNT.Modal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QL_BHNT.UI
{
    public partial class Claims : Form
    {
        private ClaimDAO claimDAO;
        private CustomerDAO _customerDAO;
        public Claims()
        {
           claimDAO = new ClaimDAO(); 
            _customerDAO = new CustomerDAO();
            InitializeComponent();
        }

        private async void list_DS_BT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_DS_BT.SelectedItems.Count > 0)
            {
                // Lấy mã khách hàng từ cột đầu tiên của mục đã chọn
                string customerId = list_DS_BT.SelectedItems[0].SubItems[0].Text; // Mã khách hàng
                string policyNumber = list_DS_BT.SelectedItems[0].SubItems[1].Text; // Mã hợp đồng
                string claimNumber = list_DS_BT.SelectedItems[0].SubItems[2].Text; // Số bảo hiểm (Mã yêu cầu bồi thường)

                // Gọi hàm để lấy thông tin chi tiết khách hàng
                CustomerModal customerDetails = await claimDAO.GetCustomerDetailsAsync(customerId);

                if (customerDetails != null)
                {
                    // Cập nhật các điều khiển trên UI
                    txt_makh.Text = customerDetails.CustomerId;
                    txt_hoten.Text = customerDetails.FullName;

                    // Tìm hợp đồng liên quan
                    var policy = customerDetails.Policies?.FirstOrDefault(p => p.PolicyNumber == policyNumber);
                    if (policy != null)
                    {
                        txt_soHD.Text = policy.PolicyNumber;
                        cbo_loaiHD.SelectedItem = policy.PolicyType;

                        // Tìm yêu cầu bồi thường liên quan
                        var claim = customerDetails.Claims?.FirstOrDefault(c => c.ClaimNumber == claimNumber);
                        if (claim != null)
                        {
                            txt_maso_BH.Text = claim.ClaimNumber;
                            dateTime_BD.Value = claim.ClaimDate;
                            txt_premium_amount.Text = claim.ClaimAmount.ToString("C");
                            txt_trangthai.Text = claim.Status;

                            // Cập nhật thông tin giấy tờ liên quan
                            txt_giaychungnhan.Text = claim.Documents.DeathCertificate ?? "Chưa có";
                            txtBanSaoHD.Text = claim.Documents.InsurancePolicy ?? "Chưa có";
                            txtGiayCMQH.Text = claim.Documents.ProofOfRelationship ?? "Chưa có";

                            // Thiết lập WebBrowser để bỏ qua lỗi script
                            webBrowser_GCT.ScriptErrorsSuppressed = true;
                            webBrowser_BanSaoHD.ScriptErrorsSuppressed = true;
                            webBrowser_CMQH.ScriptErrorsSuppressed = true;

                            // Cập nhật tài liệu cho các WebBrowser nếu cần
                            if (!string.IsNullOrEmpty(claim.Documents.DeathCertificate))
                            {
                                webBrowser_GCT.Navigate(claim.Documents.DeathCertificate); // Đường dẫn tệp PDF
                            }
                            if (!string.IsNullOrEmpty(claim.Documents.InsurancePolicy))
                            {
                                webBrowser_BanSaoHD.Navigate(claim.Documents.InsurancePolicy); // Đường dẫn tệp PDF
                            }
                            if (!string.IsNullOrEmpty(claim.Documents.ProofOfRelationship))
                            {
                                webBrowser_CMQH.Navigate(claim.Documents.ProofOfRelationship); // Đường dẫn tệp PDF
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy thông tin yêu cầu bồi thường.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin hợp đồng.");
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin khách hàng.");
                }
            }
        }



        // Hàm này dùng để lấy dữ liệu từ MongoDB và hiển thị lên ListView


        private void Claims_Load(object sender, EventArgs e)
        {
            LoadClaimsToListViewAsync();

        }
        private async void LoadClaimsToListViewAsync()
        {
            // Gọi hàm lấy danh sách khách hàng từ MongoDB, bao gồm yêu cầu bồi thường và hợp đồng
            List<CustomerModal> customerList = await _customerDAO.GetCustomerListAsync();

            // Xóa tất cả các dòng cũ trong ListView trước khi thêm mới
            list_DS_BT.Items.Clear();

            // Duyệt qua danh sách khách hàng
            foreach (var customer in customerList)
            {
                // Duyệt qua từng yêu cầu bồi thường trong danh sách yêu cầu bồi thường của khách hàng
                if (customer.Claims != null)
                {
                    foreach (var claim in customer.Claims)
                    {
                        // Kiểm tra trạng thái của yêu cầu bồi thường, chỉ thêm nếu không phải là 'Đã hủy'
                        if (claim.Status != "Đã hủy")
                        {
                            // Tìm hợp đồng liên quan đến yêu cầu bồi thường dựa trên PolicyNumber
                            var policy = customer.Policies?.FirstOrDefault(p => p.PolicyNumber == claim.PolicyNumber);

                            // Nếu tìm thấy hợp đồng liên quan
                            if (policy != null)
                            {
                                // Tạo một dòng mới cho ListView
                                var item = new ListViewItem(customer.CustomerId); // Mã khách hàng
                                item.SubItems.Add(claim.PolicyNumber); // Mã hợp đồng
                                item.SubItems.Add(claim.ClaimNumber); // Số bảo hiểm
                                item.SubItems.Add(claim.ClaimDate.ToString("dd/MM/yyyy")); // Ngày bồi thường
                                item.SubItems.Add(policy.PolicyType); // Loại hợp đồng
                                item.SubItems.Add(claim.ClaimAmount.ToString("C")); // Claims Amount (hiển thị dạng tiền tệ)
                                item.SubItems.Add(claim.Status); // Trạng thái

                                // Thêm dòng vào ListView
                                list_DS_BT.Items.Add(item);
                            }
                        }
                    }
                }
            }
        }


        private async void LoadFilteredClaimsToListViewAsync(string customerId, string claimNumber = null)
        {
            // Gọi hàm lấy danh sách khách hàng từ MongoDB
            List<CustomerModal> customerList = await _customerDAO.GetCustomerListAsync();

            // Xóa tất cả các dòng cũ trong ListView
            list_DS_BT.Items.Clear();

            // Tìm khách hàng dựa trên customerId
            var customer = customerList.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng với mã khách hàng đã cung cấp.");
                return;
            }

            // Duyệt qua danh sách yêu cầu bồi thường của khách hàng
            if (customer.Claims != null)
            {
                foreach (var claim in customer.Claims)
                {
                    // Nếu có claimNumber, kiểm tra xem claim có khớp với mã yêu cầu bồi thường không
                    if (!string.IsNullOrEmpty(claimNumber) && claim.ClaimNumber != claimNumber)
                    {
                        continue;
                    }

                    // Tìm hợp đồng liên quan đến yêu cầu bồi thường dựa trên PolicyNumber
                    var policy = customer.Policies?.FirstOrDefault(p => p.PolicyNumber == claim.PolicyNumber);

                    // Nếu tìm thấy hợp đồng liên quan
                    if (policy != null)
                    {
                        // Tạo một dòng mới cho ListView
                        var item = new ListViewItem(customer.CustomerId); // Mã khách hàng
                        item.SubItems.Add(claim.PolicyNumber); // Mã hợp đồng
                        item.SubItems.Add(claim.ClaimNumber); // Số bảo hiểm
                        item.SubItems.Add(claim.ClaimDate.ToString("dd/MM/yyyy")); // Ngày bồi thường
                        item.SubItems.Add(policy.PolicyType); // Loại hợp đồng
                        item.SubItems.Add(claim.ClaimAmount.ToString("C")); // Claims Amount (hiển thị dạng tiền tệ)
                        item.SubItems.Add(claim.Status); // Trạng thái

                        // Thêm dòng vào ListView
                        list_DS_BT.Items.Add(item);
                    }
                }
            }
        }

        private void btn_file_GCT_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                openFileDialog.Title = "Chọn tệp PDF";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Lấy đường dẫn tệp PDF
                        string filePath = openFileDialog.FileName;

                        // Hiển thị tên tệp trong TextBox
                        txt_giaychungnhan.Text = Path.GetFileName(filePath);

                        // Hiển thị PDF trong WebBrowser
                        webBrowser_GCT.Navigate(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải tệp PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnBanSaoHD_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                openFileDialog.Title = "Chọn tệp PDF";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Lấy đường dẫn tệp PDF
                        string filePath = openFileDialog.FileName;

                        // Hiển thị tên tệp trong TextBox
                        txtBanSaoHD.Text = Path.GetFileName(filePath);

                        // Hiển thị PDF trong WebBrowser
                        webBrowser_BanSaoHD.Navigate(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải tệp PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                
            }
            }
        }

        private void btnGiayCMQH_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                openFileDialog.Title = "Chọn tệp PDF";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Lấy đường dẫn tệp PDF
                        string filePath = openFileDialog.FileName;

                        // Hiển thị tên tệp trong TextBox
                        txtGiayCMQH.Text = Path.GetFileName(filePath);

                        // Hiển thị PDF trong WebBrowser
                        webBrowser_CMQH.Navigate(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải tệp PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private async void bnt_them_BT_Click(object sender, EventArgs e)
        {
            string customerId = txt_makh.Text; // Giả sử bạn có một TextBox tên là txt_makh

            // Kiểm tra giá trị txt_premium_amount.Text có phải là số hợp lệ không
            if (!decimal.TryParse(txt_premium_amount.Text, out decimal claimAmount))
            {
                MessageBox.Show("Số tiền bồi thường không hợp lệ. Vui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Thoát khỏi hàm nếu không hợp lệ
            }

            Claim newClaim = new Claim
            {
                ClaimNumber = "C" + DateTime.Now.Ticks, // Tạo mã số bồi thường tự động
                PolicyNumber = txt_soHD.Text, // Mã hợp đồng
                ClaimDate = dateTime_BD.Value, // Ngày bồi thường
                ClaimAmount = claimAmount, // Số tiền bồi thường
                Status = "Chờ xử lý", // Trạng thái khởi tạo
                Documents = new ClaimDocuments
                {
                    DeathCertificate = txt_giaychungnhan.Text, // Tên tệp giấy chứng nhận
                    InsurancePolicy = txtBanSaoHD.Text, // Tên tệp bản sao hợp đồng
                    ProofOfRelationship = txtGiayCMQH.Text // Tên tệp giấy chứng minh quan hệ
                }
            };

            try
            {
                bool isSuccess = await claimDAO.AddClaimAsync(customerId, newClaim);

                if (isSuccess)
                {
                    MessageBox.Show("Yêu cầu bồi thường đã được thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Gọi hàm để load lại danh sách yêu cầu bồi thường
                    LoadClaimsToListViewAsync();
                }
                else
                {
                    MessageBox.Show("Không thể thêm yêu cầu bồi thường!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txt_soHD_TextChanged(object sender, EventArgs e)
        {
            // Khi có thay đổi trong TextBox
            if (!string.IsNullOrWhiteSpace(txt_soHD.Text))
            {
                // Kiểm tra nếu nhấn Enter
                if (txt_soHD.Text.Length > 0)
                {
                    var contractInfo = claimDAO.DisplayContractInformation(txt_soHD.Text);
                    if (contractInfo.PolicyNumber != null) // Nếu tìm thấy thông tin hợp đồng
                    {
                        // Hiển thị thông tin lên các TextBox tương ứng
                        txt_makh.Text = contractInfo.CustomerId;
                        txt_hoten.Text = contractInfo.FullName;
                        txt_soHD.Text = contractInfo.PolicyNumber; // Giữ lại số hợp đồng
                        cbo_loaiHD.SelectedItem = contractInfo.PolicyType;
                        txt_premium_amount.Text = contractInfo.PremiumAmount.ToString("N0");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin hợp đồng.");
                    }
                }
            }
        }

        private async void bnt_xoa_BT_Click(object sender, EventArgs e)
        {
            string customerId = txt_makh.Text; // Lấy mã khách hàng từ TextBox
            string claimNumber = txt_maso_BH.Text; // Lấy mã yêu cầu bồi thường từ TextBox

            try
            {
                bool isCanceled = await claimDAO.DeleteClaimAsync(customerId, claimNumber);

                if (isCanceled)
                {
                    MessageBox.Show("Yêu cầu bồi thường đã được hủy thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Gọi hàm để load lại danh sách yêu cầu bồi thường
                    LoadClaimsToListViewAsync();
                }
                else
                {
                    MessageBox.Show("Không thể hủy yêu cầu bồi thường! Có thể yêu cầu không còn ở trạng thái 'Chờ xử lý'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //sửa 
        private async void bnt_sua_BH_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin từ các điều khiển trên giao diện
                string customerId = txt_makh.Text.Trim(); // Mã khách hàng
                string claimNumber = txt_maso_BH.Text.Trim(); // Mã yêu cầu bồi thường

                // Kiểm tra xem mã khách hàng và mã yêu cầu bồi thường có hợp lệ không
                if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(claimNumber))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ mã khách hàng và mã yêu cầu bồi thường.");
                    return;
                }

                // Tạo đối tượng Claim với thông tin cần cập nhật
                Claim updatedClaim = new Claim
                {
                    ClaimNumber = claimNumber, // Không cần sửa đổi mã yêu cầu bồi thường
                    ClaimDate = dateTime_BD.Value, // Ngày yêu cầu
                    Status = txt_trangthai.Text, // Trạng thái yêu cầu
                    ClaimAmount = decimal.TryParse(txt_premium_amount.Text, out decimal amount) ? amount : 0, // Số tiền yêu cầu
                    Documents = new ClaimDocuments
                    {
                        DeathCertificate = txt_giaychungnhan.Text, // Giấy chứng nhận
                        InsurancePolicy = txtBanSaoHD.Text, // Bản sao hợp đồng bảo hiểm
                        ProofOfRelationship = txtGiayCMQH.Text // Giấy tờ chứng minh mối quan hệ
                    }
                };

                // Gọi hàm để cập nhật yêu cầu bồi thường
                await claimDAO.UpdateClaimAsync(customerId, claimNumber, updatedClaim);

                // Hiển thị thông báo thành công
                MessageBox.Show("Cập nhật yêu cầu bồi thường thành công.");

                LoadClaimsToListViewAsync();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show($"Lỗi khi cập nhật yêu cầu bồi thường: {ex.Message}");
            }
        }


        private async void bnt_timkiem_Click(object sender, EventArgs e)
        {
            //if (radio_TK_LoaiBH.Checked)
            //{
            //    // Lấy loại bảo hiểm từ textbox hoặc combobox
            //    string policyType = cbo_loaiHD.SelectedItem?.ToString();

            //    if (string.IsNullOrEmpty(policyType))
            //    {
            //        MessageBox.Show("Vui lòng nhập loại bảo hiểm để tìm kiếm.");
            //        return;
            //    }

            //    // Gọi hàm tìm kiếm từ ClaimDAO
            //    List<CustomerModal> customerList = await claimDAO.GetClaimsByPolicyTypeAsync(policyType);

            //    // Xóa tất cả các dòng cũ trong ListView trước khi thêm mới
            //    list_DS_BT.Items.Clear();

            //    // Duyệt qua danh sách khách hàng và thêm vào ListView
            //    foreach (var customer in customerList)
            //    {
            //        if (customer.Claims != null)
            //        {
            //            foreach (var claim in customer.Claims)
            //            {
            //                // Tìm hợp đồng liên quan đến yêu cầu bồi thường dựa trên PolicyNumber
            //                var policy = customer.Policies?.FirstOrDefault(p => p.PolicyNumber == claim.PolicyNumber);

            //                // Nếu tìm thấy hợp đồng liên quan và loại bảo hiểm khớp với loại bảo hiểm tìm kiếm
            //                if (policy != null && policy.PolicyType.Equals(policyType, StringComparison.OrdinalIgnoreCase))
            //                {
            //                    // Tạo một dòng mới cho ListView
            //                    var item = new ListViewItem(customer.CustomerId); // Mã khách hàng
            //                    item.SubItems.Add(claim.PolicyNumber); // Mã hợp đồng
            //                    item.SubItems.Add(claim.ClaimNumber); // Số bảo hiểm
            //                    item.SubItems.Add(claim.ClaimDate.ToString("dd/MM/yyyy")); // Ngày bồi thường
            //                    item.SubItems.Add(policy.PolicyType); // Loại hợp đồng
            //                    item.SubItems.Add(claim.ClaimAmount.ToString("C")); // Claims Amount (hiển thị dạng tiền tệ)
            //                    item.SubItems.Add(claim.Status); // Trạng thái

            //                    // Thêm dòng vào ListView
            //                    list_DS_BT.Items.Add(item);
            //                }
            //            }
            //        }
            //    }

            //    // Hiển thị thông báo nếu không tìm thấy yêu cầu nào
            //    if (list_DS_BT.Items.Count == 0)
            //    {
            //        MessageBox.Show("Không tìm thấy yêu cầu bồi thường với loại bảo hiểm này.");
            //    }
            //}
            if (radio_tk_MaBH.Checked)
            {
                // Lấy mã hợp đồng từ textbox
                string policyNumber = txt_soHD.Text; // Mã hợp đồng 

                if (string.IsNullOrEmpty(policyNumber))
                {
                    MessageBox.Show("Vui lòng nhập mã hợp đồng để tìm kiếm.");
                    return;
                }

                // Gọi hàm tìm kiếm từ ClaimDAO
                List<CustomerModal> customerList = await claimDAO.GetClaimsByPolicyNumberAsync(policyNumber);

                // Xóa tất cả các dòng cũ trong ListView trước khi thêm mới
                list_DS_BT.Items.Clear();

                // Duyệt qua danh sách khách hàng và thêm vào ListView
                foreach (var customer in customerList)
                {
                    if (customer.Claims != null)
                    {
                        foreach (var claim in customer.Claims)
                        {
                            // Tìm hợp đồng liên quan đến yêu cầu bồi thường dựa trên PolicyNumber
                            var policy = customer.Policies?.FirstOrDefault(p => p.PolicyNumber == claim.PolicyNumber);

                            // Nếu tìm thấy hợp đồng liên quan và mã hợp đồng khớp với mã hợp đồng tìm kiếm
                            if (policy != null && policy.PolicyNumber.Equals(policyNumber, StringComparison.OrdinalIgnoreCase))
                            {
                                // Tạo một dòng mới cho ListView
                                var item = new ListViewItem(customer.CustomerId); // Mã khách hàng
                                item.SubItems.Add(claim.PolicyNumber); // Mã hợp đồng
                                item.SubItems.Add(claim.ClaimNumber); // Số bảo hiểm
                                item.SubItems.Add(claim.ClaimDate.ToString("dd/MM/yyyy")); // Ngày bồi thường
                                item.SubItems.Add(policy.PolicyType); // Loại hợp đồng
                                item.SubItems.Add(claim.ClaimAmount.ToString("C")); // Claims Amount (hiển thị dạng tiền tệ)
                                item.SubItems.Add(claim.Status); // Trạng thái

                                // Thêm dòng vào ListView
                                list_DS_BT.Items.Add(item);
                            }
                        }
                    }
                }

                // Hiển thị thông báo nếu không tìm thấy yêu cầu nào
                if (list_DS_BT.Items.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy yêu cầu bồi thường với mã hợp đồng này.");
                }
            }
            if (radio_TK_MaKH.Checked)
            {
                // Lấy mã khách hàng từ textbox
                string customerId = txt_makh.Text; // Mã khách hàng 

                if (string.IsNullOrEmpty(customerId))
                {
                    MessageBox.Show("Vui lòng nhập mã khách hàng để tìm kiếm.");
                    return;
                }

                // Gọi hàm tìm kiếm từ CustomerDAO
                List<CustomerModal> customerList = await claimDAO.GetCustomerByIdAsync(customerId);

                // Xóa tất cả các dòng cũ trong ListView trước khi thêm mới
                list_DS_BT.Items.Clear();

                // Duyệt qua danh sách khách hàng và thêm vào ListView
                foreach (var customer in customerList)
                {
                    if (customer.Claims != null)
                    {
                        foreach (var claim in customer.Claims)
                        {
                            // Tìm hợp đồng liên quan đến yêu cầu bồi thường dựa trên PolicyNumber
                            var policy = customer.Policies?.FirstOrDefault(p => p.PolicyNumber == claim.PolicyNumber);

                            // Nếu tìm thấy hợp đồng liên quan và mã khách hàng khớp với mã khách hàng tìm kiếm
                            if (policy != null && customer.CustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase))
                            {
                                // Tạo một dòng mới cho ListView
                                var item = new ListViewItem(customer.CustomerId); // Mã khách hàng
                                item.SubItems.Add(claim.PolicyNumber); // Mã hợp đồng
                                item.SubItems.Add(claim.ClaimNumber); // Số bảo hiểm
                                item.SubItems.Add(claim.ClaimDate.ToString("dd/MM/yyyy")); // Ngày bồi thường
                                item.SubItems.Add(policy.PolicyType); // Loại hợp đồng
                                item.SubItems.Add(claim.ClaimAmount.ToString("C")); // Claims Amount (hiển thị dạng tiền tệ)
                                item.SubItems.Add(claim.Status); // Trạng thái

                                // Thêm dòng vào ListView
                                list_DS_BT.Items.Add(item);
                            }
                        }
                    }
                }

                // Hiển thị thông báo nếu không tìm thấy yêu cầu nào
                if (list_DS_BT.Items.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy yêu cầu bồi thường cho mã khách hàng này.");
                }
            }



        }

        private void btn_lammoi_Click(object sender, EventArgs e)
        {
            // Đặt giá trị rỗng cho các điều khiển
            txt_premium_amount.Text = string.Empty; // Xóa nội dung TextBox
            cbo_loaiHD.SelectedIndex = -1; // Đặt lại ComboBox không chọn bất kỳ mục nào
            webBrowser_GCT.DocumentText = string.Empty; // Xóa nội dung của WebBrowser
                                                        // Đặt giá trị rỗng cho các điều khiển TextBox
            txt_makh.Text = string.Empty; // Mã khách hàng
            txt_hoten.Text = string.Empty; // Họ tên
            txt_maso_BH.Text = string.Empty; // Số bảo hiểm
            txt_soHD.Text = string.Empty; // Số hợp đồng
            txt_trangthai.Text = string.Empty; // Trạng thái
            txt_giaychungnhan.Text = string.Empty; // Giấy chứng nhận
            txtBanSaoHD.Text = string.Empty; // Bản sao hợp đồng
            txtGiayCMQH.Text = string.Empty; // Giấy chứng minh quan hệ hộ

            // Đặt lại giá trị cho DateTimePicker
            dateTime_BD.Value = DateTime.Now; // Hoặc giá trị mặc định mà bạn muốn

            // Xóa nội dung của WebBrowser
            webBrowser_BanSaoHD.DocumentText = string.Empty; // Bản sao hợp đồng
            webBrowser_CMQH.DocumentText = string.Empty; // Giấy chứng minh quan hệ hộ
        }

        private void bnt_Thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }









        ////public async Task LoadClaimsToListViewAsync()
        //{

        //    try
        //    {
        //        // Gọi hàm lấy danh sách yêu cầu bồi thường từ MongoDB
        //        List<Claim> claimsList = await GetClaimsListFromMongoDBAsync();

        //        // Xóa tất cả các mục trong ListView trước khi thêm mới
        //        list_DS_BT.Items.Clear();

        //        // Duyệt qua danh sách yêu cầu bồi thường và thêm vào ListView
        //        foreach (var claim in claimsList)
        //        {
        //            // Tạo ListViewItem để chứa từng dòng dữ liệu
        //            ListViewItem item = new ListViewItem();

        //            // Thêm mã khách hàng (Customer ID) vào cột đầu tiên
        //            item.Text = claim.CustomerId; // Bạn cần thêm CustomerId vào class Claim nếu chưa có

        //            // Thêm mã hợp đồng (Policy Number)
        //            item.SubItems.Add(claim.PolicyNumber);

        //            // Thêm mã số bồi thường (Claim Number)
        //            item.SubItems.Add(claim.ClaimNumber);

        //            // Thêm ngày bồi thường (Claim Date)
        //            item.SubItems.Add(claim.ClaimDate.ToString("dd/MM/yyyy"));

        //            // Thêm loại bồi thường (Policy Type)
        //            item.SubItems.Add(claim.PolicyType);

        //            // Thêm số tiền bồi thường (Claim Amount)
        //            item.SubItems.Add(claim.ClaimAmount.ToString("N0")); // Hiển thị định dạng số tiền

        //            // Thêm trạng thái (Claim Status)
        //            item.SubItems.Add(claim.Status);

        //            // Thêm dòng vào ListView
        //            list_DS_BT.Items.Add(item);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Lỗi khi tải dữ liệu yêu cầu bồi thường: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

    }
}
