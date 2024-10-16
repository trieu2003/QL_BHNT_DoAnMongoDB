using MongoDB.Driver;
using QL_BHNT.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_BHNT.DAO
{
    public class EmployeeDAO
    {
        private readonly IMongoCollection<employeeModal> _employeeCollection;

        public EmployeeDAO()
        {
          
            _employeeCollection = MongoDBConnection.Instance.GetCollection<employeeModal>("employee");
        }

        // Thêm một nhân viên mới
        public async Task AddEmployeeAsync(employeeModal employee)
        {
            await _employeeCollection.InsertOneAsync(employee);
        }

        // Lấy tất cả nhân viên
        public async Task<List<employeeModal>> GetAllEmployeesAsync()
        {
            return await _employeeCollection.Find(_ => true).ToListAsync();
        }

        // Tìm nhân viên theo ID
        public async Task<employeeModal> GetEmployeeByIdAsync(string id)
        {
            return await _employeeCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        // Xóa nhân viên theo ID
        public async Task DeleteEmployeeByIdAsync(string id)
        {
            await _employeeCollection.DeleteOneAsync(e => e.Id == id);
        }

        // Xác nhận đăng nhập
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var filter = Builders<employeeModal>.Filter.Eq(e => e.Username, username) &
                         Builders<employeeModal>.Filter.Eq(e => e.Password, password);
            var employee = await _employeeCollection.Find(filter).FirstOrDefaultAsync();
            return employee != null;
        }
        // Hàm lấy employee_id và position theo username
        public async Task<(string EmployeeId, string Position)> GetEmployeeInfoByUsernameAsync(string username)
        {
            try
            {
                // Tạo bộ lọc để tìm kiếm username
                var filter = Builders<employeeModal>.Filter.Eq(emp => emp.Username, username);

                // Thực hiện truy vấn tìm nhân viên với username tương ứng
                var employee = await _employeeCollection.Find(filter).FirstOrDefaultAsync();

                // Kiểm tra nếu không tìm thấy nhân viên
                if (employee == null)
                {
                    throw new Exception($"Không tìm thấy nhân viên với username: {username}");
                }

                // Trả về employee_id và position
                return (employee.EmployeeId, employee.Position);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Error retrieving employee information.", ex);
            }
        }
        }
}

