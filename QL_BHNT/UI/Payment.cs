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
using QL_BHNT.Modal;

namespace QL_BHNT.UI
{
    public partial class Payment : Form
    {
        private string _employeeId;
        private PaymentDAO _paymentDAO;
        public Payment(string employeeId)
        {
            InitializeComponent();
            // Lưu employeeId vào biến thành viên
            _employeeId = employeeId;
            _paymentDAO = new PaymentDAO();

            // Kiểm tra giá trị được truyền vào
            if (!string.IsNullOrEmpty(_employeeId))
            {
                // Hiển thị giá trị để đảm bảo dữ liệu được truyền đúng
                MessageBox.Show($"Employee ID: {_employeeId}", "Debug");
            }
            else
            {
                MessageBox.Show("Employee ID không được truyền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public Payment()
        {
            // Constructor mặc định không yêu cầu tham số
        }
        private void Payment_Load(object sender, EventArgs e)
        {
            // Hiển thị employeeId lên txt_id_employee
            txt_id_employee.Text = _employeeId;
        }
        
        private async void txt_maso_HD_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra nếu phím Enter được nhấn
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // Ngăn chặn hành động mặc định của Enter
                e.SuppressKeyPress = true; // Ngăn tiếng beep khi nhấn Enter

                // Lấy mã số hợp đồng từ TextBox
                string policyNumber = txt_maso_HD.Text;

                if (!string.IsNullOrEmpty(policyNumber))
                {
                    // Gọi hàm để lấy thông tin hợp đồng và khách hàng
                    var paymentDAO = new PaymentDAO();
                    var customer = await paymentDAO.GetPolicyByNumberAsync(policyNumber); // Sử dụng await

                    // Kiểm tra nếu tìm thấy hợp đồng
                    if (customer != null)
                    {
                        // Tìm hợp đồng phù hợp trong danh sách hợp đồng của khách hàng
                        var policy = customer.Policies.FirstOrDefault(p => p.PolicyNumber == policyNumber);

                        if (policy != null)
                        {
                            // Hiển thị thông tin hợp đồng lên các TextBox và DateTimePicker
                            txt_loaihD.Text = policy.PolicyType;
                            dateTime_BD.Value = policy.StartDate;
                            dateTime_KT.Value = policy.EndDate;
                            txt_premium_amount.Text = policy.PremiumAmount.ToString();
                            txt_coverage_amount.Text = policy.CoverageAmount.ToString();
                            txt_tongtien.Text = policy.PremiumAmount.ToString();

                            // Hiển thị thông tin khách hàng lên các TextBox
                            txt_makh.Text = customer.CustomerId;
                            txt_hoten.Text = customer.FullName;
                            datetime_ngaysinh.Value = customer.DateOfBirth;

                            // Hiển thị giới tính lên RadioButton
                            if (customer.Gender == "Nam")
                            {
                                radio_nam.Checked = true;
                                radio_Nu.Checked = false;
                            }
                            else if (customer.Gender == "Nữ")
                            {
                                radio_nam.Checked = false;
                                radio_Nu.Checked = true;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy hợp đồng với mã số đã nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private async void Payment_Load_1(object sender, EventArgs e)
        {
            // Đảm bảo rằng biến _employeeId đã có giá trị trước khi hiển thị
            if (!string.IsNullOrEmpty(_employeeId))
            {
                txt_id_employee.Text = _employeeId;
            }
            else
            {
                MessageBox.Show("Không có mã nhân viên để hiển thị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // Lấy mã thanh toán mới không trùng
            string newPaymentId = await _paymentDAO.GetNewPaymentIdAsync();

            // Hiển thị mã thanh toán mới lên TextBox (hoặc bất kỳ control nào)
            txt_mathanhtoan.Text = newPaymentId;
        }

        private async void txt_MaBT_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra nếu phím Enter được nhấn
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // Ngăn chặn hành động mặc định của Enter
                e.SuppressKeyPress = true; // Ngăn tiếng beep khi nhấn Enter

                // Lấy mã yêu cầu bồi thường từ TextBox
                string claimNumber = txt_MaBT.Text;

                if (!string.IsNullOrEmpty(claimNumber))
                {
                    try
                    {
                        // Tạo đối tượng DAO để lấy thông tin yêu cầu bồi thường
                        var claimDAO = new PaymentDAO();
                        var claim = await claimDAO.GetClaimByNumberAsync(claimNumber);

                        if (claim != null)
                        {
                            // Lấy mã số hợp đồng tương ứng
                            var policyNumber = claim.PolicyNumber;

                            // Tạo đối tượng DAO để lấy thông tin hợp đồng
                            var paymentDAO = new PaymentDAO();
                            var customer = await paymentDAO.GetPolicyByNumberAsync(policyNumber);

                            if (customer != null)
                            {
                                // Tìm hợp đồng phù hợp trong danh sách hợp đồng của khách hàng
                                var policy = customer.Policies.FirstOrDefault(p => p.PolicyNumber == policyNumber);

                                if (policy != null)
                                {
                                    // Hiển thị thông tin lên các TextBox và DateTimePicker về yêu cầu hợp đồng 
                                    txt_SoHD.Text = policy.PolicyNumber;
                                    datetime_ngayBT.Value = claim.ClaimDate;
                                    txt_loaiHD_BT.Text = policy.PolicyType;
                                    txt_claim_amount.Text = claim.ClaimAmount.ToString();
                                    txt_tongtien.Text = claim.ClaimAmount.ToString();

                                    // Hiển thị thông tin khách hàng lên các TextBox
                                    txt_makh.Text = customer.CustomerId;
                                    txt_hoten.Text = customer.FullName;
                                    datetime_ngaysinh.Value = customer.DateOfBirth;

                                    // Hiển thị giới tính lên RadioButton
                                    if (customer.Gender == "Nam")
                                    {
                                        radio_nam.Checked = true;
                                        radio_Nu.Checked = false;
                                    }
                                    else if (customer.Gender == "Nữ")
                                    {
                                        radio_nam.Checked = false;
                                        radio_Nu.Checked = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy yêu cầu bồi thường với mã số đã nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi lấy thông tin yêu cầu bồi thường: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Đảm bảo giữ lại giá trị trong txt_MaBT sau khi nhấn Enter
                    txt_MaBT.Focus();
                    txt_MaBT.SelectionStart = txt_MaBT.Text.Length;
                }
            }
        }




        private void radio_DongBaoHiem_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_DongBaoHiem.Checked)
            {
                // Vô hiệu hóa các điều khiển liên quan đến yêu cầu bồi thường
                txt_SoHD.Enabled = false;
                datetime_ngayBT.Enabled = false;
                txt_loaiHD_BT.Enabled = false;
                txt_claim_amount.Enabled = false;

             
            }
        }

        private void radio_yeucauboithuong_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_yeucauboithuong.Checked)
            {
                // Vô hiệu hóa các điều khiển liên quan đến hợp đồng
                txt_loaihD.Enabled = false;
                dateTime_BD.Enabled = false;
                dateTime_KT.Enabled = false;
                txt_premium_amount.Enabled = false;
                txt_coverage_amount.Enabled = false;

                
            }
        }

        private async void bnt_ThanhToan_Click(object sender, EventArgs e)
        {
            // Lấy giá trị từ các trường trong form
            string customerId = txt_makh.Text;
            decimal amount = decimal.Parse(txt_tongtien.Text);
            string method = cbo_phuongthuc.SelectedItem.ToString();
            DateTime paymentDate = dateTime_GD.Value; // Lấy giá trị từ DateTimePicker
            string employeeId = txt_id_employee.Text; // Lấy employeeId từ một TextBox (có thể thay đổi tùy theo form của bạn)
            string policyNumber = txt_maso_HD.Text; // Lấy policy number từ TextBox (thay đổi tùy theo form của bạn)

            try
            {
                // Kiểm tra loại thanh toán (hợp đồng hoặc yêu cầu bồi thường)
                if (radio_DongBaoHiem.Checked)
                {
                    // Thanh toán hợp đồng (policy)
                    string paymentId = await _paymentDAO.GetNewPaymentIdAsync();

                    var newPayment = new PaymentModal
                    {
                        EmployeeId = employeeId,
                        PaymentId = paymentId,
                        PaymentDate = paymentDate,
                        Amount = amount,
                        Method = method,
                        Status = "Hoàn tất",
                        PaymentType = "Hợp Đồng" ,// Loại thanh toán hợp đồng
                         RelatedTransaction = new RelatedTransaction
                         {
                             PolicyNumber = txt_maso_HD.Text // Gán số hợp đồng vào đây
                         }
                    };

                    // Thêm thanh toán cho hợp đồng của khách hàng
                    await _paymentDAO.AddPaymentAsync(customerId, policyNumber, newPayment, employeeId); // Chuyển policyNumber vào đây

                    MessageBox.Show("Thanh toán hợp đồng đã được thêm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (radio_yeucauboithuong.Checked)
                {
                    // Thanh toán yêu cầu bồi thường (claim)
                    string claimNumber = txt_MaBT.Text; // Số hợp đồng bồi thường
                    string paymentId = await _paymentDAO.GetNewPaymentIdAsync();

                    var newPayment = new PaymentModal
                    {
                        EmployeeId = employeeId,
                        PaymentId = paymentId,
                        PaymentDate = paymentDate,
                        Amount = amount,
                        Method = method,
                        Status = "Hoàn tất",
                        PaymentType = "Yêu Cầu Bồi Thường", // Loại thanh toán yêu cầu bồi thường
                        RelatedTransaction = new RelatedTransaction
                        {
                            PolicyNumber = claimNumber // Gán số hợp đồng vào đây
                        }
                    };

                    // Thực hiện thanh toán cho yêu cầu bồi thường
                    await _paymentDAO.AddClaimPaymentAsync(customerId, claimNumber, newPayment, employeeId); // Chuyển employeeId vào đây

                    MessageBox.Show("Thanh toán yêu cầu bồi thường đã được thêm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn loại thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm thanh toán: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
