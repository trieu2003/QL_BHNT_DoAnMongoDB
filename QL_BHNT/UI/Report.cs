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
using System.Windows.Forms.DataVisualization.Charting;

namespace QL_BHNT.UI
{
    public partial class Report : Form
    {
        private ReportDAO _reportDAO;
        public Report()
        {
            InitializeComponent();
            _reportDAO = new ReportDAO();
        }
        private void DrawChart(int totalContracts, int totalClaims)
        {
            // Xóa các điểm dữ liệu hiện tại
            chart_TKSL.Series.Clear();

            // Tạo series mới cho biểu đồ hình tròn
            var series = new Series
            {
                Name = "ContractsAndClaims",
                ChartType = SeriesChartType.Pie
            };

            // Thêm các điểm dữ liệu vào series
            series.Points.AddXY("Hợp Đồng", totalContracts);
            series.Points.AddXY("Yêu Cầu Bồi Thường", totalClaims);

            // Thêm series vào biểu đồ
            chart_TKSL.Series.Add(series);

            // Định dạng biểu đồ
            chart_TKSL.Legends.Clear();
            chart_TKSL.Legends.Add(new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center
            });

            // Hiển thị biểu đồ
            chart_TKSL.DataBind();
        }

        private async void btn_TK_Click(object sender, EventArgs e)
        {
            // Lấy tháng và năm từ các điều khiển
            if (!int.TryParse(cbo_TKSL_Thang.SelectedItem?.ToString(), out int month) || month < 1 || month > 12)
            {
                MessageBox.Show("Vui lòng chọn tháng hợp lệ (1-12).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Thoát hàm nếu tháng không hợp lệ
            }

            if (!int.TryParse(txt_TKSL_Năm.Text, out int year) || year < 2000)
            {
                MessageBox.Show("Vui lòng nhập năm hợp lệ (>= 2000).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Thoát hàm nếu năm không hợp lệ
            }

            try
            {
                // Truy vấn tổng số lượng hợp đồng và yêu cầu bồi thường từ ReportDAO
                var totalContracts = await _reportDAO.GetTotalContractsByMonthAndYear(month, year);
                var totalClaims = await _reportDAO.GetTotalClaimsByMonthAndYear(month, year);

                // Kiểm tra giá trị trước khi vẽ biểu đồ và cập nhật ListView
                MessageBox.Show($"Total Contracts: {totalContracts}, Total Claims: {totalClaims}");

                // Hiển thị thông tin trong ListView
                list_TKSL.Items.Clear();
                var itemContracts = new ListViewItem("Hợp Đồng");
                itemContracts.SubItems.Add(totalContracts.ToString());
                list_TKSL.Items.Add(itemContracts);

                var itemClaims = new ListViewItem("Yêu Cầu Bồi Thường");
                itemClaims.SubItems.Add(totalClaims.ToString());
                list_TKSL.Items.Add(itemClaims);

                // Vẽ biểu đồ hình tròn
                DrawChart(totalContracts, totalClaims);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }






}
