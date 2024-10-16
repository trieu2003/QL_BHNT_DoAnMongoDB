using MongoDB.Driver;
using QL_BHNT.Modal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QL_BHNT.DAO
{
    public  class ClaimDAO
    {

        private readonly IMongoCollection<CustomerModal> _customersCollection;


        public ClaimDAO()
        {
            _customersCollection = MongoDBConnection.Instance.GetCollection<CustomerModal>("customer");
        }
        //load dữ liệu kiểu dữ liệu
        public async Task<List<Claim>> GetClaimsListFromMongoDBAsync()
        {
            try
            {
                // Lấy tất cả các khách hàng có yêu cầu bồi thường
                var filter = Builders<CustomerModal>.Filter.Exists(c => c.Claims);
                var customersWithClaims = await _customersCollection.Find(filter).ToListAsync();
                
                // Tạo danh sách yêu cầu bồi thường
                List<Claim> claimsList = new List<Claim>();

                foreach (var customer in customersWithClaims)
                {
                    if (customer.Claims != null)
                    {
                        claimsList.AddRange(customer.Claims);
                    }
                }

                return claimsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu yêu cầu bồi thường từ MongoDB: {ex.Message}");
                return new List<Claim>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }
        ///hiển thị thông tin yêu cầu bồi thường
        ///
        public async Task<CustomerModal> GetCustomerDetailsAsync(string customerId)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
                var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

                if (customer != null)
                {
                    // Lấy thông tin chi tiết về hợp đồng và yêu cầu bồi thường
                    return customer;
                }

                return null; // Không tìm thấy khách hàng
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Error retrieving customer details: {ex.Message}");
                throw; // Ném lại lỗi để xử lý ở nơi gọi
            }
        }
      //  THÊM
        public async Task<bool> AddClaimAsync(string customerId, Claim newClaim)
        {
            try
            {
                // Tìm khách hàng theo mã khách hàng
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
                var update = Builders<CustomerModal>.Update.Push(c => c.Claims, newClaim);

                // Cập nhật danh sách yêu cầu bồi thường cho khách hàng
                var result = await _customersCollection.UpdateOneAsync(filter, update);

                return result.ModifiedCount > 0; // Trả về true nếu đã cập nhật thành công
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Error adding claim: {ex.Message}");
                throw; // Ném lại lỗi để xử lý ở nơi gọi
            }
        }

        public (string CustomerId, string FullName, string PolicyNumber, string PolicyType, decimal PremiumAmount) DisplayContractInformation(string policyNumber)
        {
            // Tìm kiếm khách hàng dựa vào số hợp đồng
            var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, policy => policy.PolicyNumber == policyNumber);
            var customer = _customersCollection.Find(filter).FirstOrDefault();

            if (customer != null)
            {
                // Lấy thông tin hợp đồng từ danh sách Policies
                var policy = customer.Policies.Find(p => p.PolicyNumber == policyNumber);
                if (policy != null)
                {
                    return (customer.CustomerId, customer.FullName, policy.PolicyNumber, policy.PolicyType, policy.PremiumAmount);
                }
            }

            return (null, null, null, null, 0); // Trả về null và 0 nếu không tìm thấy thông tin
        }

        //xoa s
        // Xóa yêu cầu bồi thường
        public async Task<bool> DeleteClaimAsync(string customerId, string claimId)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.And(
                    Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId),
                    Builders<CustomerModal>.Filter.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == claimId)
                );

                var update = Builders<CustomerModal>.Update.Set("Claims.$.Status", "Đã hủy");

                var result = await _customersCollection.UpdateOneAsync(filter, update);

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating claim status: {ex.Message}");
                throw;
            }
        }


        // Sửa yêu cầu bồi thường
        //public async Task<bool> UpdateClaimAsync(string customerId, Claim updatedClaim)
        //{
        //    try
        //    {
        //        // Bước 1: Lấy khách hàng dựa trên customerId
        //        var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
        //        var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

        //        if (customer != null && customer.Claims != null)
        //        {
        //            // Bước 2: Tìm yêu cầu bồi thường cần cập nhật
        //            var claimToUpdate = customer.Claims.FirstOrDefault(cl => cl.ClaimNumber == updatedClaim.ClaimNumber);
        //            if (claimToUpdate != null)
        //            {
        //                // Cập nhật các trường cần thiết
        //                // Ví dụ về trường cần cập nhật
        //                claimToUpdate.Status = updatedClaim.Status; // Cập nhật trạng thái
        //                                                            // Cập nhật các trường khác nếu cần

        //                // Bước 3: Cập nhật lại danh sách yêu cầu bồi thường
        //                var update = Builders<CustomerModal>.Update.Set(c => c.Claims, customer.Claims);
        //                var result = await _customersCollection.UpdateOneAsync(filter, update);

        //                return result.ModifiedCount > 0; // Trả về true nếu đã cập nhật thành công
        //            }
        //        }

        //        return false; // Không tìm thấy khách hàng hoặc yêu cầu bồi thường
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating claim: {ex.Message}");
        //        throw; // Ném lại lỗi để xử lý ở nơi gọi
        //    }
        //}
        // Tìm kiếm yêu cầu bồi thường theo ID yêu cầu
        public async Task<Claim> SearchClaimByIdAsync(string claimId)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == claimId);
                var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

                return customer?.Claims?.Find(c => c.ClaimNumber == claimId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching claim by ID: {ex.Message}");
                throw;
            }
        }


        public async Task UpdateClaimAsync(string customerId, string claimNumber, Claim updatedClaim)
        {
            // Tạo bộ lọc để tìm kiếm khách hàng với mã khách hàng
            var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);

            // Lấy khách hàng từ cơ sở dữ liệu
            var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

            if (customer != null)
            {
                // Tìm yêu cầu bồi thường trong danh sách yêu cầu của khách hàng
                var existingClaim = customer.Claims?.FirstOrDefault(c => c.ClaimNumber == claimNumber);

                if (existingClaim != null)
                {
                    // Cập nhật thông tin của yêu cầu bồi thường
                    existingClaim.ClaimDate = updatedClaim.ClaimDate;
                    existingClaim.Status = updatedClaim.Status;
                    existingClaim.ClaimAmount = updatedClaim.ClaimAmount;
                    existingClaim.Documents = updatedClaim.Documents;

                    // Ghi đè lại danh sách yêu cầu bồi thường vào cơ sở dữ liệu
                    var update = Builders<CustomerModal>.Update.Set(c => c.Claims, customer.Claims);
                    await _customersCollection.UpdateOneAsync(filter, update);
                }
            }
        }




        // Tìm kiếm yêu cầu bồi thường theo ID khách hàng
        public async Task<List<Claim>> SearchClaimsByCustomerIdAsync(string customerId)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
                var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

                return customer?.Claims ?? new List<Claim>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching claims by customer ID: {ex.Message}");
                throw;
            }
        }

        // Tìm kiếm yêu cầu bồi thường theo ID yêu cầu
        //public async Task<Claim> SearchClaimByIdAsync(string claimId)
        //{
        //    try
        //    {
        //        var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == claimId);
        //        var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

        //        return customer?.Claims?.Find(c => c.ClaimNumber == claimId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error searching claim by ID: {ex.Message}");
        //        throw;
        //    }
        //}
        public async Task<List<ClaimDisplay>> SearchClaimsAsync(string loaiBH, string maKH, string maBH)
        {
            var filterBuilder = Builders<CustomerModal>.Filter;
            var filter = filterBuilder.Empty; // Bắt đầu với filter rỗng

            // Thêm điều kiện loại bảo hiểm
            if (!string.IsNullOrWhiteSpace(loaiBH))
            {
                filter = filterBuilder.And(filter,
                    filterBuilder.ElemMatch(c => c.Policies, p => p.PolicyType == loaiBH));
            }

            // Thêm điều kiện mã khách hàng
            if (!string.IsNullOrWhiteSpace(maKH))
            {
                filter = filterBuilder.And(filter,
                    filterBuilder.Eq(c => c.CustomerId, maKH));
            }

            // Thêm điều kiện mã bảo hiểm
            if (!string.IsNullOrWhiteSpace(maBH))
            {
                filter = filterBuilder.And(filter,
                    filterBuilder.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == maBH));
            }

            // Tìm kiếm khách hàng theo filter
            var customers = await _customersCollection.Find(filter).ToListAsync();

            List<ClaimDisplay> resultClaims = new List<ClaimDisplay>();

            // Lấy danh sách yêu cầu bồi thường từ khách hàng tìm được
            foreach (var customer in customers)
            {
                if (customer.Claims != null)
                {
                    foreach (var claim in customer.Claims)
                    {
                        // Tạo đối tượng mới chứa thông tin cần hiển thị
                        resultClaims.Add(new ClaimDisplay
                        {
                            ClaimNumber = claim.ClaimNumber,
                            ClaimDate = claim.ClaimDate,
                            ClaimAmount = claim.ClaimAmount,
                            Status = claim.Status,
                            CustomerId = customer.CustomerId // Lấy mã khách hàng từ customer
                        });
                    }
                }
            }

            return resultClaims;
        }



        //tìm theo loại hợp đồng 
        public async Task<List<CustomerModal>> GetClaimsByPolicyTypeAsync(string policyType)
        {
            try
            {
                // Lọc các khách hàng có hợp đồng với loại bảo hiểm phù hợp
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, p => p.PolicyType == policyType);
                var customers = await _customersCollection.Find(filter).ToListAsync();

                return customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving claims by policy type: {ex.Message}");
                throw;
            }
        }

        //Tìm kiếm theo mã hợp đồng 
        public async Task<List<CustomerModal>> GetClaimsByPolicyNumberAsync(string policyNumber)
        {
            try
            {
                // Tìm kiếm khách hàng có hợp đồng với PolicyNumber khớp
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, p => p.PolicyNumber == policyNumber);
                var customers = await _customersCollection.Find(filter).ToListAsync();

                return customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving claims by policy number: {ex.Message}");
                throw;
            }
        }
        //tìm kiếm theo mã khách hàng 
        public async Task<List<CustomerModal>> GetCustomerByIdAsync(string customerId)
        {
            try
            {
                // Tìm kiếm khách hàng theo CustomerId
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
                var customers = await _customersCollection.Find(filter).ToListAsync();

                return customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customer by ID: {ex.Message}");
                throw;
            }
        }

    }
}



