using QL_BHNT.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QL_BHNT.UI;

namespace QL_BHNT.DAO
{

    public class PolicyDAO
    {
        private readonly IMongoCollection<CustomerModal> _customersCollection;
        private readonly IMongoCollection<PaymentModal> _PolicyField;

        public PolicyDAO()
        {
            _customersCollection = MongoDBConnection.Instance.GetCollection<CustomerModal>("customer");
            _PolicyField = MongoDBConnection.Instance.GetCollection<PaymentModal>("customer");
        }

        //hiển thị dữ liệu theo MaKH và MaHD
        public async Task<List<CustomerInfoDisplay>> GetCustomerInfo(string customerId, string policyNumber)
        {
            // Tìm khách hàng theo customerId
            var customer = await _customersCollection.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return new List<CustomerInfoDisplay>(); // Trả về danh sách rỗng nếu không tìm thấy
            }

            // Tìm hợp đồng theo policyNumber
            var policy = customer.Policies?.Find(p => p.PolicyNumber == policyNumber);
            if (policy == null)
            {
                return new List<CustomerInfoDisplay>(); // Trả về danh sách rỗng nếu không tìm thấy hợp đồng
            }

            // Tìm người hưởng thụ
            var beneficiary = policy.Beneficiary;

            // Tạo danh sách thông tin khách hàng để hiển thị
            var displayInfo = new List<CustomerInfoDisplay>
            {
                new CustomerInfoDisplay
                {
                    CustomerId = customer.CustomerId,
                    FullName = customer.FullName,
                    DateOfBirth = customer.DateOfBirth,
                    Gender = customer.Gender,
                    PolicyNumber = policy.PolicyNumber,
                    PolicyType = policy.PolicyType,
                    StartDate = policy.StartDate,
                    EndDate = policy.EndDate,
                    PremiumAmount = policy.PremiumAmount,
                    CoverageAmount = policy.CoverageAmount,
                    NextPaymentDate = policy.NextPaymentDate,
                    Status = policy.Status,
                    BeneficiaryFullName = beneficiary?.FullName,
                    BeneficiaryRelationship = beneficiary?.Relationship,
                    BeneficiaryPhone = beneficiary?.ContactInformation?.Phone,
                    BeneficiaryEmail = beneficiary?.ContactInformation?.Email
                }
            };

            return displayInfo; // Trả về danh sách thông tin để sử dụng trong ListView
        }

        // thêm một hợp đồng 
        public async Task<bool> AddPolicyForCustomerAsync(string customerId, string policyType, decimal premiumAmount, decimal coverageAmount, Beneficiary beneficiary)
        {
            // Kiểm tra xem khách hàng có tồn tại hay không
            var customer = await _customersCollection.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return false; // Khách hàng không tồn tại
            }

            // Tạo mã hợp đồng tự động (ví dụ: P987654321)
            string newPolicyNumber = await GeneratePolicyNumberAsync();

            // Ngày bắt đầu là ngày hiện tại
            DateTime startDate = DateTime.UtcNow;

            // Ngày kết thúc là 1 năm sau ngày bắt đầu
            DateTime endDate = startDate.AddYears(1);

            // Tạo đối tượng Policy mới
            var newPolicy = new PolicyModal
            {
                PolicyNumber = newPolicyNumber,
                PolicyType = policyType,
                StartDate = startDate,
                EndDate = endDate,
                PremiumAmount = premiumAmount,
                CoverageAmount = coverageAmount,
                Status = "Đang hoạt động",
                Beneficiary = beneficiary,
                NextPaymentDate = null // Để null
            };

            // Cập nhật thông tin khách hàng để thêm hợp đồng mới
            var update = Builders<CustomerModal>.Update.Push(c => c.Policies, newPolicy);
            await _customersCollection.UpdateOneAsync(c => c.CustomerId == customerId, update);

