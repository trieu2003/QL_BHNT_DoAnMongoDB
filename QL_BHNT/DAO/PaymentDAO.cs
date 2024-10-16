using MongoDB.Bson;
using MongoDB.Driver;
using QL_BHNT.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;


namespace QL_BHNT.DAO
{
    public  class PaymentDAO
    {
        private readonly IMongoCollection<CustomerModal> _customerCollection;

        public PaymentDAO()
        {
            // Kết nối tới MongoDB và lấy collection Customer
            _customerCollection = MongoDBConnection.Instance.GetCollection<CustomerModal>("customer");
        }
        // Hàm lấy thông tin hợp đồng dựa trên mã số hợp đồng
        public async Task<CustomerModal> GetPolicyByNumberAsync(string policyNumber)
        {
            try
            {
                // Lọc khách hàng có hợp đồng với mã số hợp đồng trùng khớp
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, p => p.PolicyNumber == policyNumber);

                // Tìm khách hàng và lấy hợp đồng có mã số hợp đồng phù hợp
                var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();

                if (customer != null)
                {
                    // Tìm hợp đồng với mã số hợp đồng tương ứng
                    var policy = customer.Policies.FirstOrDefault(p => p.PolicyNumber == policyNumber);

                    // Nếu tìm thấy hợp đồng, trả về đối tượng khách hàng
                    if (policy != null)
                    {
                        return customer;
                    }
                }

                return null; // Nếu không tìm thấy, trả về null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin hợp đồng: {ex.Message}");
                throw;
            }
        }
        //lấy thông tin yêu cầu bồi thường 
        public async Task<Claim> GetClaimByNumberAsync(string claimNumber)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == claimNumber);
                var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();

                if (customer != null)
                {
                    var claim = customer.Claims.FirstOrDefault(cl => cl.ClaimNumber == claimNumber);
                    return claim;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin yêu cầu bồi thường: {ex.Message}");
                throw;
            }
        }
        // Hàm lấy tất cả các mã thanh toán (payment_id) từ MongoDB
        public async Task<List<string>> GetAllPaymentIdsAsync()
        {
            var paymentIds = new List<string>();

            // Truy vấn tất cả các customer có thanh toán
            var customers = await _customerCollection.Find(Builders<CustomerModal>.Filter.Exists(c => c.Payments)).ToListAsync();

            // Lấy tất cả các payment_id
            foreach (var customer in customers)
            {
                if (customer.Payments != null)
                {
                    paymentIds.AddRange(customer.Payments.Select(p => p.PaymentId));
                }
            }

            return paymentIds;
        }

        // Hàm tạo mã thanh toán mới không trùng
        public async Task<string> GetNewPaymentIdAsync()
        {
            List<string> existingPaymentIds = await GetAllPaymentIdsAsync();

            // Tìm mã thanh toán lớn nhất hiện có
            int maxIdNumber = 0;
            foreach (var paymentId in existingPaymentIds)
            {
                if (paymentId.StartsWith("P") && paymentId.Length > 1)
                {
                    if (int.TryParse(paymentId.Substring(1), out int idNumber))
                    {
                        maxIdNumber = Math.Max(maxIdNumber, idNumber);
                    }
                }
            }

            // Tạo mã mới
            int newIdNumber = maxIdNumber + 1;
            string newPaymentId = $"P{newIdNumber:D3}"; // Tạo mã với định dạng "Pxxx"

            return newPaymentId;
        }

        // Hàm thêm một khoản thanh toán vào trường Payments của khách hàng
        //public async Task AddPaymentAsync(string customerId, PaymentModal newPayment)
        //{
        //    // Tạo bộ lọc để tìm khách hàng theo customerId
        //    var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);

        //    // Tạo đối tượng cập nhật để thêm Payment vào danh sách Payments
        //    var updatePayment = Builders<CustomerModal>.Update.Push(c => c.Payments, newPayment);

        //    // Cập nhật ngày thanh toán tiếp theo (next_payment_date) của tất cả các Policy
        //    var updateNextPaymentDate = Builders<CustomerModal>.Update.Combine(
        //        Builders<CustomerModal>.Update.Set(c => c.Policies[-1].NextPaymentDate, newPayment.PaymentDate.AddYears(1))
        //    );

        //    try
        //    {
        //        // Thực hiện cập nhật Payment
        //        var paymentResult = await _customerCollection.UpdateOneAsync(filter, updatePayment);

        //        if (paymentResult.ModifiedCount == 0)
        //        {
        //            // Nếu không có tài liệu nào bị sửa đổi, có thể không có khách hàng với customerId này
        //            throw new Exception("Không tìm thấy khách hàng với mã số khách hàng đã cho.");
        //        }

        //        // Thực hiện cập nhật ngày thanh toán tiếp theo cho tất cả các Policies
        //        var nextPaymentResult = await _customerCollection.UpdateManyAsync(
        //            filter,
        //            Builders<CustomerModal>.Update.Set("policies.$[].next_payment_date", newPayment.PaymentDate.AddYears(1))
        //        );

        //        if (nextPaymentResult.ModifiedCount == 0)
        //        {
        //            throw new Exception("Không thể cập nhật ngày thanh toán tiếp theo.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi (có thể log lỗi hoặc thông báo cho người dùng)
        //        throw new Exception($"Lỗi khi thêm thanh toán: {ex.Message}", ex);
        //    }

        //}
        //

        public async Task AddPaymentAsync(string customerId, string policyNumber, PaymentModal newPayment, string employeeId)
        {
            // Tạo bộ lọc để tìm khách hàng theo customerId
            var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);

            // Lọc hợp đồng theo policyNumber
            var policyFilter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies, p => p.PolicyNumber == policyNumber);

            try
            {
                // Kiểm tra xem khách hàng và hợp đồng có tồn tại không
                var customer = await _customerCollection.Find(filter & policyFilter).FirstOrDefaultAsync();

                if (customer == null)
                {
                    throw new Exception("Không tìm thấy khách hàng hoặc hợp đồng đã cho.");
                }

                // Tạo đối tượng cập nhật để thêm Payment vào danh sách Payments
                var updatePayment = Builders<CustomerModal>.Update.Push(c => c.Payments, newPayment);

                // Cập nhật ngày thanh toán tiếp theo cho hợp đồng
                var updateNextPaymentDate = Builders<CustomerModal>.Update.Set("policies.$.next_payment_date", newPayment.PaymentDate.AddYears(1));

                // Cập nhật thanh toán và ngày thanh toán tiếp theo
                await _customerCollection.UpdateOneAsync(filter & policyFilter, updatePayment);
                await _customerCollection.UpdateOneAsync(filter & policyFilter, updateNextPaymentDate);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception($"Lỗi khi thêm thanh toán: {ex.Message}", ex);
            }
        }

        //public async Task AddClaimPaymentAsync(string customerId, string claimNumber, PaymentModal payment)
        //{
        //    // Tạo bộ lọc để tìm khách hàng theo customerId và yêu cầu bồi thường theo claimNumber
        //    var filter = Builders<CustomerModal>.Filter.And(
        //        Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId),
        //        Builders<CustomerModal>.Filter.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == claimNumber && cl.Status == "Chờ xử lý")
        //    );

        //    // Tạo đối tượng cập nhật cho yêu cầu bồi thường (thanh toán và chuyển trạng thái)
        //    var updateClaim = Builders<CustomerModal>.Update.Set("claims.$.status", "Hoàn tất");

        //    // Tạo đối tượng cập nhật thêm thông tin thanh toán vào danh sách Payments
        //    var updatePayment = Builders<CustomerModal>.Update.Push(c => c.Payments, payment);

        //    try
        //    {
        //        // Thực hiện cập nhật trạng thái yêu cầu bồi thường
        //        var updateClaimResult = await _customerCollection.UpdateOneAsync(filter, updateClaim);

        //        if (updateClaimResult.ModifiedCount == 0)
        //        {
        //            throw new Exception("Không tìm thấy yêu cầu bồi thường hoặc yêu cầu đã được xử lý.");
        //        }

        //        // Thực hiện cập nhật thêm thông tin thanh toán vào danh sách Payments
        //        var updatePaymentResult = await _customerCollection.UpdateOneAsync(filter, updatePayment);

        //        if (updatePaymentResult.ModifiedCount == 0)
        //        {
        //            throw new Exception("Không thể thêm thông tin thanh toán.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi (có thể log lỗi hoặc thông báo cho người dùng)
        //        throw new Exception($"Lỗi khi thanh toán yêu cầu bồi thường: {ex.Message}", ex);
        //    }
        //}

        // Hàm thêm yêu cầu bồi thường kèm thông tin thanh toán
        public async Task AddClaimPaymentAsync(string customerId, string claimNumber, PaymentModal payment, string employeeId)
        {
            // Tạo bộ lọc để tìm khách hàng theo customerId và yêu cầu bồi thường theo claimNumber
            var filter = Builders<CustomerModal>.Filter.And(
                Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId),
                Builders<CustomerModal>.Filter.ElemMatch(c => c.Claims, cl => cl.ClaimNumber == claimNumber)
            );

            try
            {
                // Kiểm tra xem yêu cầu bồi thường có tồn tại hay không
                var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();
                if (customer == null)
                {
                    throw new Exception("Không tìm thấy yêu cầu bồi thường.");
                }

                // Kiểm tra trạng thái của yêu cầu bồi thường
                var claim = customer.Claims.FirstOrDefault(cl => cl.ClaimNumber == claimNumber);
                if (claim == null || claim.Status == "Hoàn tất")
                {
                    throw new Exception("Yêu cầu bồi thường đã được thanh toán.");
                }

                // Gán EmployeeId vào PaymentModal
                payment.EmployeeId = employeeId;

                // Tạo đối tượng cập nhật cho yêu cầu bồi thường (thanh toán và chuyển trạng thái)
                var update = Builders<CustomerModal>.Update
                    .Set("claims.$.status", "Hoàn tất")
                    .Push(c => c.Payments, payment);

                // Thực hiện cập nhật trạng thái yêu cầu bồi thường và thêm thông tin thanh toán vào danh sách Payments
                var updateResult = await _customerCollection.UpdateOneAsync(filter, update);

                if (updateResult.ModifiedCount == 0)
                {
                    throw new Exception("Không thể cập nhật yêu cầu bồi thường.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thanh toán yêu cầu bồi thường: {ex.Message}", ex);
            }
        }




    }
}
