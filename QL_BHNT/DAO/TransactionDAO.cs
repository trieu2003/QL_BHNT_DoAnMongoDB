using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QL_BHNT.Modal;
using QL_BHNT.UI;

namespace QL_BHNT.DAO
{
    
    public  class TransactionDAO
    {
        private readonly IMongoCollection<CustomerModal> _customerCollection;
        private readonly IMongoCollection<PaymentModal> _paymentCollection; // Thêm collection cho PaymentModal
        public TransactionDAO()
        {
            // Kết nối tới MongoDB và lấy collection Customer
            _customerCollection = MongoDBConnection.Instance.GetCollection<CustomerModal>("customer");
            _paymentCollection = MongoDBConnection.Instance.GetCollection<PaymentModal>("payments");
        }

        // Hàm lấy thông tin giao dịch dựa vào Mã giao dịch
        public async Task<TransactionDetails> DisplayTransactionDetailsAsync(string transactionId)
        {
            try
            {
                // Tạo bộ lọc để tìm kiếm giao dịch dựa trên PaymentId
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Payments, p => p.PaymentId == transactionId);
                var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();

                if (customer != null)
                {
                    // Lấy thông tin giao dịch cụ thể từ danh sách Payments
                    var payment = customer.Payments.FirstOrDefault(p => p.PaymentId == transactionId);

                    if (payment != null)
                    {
                        // Tạo đối tượng TransactionDetails và điền thông tin từ Payment và Customer
                        var transactionDetails = new TransactionDetails
                        {
                            TransactionId = payment.PaymentId,
                            Customer = new CustomerModal
                            {
                                CustomerId = customer.CustomerId,
                                FullName = customer.FullName,
                                DateOfBirth = customer.DateOfBirth,
                                Gender = customer.Gender,
                                ContactInformation = customer.ContactInformation
                            },
                            Payments = new List<PaymentModal>
                    {
                        new PaymentModal
                        {
                            PaymentId = payment.PaymentId,
                            PaymentDate = payment.PaymentDate,
                            Amount = payment.Amount,
                            Method = payment.Method,
                            Status = payment.Status,
                            PaymentType = payment.PaymentType,
                            RelatedTransaction = payment.RelatedTransaction
                        }
                    }
                        };

                        return transactionDetails; // Trả về thông tin giao dịch và khách hàng
                    }
                }

                return null; // Không tìm thấy giao dịch
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Error retrieving transaction details: {ex.Message}");
                throw; // Ném lại lỗi để xử lý ở nơi gọi
            }
        }
        //xoá giao dịch 
        public async Task<bool> CancelTransactionAsync(string customerId, string paymentId)
        {
            try
            {
                // Tìm kiếm khách hàng dựa vào customerId và paymentId
                var filter = Builders<CustomerModal>.Filter.And(
                    Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId),
                    Builders<CustomerModal>.Filter.ElemMatch(c => c.Payments, p => p.PaymentId == paymentId)
                );

                // Định nghĩa update để cập nhật trạng thái thành "Đã Huỷ"
                var update = Builders<CustomerModal>.Update.Set("payments.$.status", "Đã Huỷ");

                // Cập nhật thông tin khách hàng với giao dịch có trạng thái "Đã Huỷ"
                var result = await _customerCollection.UpdateOneAsync(filter, update);

                // Trả về kết quả của việc cập nhật
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có
                Console.WriteLine($"Lỗi khi huỷ giao dịch: {ex.Message}");
                return false;
            }
        }
        //cập nhật giao dịch
        public async Task<bool> UpdateTransactionAsync(string customerId, string paymentId, PaymentModal updatedPaymentInfo)
        {
            try
            {
                // Tạo bộ lọc để tìm khách hàng với mã giao dịch
                var customerFilter = Builders<CustomerModal>.Filter.And(
                    Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId),
                    Builders<CustomerModal>.Filter.ElemMatch(c => c.Payments, p => p.PaymentId == paymentId)
                );

                // Chỉ cập nhật những trường cần thiết
                var updateDefinition = Builders<CustomerModal>.Update.Set("payments.$.payment_date", updatedPaymentInfo.PaymentDate)
                                                                     .Set("payments.$.amount", updatedPaymentInfo.Amount)
                                                                     .Set("payments.$.method", updatedPaymentInfo.Method)
                                                                     .Set("payments.$.status", updatedPaymentInfo.Status)
                                                                     .Set("payments.$.payment_type", updatedPaymentInfo.PaymentType);

                // Thực hiện cập nhật
                var updateResult = await _customerCollection.UpdateOneAsync(customerFilter, updateDefinition);

                // Kiểm tra xem việc cập nhật có thành công không
                return updateResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Log lỗi và ném lại lỗi để xử lý ở UI nếu cần
                Console.WriteLine($"Error updating transaction: {ex.Message}");
                throw;
            }
        }
        //tìm kiếm theo mã khách hàng 
        public async Task<List<PaymentModal>> GetTransactionsByCustomerIdAsync(string customerId)
        {
            try
            {
                // Tạo bộ lọc để tìm khách hàng theo mã khách hàng
                var filter = Builders<CustomerModal>.Filter.Eq(c => c.CustomerId, customerId);

                // Tìm khách hàng từ collection
                var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();

                // Kiểm tra xem khách hàng có tồn tại không
                if (customer == null)
                {
                    Console.WriteLine($"Không tìm thấy khách hàng với mã: {customerId}");
                    return new List<PaymentModal>(); // Trả về danh sách trống nếu không tìm thấy khách hàng
                }

                // Lặp qua danh sách Payments và bổ sung thông tin từ CustomerModal vào mỗi PaymentModal
                if (customer.Payments != null)
                {
                    foreach (var payment in customer.Payments)
                    {
                        // Bổ sung thêm thông tin từ Customer vào mỗi PaymentModal
                        payment.EmployeeId = customer.CustomerId; // Sử dụng EmployeeId để chứa mã khách hàng (hoặc thêm thuộc tính khác nếu cần)
                        payment.RelatedTransaction = new RelatedTransaction
                        {
                            PolicyNumber = payment.RelatedTransaction?.PolicyNumber ?? "N/A"
                        };
                    }
                }

                return customer.Payments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách giao dịch cho mã khách hàng {customerId}: {ex.Message}");
                throw;
            }
        }



        //tìm kiếm theo loại giao dịch là hợp đồng hay yêu cầu bồi thường
        public async Task<List<PaymentModal>> GetContractPaymentsAsync(string paymentType)
        {
            var pipeline = new[]
            {
        new BsonDocument("$unwind", "$payments"),
        new BsonDocument("$match", new BsonDocument("payments.payment_type", paymentType)), // Lọc theo loại giao dịch truyền vào
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 0 },
            { "customer_id", "$customer_id" }, // Mã khách hàng
            { "related_transaction", "$payments.related_transaction" }, // Thông tin giao dịch liên quan
            { "payment_type", "$payments.payment_type" }, // Loại hợp đồng
            { "payment_id", "$payments.payment_id" }, // Mã giao dịch
            { "amount", "$payments.amount" }, // Tổng tiền
            { "status", "$payments.status" } // Trạng thái giao dịch
        })
    };

            // Thực hiện truy vấn và trả về danh sách PaymentModal
            var result = await _customerCollection.Aggregate<PaymentModal>(pipeline).ToListAsync();
            return result;
        }
        //tìm giao dịch theo mã số (hợp đồng / yêu cầu bồi thường)
        public async Task<List<PaymentModal>> GetPaymentsByPolicyNumberAsync(string policyNumber)
        {
            var pipeline = new[]
            {
                new BsonDocument("$unwind", "$payments"),
                new BsonDocument("$match", new BsonDocument("payments.related_transaction.policy_number", policyNumber)), // Lọc theo mã hợp đồng
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "customer_id", "$customer_id" }, // Mã khách hàng
                    { "related_transaction", "$payments.related_transaction" }, // Thông tin giao dịch liên quan
                    { "payment_type", "$payments.payment_type" }, // Loại hợp đồng
                    { "payment_id", "$payments.payment_id" }, // Mã giao dịch
                    { "amount", "$payments.amount" }, // Tổng tiền
                    { "status", "$payments.status" } // Trạng thái giao dịch
                })
    };

            // Thực hiện truy vấn và trả về danh sách PaymentModal
            var result = await _customerCollection.Aggregate<PaymentModal>(pipeline).ToListAsync();
            return result;
        }





    }
}