            return true; // Thành công
        }

        private async Task<string> GeneratePolicyNumberAsync()
        {
            // Lấy tất cả các hợp đồng hiện tại
            var allPolicies = await _customersCollection.Find(Builders<CustomerModal>.Filter.Empty).ToListAsync();

            // Tìm mã hợp đồng lớn nhất để tạo mã mới
            string maxPolicyNumber = allPolicies
                .SelectMany(c => c.Policies)
                .Select(p => p.PolicyNumber)
                .OrderByDescending(pn => pn)
                .FirstOrDefault();

            // Chuyển đổi mã hợp đồng lớn nhất sang số và tạo mã mới
            if (maxPolicyNumber != null)
            {
                string numericPart = maxPolicyNumber.Substring(1); // Lấy phần số
                if (int.TryParse(numericPart, out int number))
                {
                    number++; // Tăng số lên 1
                    return $"P{number:D9}"; // Trả về mã mới với định dạng P000000001
                }
            }

            // Nếu không có hợp đồng nào, trả về mã đầu tiên
            return "P000000001";
        }

        //
        // Xóa hợp đồng theo MaKH và MaHD
        public async Task<bool> DeletePolicyForCustomerAsync(string customerId, string policyNumber)
        {
            // Kiểm tra xem khách hàng có tồn tại hay không
            var customer = await _customersCollection.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return false; // Khách hàng không tồn tại
            }

            // Lọc theo mã khách hàng và mã hợp đồng
            var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);
            var update = Builders<CustomerModal>.Update.PullFilter(c => c.Policies, p => p.PolicyNumber == policyNumber);

            // Cập nhật cơ sở dữ liệu, xóa hợp đồng ra khỏi danh sách
            var result = await _customersCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0; // Trả về true nếu xóa thành công
        }
        // Sửa thông tin hợp đồng dựa trên MaKH và MaHD
        public async Task<bool> UpdatePolicyForCustomerAsync(string customerId, string policyNumber, string policyType, decimal premiumAmount, decimal coverageAmount, string status, Beneficiary beneficiary)
        {
            // Kiểm tra xem khách hàng có tồn tại hay không
            var customer = await _customersCollection.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return false; // Khách hàng không tồn tại
            }

            // Lọc theo mã khách hàng và số hợp đồng
            var filter = Builders<CustomerModal>.Filter.And(
                Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId),
                Builders<CustomerModal>.Filter.Eq("Policies.PolicyNumber", policyNumber)
            );

            // Cập nhật thông tin hợp đồng
            var update = Builders<CustomerModal>.Update
                .Set("Policies.$.PolicyType", policyType)
                .Set("Policies.$.PremiumAmount", premiumAmount)
                .Set("Policies.$.CoverageAmount", coverageAmount)
                .Set("Policies.$.Status", status)
                .Set("Policies.$.Beneficiary", beneficiary);

            // Thực hiện cập nhật
            var result = await _customersCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0; // Trả về true nếu cập nhật thành công
        }
        // Làm mới và lấy danh sách hợp đồng theo MaKH
        public async Task<List<PolicyModal>> RefreshPoliciesForCustomerAsync(string customerId)
        {
            // Tìm khách hàng theo customerId
            var customer = await _customersCollection.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return new List<PolicyModal>(); // Trả về danh sách rỗng nếu không tìm thấy khách hàng
            }
            
            // Trả về danh sách các hợp đồng của khách hàng
            return customer.Policies ?? new List<PolicyModal>();
        }

        public async Task<List<PolicyModal>> GetPoliciesByTypeAsync(string policyType)
        {
            var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, p => p.PolicyType == policyType);
            var customers = await _customersCollection.Find(filter).ToListAsync();

            // Lấy danh sách hợp đồng từ tất cả khách hàng
            var policies = customers.SelectMany(c => c.Policies).ToList();

            return policies;
        }

        // Tìm kiếm hợp đồng theo tên khách hàng
        public async Task<List<PolicyModal>> GetPoliciesByCustomerNameAsync(string customerName)
        {
            // Tìm tất cả khách hàng có tên khớp với customerName
            var customerFilter = Builders<CustomerModal>.Filter.Eq(c => c.FullName, customerName);
            var customers = await _customersCollection.Find(customerFilter).ToListAsync();

            // Lấy danh sách hợp đồng từ tất cả khách hàng
            var policies = customers.SelectMany(c => c.Policies).ToList();

            return policies;
        }

        // Tìm kiếm hợp đồng theo số điện thoại của khách hàng
        public async Task<List<PolicyModal>> GetPoliciesByPhoneNumberAsync(string phoneNumber)
        {
            // Tìm tất cả khách hàng có số điện thoại khớp với phoneNumber
            var customerFilter = Builders<CustomerModal>.Filter.Eq(c => c.ContactInformation.Phone, phoneNumber);
            var customers = await _customersCollection.Find(customerFilter).ToListAsync();

            // Lấy danh sách hợp đồng từ tất cả khách hàng
            var policies = customers.SelectMany(c => c.Policies).ToList();

            return policies;
        }

        // Tìm kiếm hợp đồng theo loại hợp đồng
        public async Task<List<PolicyModal>> GetPoliciesByPolicyTypeAsync(string policyType)
        {
            // Tìm tất cả khách hàng có hợp đồng với policyType khớp
            var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, p => p.PolicyType == policyType);
            var customers = await _customersCollection.Find(filter).ToListAsync();

            // Lấy danh sách hợp đồng từ tất cả khách hàng
            // Không cần điều kiện thêm, vì đã lọc với policyType ở trên
            var policies = customers.SelectMany(c => c.Policies).ToList();

            return policies;
        }


    }

}
