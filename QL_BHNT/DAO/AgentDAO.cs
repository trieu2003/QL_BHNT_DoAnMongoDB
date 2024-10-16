using MongoDB.Bson;
using MongoDB.Driver;
using QL_BHNT.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_BHNT.DAO
{
    public class AgentDAO
    {
        private readonly IMongoCollection<AgentModal> _agentsCollection;

        public AgentDAO()
        {
            _agentsCollection = MongoDBConnection.Instance.GetCollection<AgentModal>("agent");
        }

        // Hiển thị danh sách đại lý
        public async Task<List<AgentModal>> GetAgentListAsync()
        {
            try
            {
                // Không sử dụng bộ lọc để lấy tất cả các đại lý
                var agents = await _agentsCollection.Find(FilterDefinition<AgentModal>.Empty).ToListAsync();
                return agents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        // Hàm lấy thông tin đại lý dựa trên mã đại lý
        public async Task<AgentModal> GetAgentInfoByAgentIdAsync(string agentId)
        {
            try
            {
                // Tạo filter để tìm đại lý dựa trên mã đại lý
                var filter = Builders<AgentModal>.Filter.Eq(a => a.AgentId, agentId);

                // Thực hiện truy vấn để tìm đại lý
                var agent = await _agentsCollection.Find(filter).FirstOrDefaultAsync();

                // Trả về thông tin đại lý nếu tìm thấy, hoặc null nếu không tìm thấy
                return agent;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và thông báo lỗi
                throw new Exception("Lỗi khi lấy thông tin đại lý.", ex);
            }
        }

        public async Task<List<string>> GetAgentIdsAsync()
        {
            try
            {
                // Chỉ lấy các trường AgentId từ cơ sở dữ liệu
                var agentIds = await _agentsCollection
                    .Find(FilterDefinition<AgentModal>.Empty)
                    .Project(a => a.AgentId) // Chỉ lấy trường AgentId
                    .ToListAsync();

                return agentIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách mã đại lý: {ex.Message}");
                return new List<string>();
            }
        }




        // Kiểm tra xem mã đại lý có tồn tại không
        public async Task<bool> AgentIdExistsAsync(string agentId)
        {
            var filter = Builders<AgentModal>.Filter.Eq(a => a.AgentId, agentId);
            var agent = await _agentsCollection.Find(filter).FirstOrDefaultAsync();
            return agent != null;
        }

        public async Task<bool> UpdateAgentInfoAsync(AgentModal agent)
        {
            try
            {
                // Tạo filter dựa trên mã đại lý
                var filter = Builders<AgentModal>.Filter.Eq(a => a.AgentId, agent.AgentId);

                // Tạo bản cập nhật với các field mới, trừ mã đại lý
                var update = Builders<AgentModal>.Update
                    .Set(a => a.Name, agent.Name)
                    .Set(a => a.ContactInfo.Address, agent.ContactInfo.Address)
                    .Set(a => a.ContactInfo.Email, agent.ContactInfo.Email)
                    .Set(a => a.ContactInfo.Phone, agent.ContactInfo.Phone);

                // Thực hiện cập nhật trong MongoDB
                var result = await _agentsCollection.UpdateOneAsync(filter, update);

                // Trả về true nếu có bản ghi được cập nhật
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật đại lý: {ex.Message}");
                return false;
            }
        }

        public async Task<List<AgentModal>> GetAgentsByAddressAsync(string address)
        {
            var filter = Builders<AgentModal>.Filter.Regex(a => a.ContactInfo.Address, new BsonRegularExpression(address, "i"));
            return await _agentsCollection.Find(filter).ToListAsync();
        }

        // Tìm đại lý theo tên (sử dụng Regex để tìm gần trùng)
        public async Task<List<AgentModal>> GetAgentsByNameAsync(string name)
        {
            var filter = Builders<AgentModal>.Filter.Regex(a => a.Name, new BsonRegularExpression(name, "i"));
            return await _agentsCollection.Find(filter).ToListAsync();
        }




    }
}
