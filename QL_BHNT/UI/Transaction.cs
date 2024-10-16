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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QL_BHNT.UI
{
    public partial class Transaction : Form
    {
        private CustomerDAO _customerDAO;
        private TransactionDAO _transactionDAO;
        public Transaction()
        {
            InitializeComponent();
            _customerDAO = new CustomerDAO();
            _transactionDAO = new TransactionDAO();
        }
        private async void LoadCustomerListView()
        {
            try
            {
                // Lấy danh sách khách hàng từ cơ sở dữ liệu
                var customerList = await _customerDAO.GetCustomerListAsync();

                // Xóa sạch ListView trước khi thêm dữ liệu mới
                list_DS_GD.Items.Clear();

                // Duyệt qua từng khách hàng và thêm vào ListView
                foreach (var customer in customerList)
                {
                    // Duyệt qua từng giao dịch thanh toán của khách hàng
                    foreach (var payment in customer.Payments)
                    {
                        // Chỉ xử lý những giao dịch có trạng thái khác "Đã Huỷ"
                        if (payment.Status != "Đã Huỷ")
                        {
                            // Tạo một dòng mới trong ListView
                            var listViewItem = new ListViewItem(customer.CustomerId); // MaKH

                            // Thêm các cột khác
                            listViewItem.SubItems.Add(payment.RelatedTransaction?.PolicyNumber ?? "N/A"); // Số hợp đồng (policy_number) từ RelatedTransaction
                            listViewItem.SubItems.Add(payment.PaymentType); // Loại thanh toán (payment_type)
                            listViewItem.SubItems.Add(payment.PaymentId); // Mã giao dịch (MaGD)
                            listViewItem.SubItems.Add(payment.Amount.ToString("N0")); // Tổng tiền (định dạng với dấu phân cách hàng nghìn)
                            listViewItem.SubItems.Add(payment.Status); // Trạng thái

                            // Thêm dòng vào ListView
                            list_DS_GD.Items.Add(listViewItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khách hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Transaction_Load(object sender, EventArgs e)
        {
            LoadCustomerListView();
        }

        private async void list_DS_GD_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn không
            if (list_DS_GD.SelectedItems.Count > 0)
            {
                // Lấy mã giao dịch từ mục đã chọn
                var selectedItem = list_DS_GD.SelectedItems[0].SubItems[3].Text;

                // Kiểm tra nếu mã giao dịch không null hoặc rỗng
                if (!string.IsNullOrEmpty(selectedItem))
                {
                    // Gọi hàm để lấy thông tin giao dịch dựa trên mã giao dịch
                    var transactionId = selectedItem;

                    try
                    {
                        // Sử dụng `await` để gọi hàm bất đồng bộ
                        var customerInfo = await _transactionDAO.DisplayTransactionDetailsAsync(transactionId);

                        if (customerInfo != null && customerInfo.Customer != null)
                        {
                            // Hiển thị thông tin khách hàng
                            txt_MaKH.Text = customerInfo.Customer.CustomerId;
                            txt_HoTen.Text = customerInfo.Customer.FullName;
                            datetime_ngaysinh.Value = customerInfo.Customer.DateOfBirth;

                            // Cập nhật RadioButton cho giới tính
                            if (customerInfo.Customer.Gender == "Nam")
                            {
                                radio_nam.Checked = true;
                                radio_nu.Checked = false;
                            }
                            else if (customerInfo.Customer.Gender == "Nữ")
                            {
                                radio_nam.Checked = false;
                                radio_nu.Checked = true;
                            }
                            else
                            {
                                radio_nam.Checked = false;
                                radio_nu.Checked = false;
                            }

                            // Hiển thị thông tin giao dịch từ Payments
                            var payment = customerInfo.Payments.FirstOrDefault(p => p.PaymentId == transactionId);
                            if (payment != null)
                            {
                                txt_MaGiaoDich.Text = payment.PaymentId;
                                dateTime_NgayGD.Value = payment.PaymentDate;
                                txt_TongTien.Text = payment.Amount.ToString("N0"); // Định dạng số tiền với dấu phân cách hàng nghìn
                                                                                   // Hiển thị phương thức thanh toán
                                cbo_phuongthuc.SelectedItem = payment.Method;

                                
                                // Hiển thị loại thanh toán (PaymentType)
                                cbo_loaithanhtoan.SelectedItem = payment.PaymentType; // Lấy loại thanh toán từ Payment

                                txt_trangthai.Text = payment.Status;

                                // Hiển thị mã hợp đồng bảo hiểm từ chính Payment
                                txt_maHD.Text = payment.RelatedTransaction?.PolicyNumber ?? "Không có"; // Lấy mã hợp đồng từ Payment nếu có
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin thanh toán cho giao dịch này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            // Nếu không tìm thấy thông tin, có thể thông báo cho người dùng
                            MessageBox.Show("Không tìm thấy thông tin giao dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Bắt lỗi nếu có lỗi xảy ra
                        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Mã giao dịch không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private async void bnt_xoaGiaoDich_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy mã khách hàng và mã giao dịch từ giao diện người dùng
                string customerId = txt_MaKH.Text;  // Giả sử mã khách hàng được nhập ở txt_MaKH
                string paymentId = txt_MaGiaoDich.Text;  // Giả sử mã giao dịch được nhập ở txt_MaGiaoDich

                // Kiểm tra xem người dùng đã nhập đủ thông tin chưa
                if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(paymentId))
                {
                    MessageBox.Show("Vui lòng nhập mã khách hàng và mã giao dịch để xoá.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi hàm để huỷ giao dịch
                bool isCancelled = await _transactionDAO.CancelTransactionAsync(customerId, paymentId);

                // Hiển thị thông báo thành công hoặc thất bại
                if (isCancelled)
                {
                    MessageBox.Show("Giao dịch đã được huỷ thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Cập nhật lại danh sách giao dịch nếu cần
                    LoadCustomerListView();  // Giả sử bạn có phương thức này để tải lại danh sách khách hàng
                }
                else
                {
                    MessageBox.Show("Không thể huỷ giao dịch. Vui lòng kiểm tra lại thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi huỷ giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btn_suaGD_Click(object sender, EventArgs e)
        {
            // Lấy thông tin giao dịch cần sửa
            var transactionId = txt_MaGiaoDich.Text;
            var customerId = txt_MaKH.Text; // Giả sử bạn có TextBox cho mã khách hàng
            var paymentDate = dateTime_NgayGD.Value;
            var amountText = txt_TongTien.Text;
            var paymentMethod = cbo_phuongthuc.SelectedItem?.ToString();
            var paymentType = cbo_loaithanhtoan.SelectedItem?.ToString();
            var paymentStatus = txt_trangthai.Text;

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(customerId))
            {
                MessageBox.Show("Vui lòng chọn một giao dịch để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chuyển đổi số tiền từ TextBox sang dạng số
            if (!decimal.TryParse(amountText, out decimal amount))
            {
                MessageBox.Show("Số tiền không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tạo đối tượng PaymentModal với thông tin cập nhật
                var paymentUpdate = new PaymentModal
                {
                    PaymentId = transactionId,
                    PaymentDate = paymentDate,
                    Amount = amount,
                    Method = paymentMethod,
                    PaymentType = paymentType,
                    Status = paymentStatus // Thay đổi trạng thái nếu cần
                };

                // Gọi hàm cập nhật giao dịch
                var result = await _transactionDAO.UpdateTransactionAsync(customerId, transactionId, paymentUpdate);

                if (result)
                {
                    MessageBox.Show("Cập nhật giao dịch thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomerListView();
                }
                else
                {
                    MessageBox.Show("Cập nhật giao dịch thất bại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btn_timkiem_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có RadioButton nào được chọn không
            if (!radio_MaKH.Checked && !radio_loaiGD.Checked && !radio_maSo.Checked)
            {
                MessageBox.Show("Vui lòng chọn phương thức tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Không có gì được chọn, dừng lại
            }

            try
            {
                // Gọi hàm tìm kiếm tương ứng dựa trên RadioButton được chọn
                if (radio_MaKH.Checked)
                {
                    await SearchByCustomerIdAsync(); // Tìm kiếm theo mã khách hàng
                }
                else if (radio_loaiGD.Checked)
                {
                    await SearchByPaymentTypeAsync(); // Tìm kiếm theo loại giao dịch
                }
                else if (radio_maSo.Checked)
                {
                    await SearchByPolicyNumberAsync(); // Tìm kiếm theo mã số
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Hàm tìm kiếm theo mã khách hàng
        private async Task SearchByCustomerIdAsync()
        {
            var customerId = txt_MaKH.Text; // Lấy mã khách hàng từ TextBox

            if (!string.IsNullOrEmpty(customerId))
            {
                try
                {
                    // Lấy danh sách giao dịch theo mã khách hàng
                    var transactions = await _transactionDAO.GetTransactionsByCustomerIdAsync(customerId);

                    if (transactions.Any())
                    {
                        // Xóa dữ liệu cũ trên giao diện (nếu cần)
                        list_DS_GD.Items.Clear();

                        // Hiển thị danh sách giao dịch lên giao diện
                        foreach (var transaction in transactions)
                        {
                            // Tạo một dòng mới trong ListView để hiển thị thông tin giao dịch
                            var listViewItem = new ListViewItem(customerId); // Hiển thị Mã Khách Hàng (CustomerId)

                            // Thêm các cột khác
                            listViewItem.SubItems.Add(transaction.RelatedTransaction?.PolicyNumber ?? "N/A"); // Số hợp đồng (policy_number) từ RelatedTransaction
                            listViewItem.SubItems.Add(transaction.PaymentType); // Loại thanh toán (payment_type)
                            listViewItem.SubItems.Add(transaction.PaymentId); // Mã giao dịch (MaGD)
                            listViewItem.SubItems.Add(transaction.Amount.ToString("N0")); // Tổng tiền (định dạng với dấu phân cách hàng nghìn)
                            listViewItem.SubItems.Add(transaction.Status); // Trạng thái

                            // Thêm dòng vào ListView
                            list_DS_GD.Items.Add(listViewItem);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy giao dịch nào cho khách hàng này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lấy danh sách giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        // Hàm tìm kiếm theo loại giao dịch
        private async Task SearchByPaymentTypeAsync()
        {
            try
            {
              

                // Lấy loại giao dịch từ UI (giả sử bạn có một ComboBox hoặc TextBox để nhập loại giao dịch)
                string selectedPaymentType = cbo_loaithanhtoan.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedPaymentType))
                {
                    MessageBox.Show("Vui lòng chọn loại giao dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi hàm lấy danh sách giao dịch với loại giao dịch được chọn
                List<PaymentModal> payments = await _transactionDAO.GetContractPaymentsAsync(selectedPaymentType);

                list_DS_GD.Items.Clear();

                foreach (var payment in payments)
                {
                    ListViewItem item = new ListViewItem(payment.CustomerID); // Hiển thị mã khách hàng
                    item.SubItems.Add(payment.RelatedTransaction?.PolicyNumber ?? "N/A"); // Hiển thị mã hợp đồng
                    item.SubItems.Add(payment.PaymentType); // Hiển thị loại hợp đồng
                    item.SubItems.Add(payment.PaymentId); // Hiển thị mã giao dịch
                    item.SubItems.Add(payment.Amount.ToString("N2")); // Hiển thị tổng tiền với 2 chữ số thập phân
                    item.SubItems.Add(payment.Status); // Hiển thị trạng thái giao dịch

                    list_DS_GD.Items.Add(item);
                }

                if (payments.Count == 0)
                {
                    MessageBox.Show($"Không có giao dịch nào với loại giao dịch '{selectedPaymentType}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy danh sách giao dịch: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task SearchByPolicyNumberAsync()
        {
            try
            {
               

                // Lấy mã hợp đồng từ UI (giả sử bạn có một TextBox để nhập mã hợp đồng)
                string policyNumber = txt_maHD.Text;

                if (string.IsNullOrEmpty(policyNumber))
                {
                    MessageBox.Show("Vui lòng nhập mã hợp đồng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi hàm lấy danh sách giao dịch với mã hợp đồng
                List<PaymentModal> payments = await _transactionDAO.GetPaymentsByPolicyNumberAsync(policyNumber);

                list_DS_GD.Items.Clear();

                foreach (var payment in payments)
                {
                    ListViewItem item = new ListViewItem(payment.CustomerID); // Hiển thị mã khách hàng
                    item.SubItems.Add(payment.RelatedTransaction?.PolicyNumber ?? "N/A"); // Hiển thị mã hợp đồng
                    item.SubItems.Add(payment.PaymentType); // Hiển thị loại hợp đồng
                    item.SubItems.Add(payment.PaymentId); // Hiển thị mã giao dịch
                    item.SubItems.Add(payment.Amount.ToString("N2")); // Hiển thị tổng tiền với 2 chữ số thập phân
                    item.SubItems.Add(payment.Status); // Hiển thị trạng thái giao dịch

                    list_DS_GD.Items.Add(item);
                }

                if (payments.Count == 0)
                {
                    MessageBox.Show($"Không có giao dịch nào với mã hợp đồng '{policyNumber}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm danh sách giao dịch: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




            private void radio_MaKH_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị txt_MaKH và vô hiệu hóa các trường còn lại
            txt_MaKH.Enabled = true;

            // Vô hiệu hóa các trường khác
            txt_HoTen.Enabled = false;
            txt_MaGiaoDich.Enabled = false;
            txt_TongTien.Enabled = false;
            cbo_phuongthuc.Enabled = false;
            txt_maHD.Enabled = false;
            txt_trangthai.Enabled = false;
            cbo_loaithanhtoan.Enabled = false;
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_MaKH_TextChanged(object sender, EventArgs e)
        {

        }

        private void radio_MaKH_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void radio_loaiGD_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị txt_MaKH (nếu cần) và vô hiệu hóa các trường khác
            txt_MaKH.Enabled = false; // Vô hiệu hóa txt_MaKH nếu không cần thiết
            txt_HoTen.Enabled = false; // Vô hiệu hóa txt_HoTen
            txt_MaGiaoDich.Enabled = false; // Vô hiệu hóa txt_MaGiaoDich
            txt_TongTien.Enabled = false; // Vô hiệu hóa txt_TongTien
            cbo_phuongthuc.Enabled = false; // Vô hiệu hóa cbo_phuongthuc
            txt_maHD.Enabled = false; // Vô hiệu hóa txt_maHD
            txt_trangthai.Enabled = false; // Vô hiệu hóa txt_trangthai
            cbo_loaithanhtoan.Enabled = true; // Vô hiệu hóa cbo_loaithanhtoan
        }

        private void radio_maSo_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị txt_MaKH (nếu cần) và vô hiệu hóa các trường khác
            txt_MaKH.Enabled = false; // Vô hiệu hóa txt_MaKH nếu không cần thiết
            txt_HoTen.Enabled = false; // Vô hiệu hóa txt_HoTen
            txt_MaGiaoDich.Enabled = false; // Vô hiệu hóa txt_MaGiaoDich
            txt_TongTien.Enabled = false; // Vô hiệu hóa txt_TongTien
            cbo_phuongthuc.Enabled = false; // Vô hiệu hóa cbo_phuongthuc
            txt_maHD.Enabled = true; // Vô hiệu hóa txt_maHD
            txt_trangthai.Enabled = false; // Vô hiệu hóa txt_trangthai
            cbo_loaithanhtoan.Enabled = false; // Vô hiệu hóa cbo_loaithanhtoan
        }

        private void btn_lammoi_Click(object sender, EventArgs e)
        {
            // Đặt giá trị rỗng cho tất cả các trường
            txt_MaKH.Text = string.Empty; // Làm trống txt_MaKH
            txt_HoTen.Text = string.Empty; // Làm trống txt_HoTen
            txt_MaGiaoDich.Text = string.Empty; // Làm trống txt_MaGiaoDich
            txt_TongTien.Text = string.Empty; // Làm trống txt_TongTien
            cbo_phuongthuc.SelectedIndex = -1; // Đặt lại cbo_phuongthuc (không chọn gì)
            txt_maHD.Text = string.Empty; // Làm trống txt_maHD
            txt_trangthai.Text = string.Empty; // Làm trống txt_trangthai
            cbo_loaithanhtoan.SelectedIndex = -1; // Đặt lại cbo_loaithanhtoan (không chọn gì)
                                                  // Vô hiệu hóa các radio button
            txt_MaKH.Enabled = true; // Vô hiệu hóa txt_MaKH nếu không cần thiết
            txt_HoTen.Enabled = true; // Vô hiệu hóa txt_HoTen
            txt_MaGiaoDich.Enabled = true; // Vô hiệu hóa txt_MaGiaoDich
            txt_TongTien.Enabled = true; // Vô hiệu hóa txt_TongTien
            cbo_phuongthuc.Enabled = true; // Vô hiệu hóa cbo_phuongthuc
            txt_maHD.Enabled = true; // Vô hiệu hóa txt_maHD
            txt_trangthai.Enabled = true; // Vô hiệu hóa txt_trangthai
            cbo_loaithanhtoan.Enabled = true; // Vô hiệu hóa cbo_loaithanhtoan
            LoadCustomerListView();

        }
    }
}
