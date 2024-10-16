using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QL_BHNT.Modal;

namespace QL_BHNT.DAO
{
    public class CustomerDAO
    {
        private readonly IMongoCollection<CustomerModal> _customersCollection;

        public CustomerDAO()
        {
            _customersCollection = MongoDBConnection.Instance.GetCollection<CustomerModal>("customer");
        }

        //hiển thị danh sách khách hàng 
        public async Task<List<CustomerModal>> GetCustomerListAsync()
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.Status, "active");
                var customers = await _customersCollection.Find(filter).ToListAsync();
                return customers;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        // Hàm tìm khách hàng có status là 'active' và mã đại lý tương ứng
        public async Task<List<CustomerModal>> GetActiveCustomersByAgentIdAsync(string agentId)
        {
            try
            {

                var filter = Builders<CustomerModal>.Filter.And(
                    Builders<CustomerModal>.Filter.Eq(c => c.Status, "active"),
                    Builders<CustomerModal>.Filter.Eq(c => c.AgentId, agentId)
                );
                var customers = await _customersCollection.Find(filter).ToListAsync();
                return customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        // Hàm lấy thông tin khách hàng dựa trên mã khách hàng
        public async Task<CustomerModal> GetCustomerInfoByCustomerIdAsync(string customerId)
        {
            try
            {
                // Tạo filter để tìm khách hàng dựa trên mã khách hàng
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);

                // Thực hiện truy vấn để tìm khách hàng
                var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

                // Trả về thông tin khách hàng nếu tìm thấy, hoặc null nếu không tìm thấy
                return customer;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và thông báo lỗi
                throw new Exception("Lỗi khi lấy thông tin khách hàng.", ex);
            }
        }

        public async Task<bool> CustomerIdExistsAsync(string customerId)
        {
            var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
            var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();
            return customer != null;
        }

        public async Task AddCustomerAsync(CustomerModal customer)
        {
            try
            {
                // Kiểm tra xem mã khách hàng đã tồn tại chưa trước khi thêm
                bool exists = await _customersCollection.Find(c => c.CustomerId == customer.CustomerId).AnyAsync();
                if (exists)
                {
                   // throw new Exception("Mã khách hàng đã tồn tại.");
                }

                await _customersCollection.InsertOneAsync(customer);
            }
            catch (MongoBulkWriteException ex)
            {
                // Xử lý lỗi DuplicateKey
                if (ex.WriteErrors.Any(e => e.Code == 11000))
                {
                    throw new Exception("Trùng mã khách hàng hoặc _id.", ex);
                }
                throw new Exception("Lỗi khi thêm khách hàng.", ex);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khác
               // throw new Exception("Lỗi khi thêm khách hàng.", ex);
            }
        }



        public async Task UpdateCustomerStatusAsync(string customerId, string status)
        {
            var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
            var update = Builders<CustomerModal>.Update.Set(c => c.Status, status);
            await _customersCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateCustomerAsync(CustomerModal updatedCustomer)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, updatedCustomer.CustomerId);
                var update = Builders<CustomerModal>.Update
                    .Set(c => c.FullName, updatedCustomer.FullName)
                    .Set(c => c.DateOfBirth, updatedCustomer.DateOfBirth)
                    .Set(c => c.Gender, updatedCustomer.Gender)
                    .Set(c => c.ContactInformation.Phone, updatedCustomer.ContactInformation.Phone)
                    .Set(c => c.ContactInformation.Email, updatedCustomer.ContactInformation.Email)
                    .Set(c => c.ContactInformation.Address, updatedCustomer.ContactInformation.Address)
                    .Set(c => c.Status, updatedCustomer.Status)
                    .Set(c => c.AgentId, updatedCustomer.AgentId);

                await _customersCollection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật khách hàng.", ex);
            }
        }

        public async Task<List<CustomerModal>> GetCustomersByNameAsync(string name)
        {
            var filter = Builders<CustomerModal>.Filter.Regex(c => c.FullName, new BsonRegularExpression(name, "i"));
            return await _customersCollection.Find(filter).ToListAsync();
        }

    }
}
