using MongoDB.Bson;
using MongoDB.Driver;
using QL_BHNT.Modal;
using QL_BHNT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_BHNT.DAO
{
    public class ReportDAO
    {
        private readonly IMongoCollection<Contract> _reportCollection;
        private readonly IMongoCollection<CustomerModal> _customersCollection;
        private readonly IMongoCollection<PaymentModal> _paymentsCollection;
        private readonly IMongoCollection<Contract> _contractsCollection;

        private readonly IMongoCollection<ContractReport> _reportListCollection;
        public ReportDAO() 
        {
            // Kết nối tới MongoDB và lấy collection Customer
            _reportCollection = MongoDBConnection.Instance.GetCollection<Contract>("customer");
            _customersCollection = MongoDBConnection.Instance.GetCollection<CustomerModal>("customer");
            _reportListCollection =MongoDBConnection.Instance.GetCollection<ContractReport>("customer");
            _paymentsCollection = MongoDBConnection.Instance.GetCollection<PaymentModal>("customer");
            _contractsCollection = MongoDBConnection.Instance.GetCollection<Contract>("customer");
        }
        public async Task<(int totalContracts, int totalClaims, List<DateTime> contractDates, List<DateTime> claimDates)> GetMonthlyStatisticsAsync(int month, int year)
        {
            // Chuyển tháng từ 1-12 thành 0-11 cho MongoDB
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument("$or", new BsonArray
        {
            new BsonDocument("policies.start_date", new BsonDocument("$gte", startDate)),
            new BsonDocument("claims.claim_date", new BsonDocument("$gte", startDate))
        })),
        new BsonDocument("$project", new BsonDocument
        {
            { "totalContracts", new BsonDocument
                {
                    { "$size", new BsonDocument
                        {
                            { "$filter", new BsonDocument
                                {
                                    { "input", "$policies" },
                                    { "as", "policy" },
                                    { "cond", new BsonDocument
                                        {
                                            { "$and", new BsonArray
                                                {
                                                    new BsonDocument("$gte", new BsonArray { "$$policy.start_date", startDate }),
                                                    new BsonDocument("$lt", new BsonArray { "$$policy.start_date", endDate })
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            { "totalClaims", new BsonDocument
                {
                    { "$size", new BsonDocument
                        {
                            { "$filter", new BsonDocument
                                {
                                    { "input", "$claims" },
                                    { "as", "claim" },
                                    { "cond", new BsonDocument
                                        {
                                            { "$and", new BsonArray
                                                {
                                                    new BsonDocument("$gte", new BsonArray { "$$claim.claim_date", startDate }),
                                                    new BsonDocument("$lt", new BsonArray { "$$claim.claim_date", endDate })
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            { "contractDates", "$policies.start_date" }, // Lấy danh sách ngày bắt đầu hợp đồng
            { "claimDates", "$claims.claim_date" } // Lấy danh sách ngày yêu cầu bồi thường
        }),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", BsonNull.Value },
            { "totalContracts", new BsonDocument("$sum", "$totalContracts") },
            { "totalClaims", new BsonDocument("$sum", "$totalClaims") },
            { "contractDates", new BsonDocument("$push", "$contractDates") }, // Gom nhóm các ngày hợp đồng
            { "claimDates", new BsonDocument("$push", "$claimDates") } // Gom nhóm các ngày yêu cầu bồi thường
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 0 },
            { "totalContracts", 1 },
            { "totalClaims", 1 },
            { "contractDates", new BsonDocument("$reduce", new BsonDocument
                {
                    { "input", "$contractDates" },
                    { "initialValue", new BsonArray() },
                    { "in", new BsonDocument("$concatArrays", new BsonArray { "$$value", "$$this" }) }
                })
            },
            { "claimDates", new BsonDocument("$reduce", new BsonDocument
                {
                    { "input", "$claimDates" },
                    { "initialValue", new BsonArray() },
                    { "in", new BsonDocument("$concatArrays", new BsonArray { "$$value", "$$this" }) }
                })
            }
        })
    };

            var result = await _customersCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            // Trả về các giá trị thống kê
            if (result != null)
            {
                int totalContracts = result.GetValue("totalContracts").AsInt32;
                int totalClaims = result.GetValue("totalClaims").AsInt32;

                // Lấy danh sách ngày hợp đồng và ngày yêu cầu bồi thường
                var contractDates = result.GetValue("contractDates").AsBsonArray
                    .Select(date => date.AsDateTime)
                    .ToList();

                var claimDates = result.GetValue("claimDates").AsBsonArray
                    .Select(date => date.AsDateTime)
                    .ToList();

                return (totalContracts, totalClaims, contractDates, claimDates);
            }

            // Trả về mặc định nếu không tìm thấy dữ liệu
            return (0, 0, new List<DateTime>(), new List<DateTime>());
        }
        //public List<ContractReport> GetContractReports()
        //{
        //    // Lấy tất cả tài liệu trong collection
        //    var results = _customersCollection.Find(new BsonDocument()).ToList();
        //    var contractReports = new List<ContractReport>();

        //    foreach (var result in results)
        //    {
        //        var customerId = result["customer_id"].AsString;

        //        // Xử lý thông tin hợp đồng
        //        if (result.Contains("policies") && result["policies"].IsBsonArray)
        //        {
        //            foreach (var policy in result["policies"].AsBsonArray)
        //            {
        //                var policyDoc = policy.AsBsonDocument;
        //                var policyNumber = policyDoc["policy_number"].AsString;
        //                var startDate = policyDoc["start_date"].ToUniversalTime();

        //                // Thêm hợp đồng vào danh sách
        //                contractReports.Add(new ContractReport
        //                {
        //                    CustomerId = customerId,
        //                    ContractId = policyNumber,
        //                    CreatedDate = startDate
        //                });
        //            }
        //        }

        //        // Xử lý thông tin yêu cầu bồi thường
        //        if (result.Contains("claims") && result["claims"].IsBsonArray)
        //        {
        //            foreach (var claim in result["claims"].AsBsonArray)
        //            {
        //                var claimDoc = claim.AsBsonDocument;
        //                var claimNumber = claimDoc["claim_number"].AsString;
        //                var claimDate = claimDoc["claim_date"].ToUniversalTime();

        //                // Thêm yêu cầu bồi thường vào danh sách
        //                contractReports.Add(new ContractReport
        //                {
        //                    CustomerId = customerId,
        //                    ContractId = claimNumber,
        //                    CreatedDate = claimDate
        //                });
        //            }
        //        }
        //    }

        //    return contractReports;
        //}
        public  List<dynamic> GetContractInfoByMonthYear(int month, int year)
        {
            var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Policies,
                         policy => policy.StartDate.Month == month && policy.StartDate.Year == year);
            var customers = _customersCollection.Find(filter).ToList();

            var contractInfos = customers.Select(customer => new
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Policies = customer.Policies.Where(p => p.StartDate.Month == month && p.StartDate.Year == year)
                                            .Select(policy => new
                                            {
                                                PolicyNumber = policy.PolicyNumber,
                                                StartDate = policy.StartDate
                                            })
            }).ToList<dynamic>();

            return contractInfos;
        }

        public async Task<List<ContractAndClaimInfo>> GetMonthlyAsync(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument("$or", new BsonArray
        {
            new BsonDocument("policies.start_date", new BsonDocument("$gte", startDate)),
            new BsonDocument("claims.claim_date", new BsonDocument("$gte", startDate))
        })),
        new BsonDocument("$unwind", "$policies"),
        new BsonDocument("$unwind", "$claims"),
        new BsonDocument("$match", new BsonDocument("$or", new BsonArray
        {
            new BsonDocument("policies.start_date", new BsonDocument("$lt", endDate)),
            new BsonDocument("claims.claim_date", new BsonDocument("$lt", endDate))
        })),
        new BsonDocument("$project", new BsonDocument
        {
            { "CustomerId", "$customer_id" },
            { "PolicyId", "$policies.policy_id" },
            { "PolicyStartDate", "$policies.start_date" },
            { "ClaimId", "$claims.claim_id" },
            { "ClaimDate", "$claims.claim_date" }
        })
    };

            var result = await _customersCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // Chuyển kết quả sang dạng List<ContractAndClaimInfo> để tiện xử lý và hiển thị
            var contractAndClaimInfoList = result.Select(doc => new ContractAndClaimInfo
            {
                CustomerId = doc.GetValue("CustomerId", "").AsString,
                PolicyId = doc.Contains("PolicyId") ? doc.GetValue("PolicyId").AsString : null,
                PolicyStartDate = doc.Contains("PolicyStartDate") ? (DateTime?)doc.GetValue("PolicyStartDate").ToUniversalTime() : null,
                ClaimId = doc.Contains("ClaimId") ? doc.GetValue("ClaimId").AsString : null,
                ClaimDate = doc.Contains("ClaimDate") ? (DateTime?)doc.GetValue("ClaimDate").ToUniversalTime() : null
            }).ToList();

            return contractAndClaimInfoList;
        }
        public async Task<(List<ContractAndClaimInfo> contracts, List<ContractAndClaimInfo> claims)> GetMonthlyContractsAndClaimsAsync(int month, int year)
        {
            var allInfo = await GetMonthlyAsync(month, year);

            // Tách biệt thông tin hợp đồng và yêu cầu bồi thường
            var contracts = allInfo.Where(info => info.PolicyId != null).ToList();
            var claims = allInfo.Where(info => info.ClaimId != null).ToList();

            return (contracts, claims);
        }

        public async Task<(int totalContractPayments, int totalClaimPayments)> GetMonthlyPaymentStatisticsAsync(int month, int year)
        {
            // Chuyển tháng từ 1-12 thành 0-11 cho MongoDB
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument
        {
            { "payments.payment_date", new BsonDocument("$gte", startDate) },
            { "payments.payment_date", new BsonDocument("$lt", endDate) }
        }),
        new BsonDocument("$unwind", "$payments"),
        new BsonDocument("$match", new BsonDocument
        {
            { "payments.payment_date", new BsonDocument("$gte", startDate) },
            { "payments.payment_date", new BsonDocument("$lt", endDate) }
        }),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", null },
            { "totalContractPayments", new BsonDocument("$sum", new BsonDocument
                {
                    { "$cond", new BsonArray
                        {
                            new BsonDocument("$eq", new BsonArray { "$payments.payment_type", "Hợp Đồng" }),
                            1,
                            0
                        }
                    }
                })
            },
            { "totalClaimPayments", new BsonDocument("$sum", new BsonDocument
                {
                    { "$cond", new BsonArray
                        {
                            new BsonDocument("$eq", new BsonArray { "$payments.payment_type", "Yêu Cầu Bồi Thường" }),
                            1,
                            0
                        }
                    }
                })
            }
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 0 },
            { "totalContractPayments", 1 },
            { "totalClaimPayments", 1 }
        })
    };

            var result = await _customersCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            // Trả về các giá trị thống kê
            if (result != null)
            {
                int totalContractPayments = result.GetValue("totalContractPayments", 0).AsInt32;
                int totalClaimPayments = result.GetValue("totalClaimPayments", 0).AsInt32;

                return (totalContractPayments, totalClaimPayments);
            }

            // Trả về mặc định nếu không tìm thấy dữ liệu
            return (0, 0);
        }




        ////
        public async Task<int> GetTotalContractsAsync(int month, int year)
        {
            // Tính ngày bắt đầu và ngày kết thúc của tháng
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1); // Ngày đầu tiên của tháng tiếp theo

            // Tạo bộ lọc theo khoảng thời gian
            var filterBuilder = Builders<PaymentModal>.Filter;
            var filter = filterBuilder.Eq("payment_type", "Hợp Đồng") &
                         filterBuilder.Eq("status", "Hoàn tất") &
                         filterBuilder.Gte("payment_date", startDate) &
                         filterBuilder.Lt("payment_date", endDate); // Lưu ý dùng Lt (Less than) cho endDate

            return (int)await _paymentsCollection.CountDocumentsAsync(filter);
        }

        public async Task<int> GetTotalClaimsAsync(int month, int year)
        {
            // Tính ngày bắt đầu và ngày kết thúc của tháng
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1); // Ngày đầu tiên của tháng tiếp theo

            // Tạo bộ lọc theo khoảng thời gian
            var filterBuilder = Builders<PaymentModal>.Filter;
            var filter = filterBuilder.Eq("payment_type", "Yêu Cầu Bồi Thường") &
                         filterBuilder.Eq("status", "Hoàn tất") &
                         filterBuilder.Gte("payment_date", startDate) &
                         filterBuilder.Lt("payment_date", endDate); // Lưu ý dùng Lt (Less than) cho endDate

            return (int)await _paymentsCollection.CountDocumentsAsync(filter);
        }


        ////
        ///
        public async Task<int> GetTotalContractsByMonthAndYear(int month, int year)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Payments,
                    Builders<PaymentModal>.Filter.And(
                        Builders<PaymentModal>.Filter.Eq(p => p.PaymentType, "Hợp Đồng"), // Thay bằng loại hợp đồng bạn muốn
                        Builders<PaymentModal>.Filter.Eq(p => p.Status, "Hoàn tất"), // Thay bằng trạng thái bạn muốn
                        Builders<PaymentModal>.Filter.Gte(p => p.PaymentDate, new DateTime(year, month, 1)), // Ngày đầu tháng
                        Builders<PaymentModal>.Filter.Lt(p => p.PaymentDate, new DateTime(year, month, 1).AddMonths(1)) // Ngày đầu tháng sau
                    )
                );

                var result = await _customersCollection.Find(filter).ToListAsync();

                return result.Count; // Đếm tổng số kết quả phù hợp
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching total contracts: " + ex.Message);
                throw; // Ném lỗi để xử lý ở nơi gọi
            }
        }


        public async Task<int> GetTotalClaimsByMonthAndYear(int month, int year)
        {
            try
            {
                var filter = Builders<CustomerModal>.Filter.ElemMatch(c => c.Payments,
                    Builders<PaymentModal>.Filter.And(
                        Builders<PaymentModal>.Filter.Eq(p => p.PaymentType, "Yêu Cầu Bồi Thường"),
                        Builders<PaymentModal>.Filter.Eq(p => p.Status, "Hoàn tất"),
                        Builders<PaymentModal>.Filter.Gte(p => p.PaymentDate, new DateTime(year, month, 1)), // Ngày đầu tháng
                        Builders<PaymentModal>.Filter.Lt(p => p.PaymentDate, new DateTime(year, month, 1).AddMonths(1)) // Ngày đầu tháng sau
                    )
                );

                var result = await _customersCollection.Find(filter).ToListAsync();

                return result.Count; // Đếm tổng số kết quả phù hợp
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching total claims: " + ex.Message);
                throw; // Ném lỗi để xử lý ở nơi gọi
            }
        }



    }

}

