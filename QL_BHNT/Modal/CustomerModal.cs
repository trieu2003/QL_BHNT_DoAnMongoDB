using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_BHNT.Modal
{
    public class CustomerModal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("customer_id")]
        public string CustomerId { get; set; }

        [BsonElement("full_name")]
        public string FullName { get; set; }

        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("contact_info")]
        public ContactInfo ContactInformation { get; set; }

        [BsonElement("policies")]
        public List<PolicyModal> Policies { get; set; }

        [BsonElement("claims")]
        public List<Claim> Claims { get; set; }

        [BsonElement("payments")]
        public List<PaymentModal> Payments { get; set; }

        [BsonElement("agent_id")]
        public string AgentId { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }

    public class ContactInfo
    {
        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }
    }

    public class PolicyModal
    {
        [BsonElement("policy_number")]
        public string PolicyNumber { get; set; }

        [BsonElement("policy_type")]
        public string PolicyType { get; set; }

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        public DateTime EndDate { get; set; }

        [BsonElement("premium_amount")]
        public decimal PremiumAmount { get; set; }

        [BsonElement("coverage_amount")]
        public decimal CoverageAmount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("beneficiary")]
        public Beneficiary Beneficiary { get; set; }

        [BsonElement("next_payment_date")]
        public DateTime? NextPaymentDate { get; set; }
    }

    public class Beneficiary
    {
        [BsonElement("full_name")]
        public string FullName { get; set; }

        [BsonElement("relationship")]
        public string Relationship { get; set; }

        [BsonElement("contact_info")]
        public ContactInfo ContactInformation { get; set; }
    }

    public class Claim
    {
        [BsonElement("claim_number")]
        public string ClaimNumber { get; set; }

        [BsonElement("policy_number")]
        public string PolicyNumber { get; set; }

        [BsonElement("claim_date")]
        public DateTime ClaimDate { get; set; }

        [BsonElement("claim_amount")]
        public decimal ClaimAmount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("documents")]
        public ClaimDocuments Documents { get; set; }
    }

    public class ClaimDocuments
    {
        [BsonElement("death_certificate")]
        public string DeathCertificate { get; set; }

        [BsonElement("insurance_policy")]
        public string InsurancePolicy { get; set; }

        [BsonElement("proof_of_relationship")]
        public string ProofOfRelationship { get; set; }
    }

    // Cập nhật PaymentModal với employee_id và related_transaction
    public class PaymentModal
    {
        [BsonElement("payment_id")]
        public string PaymentId { get; set; }

        [BsonElement("payment_date")]
        public DateTime PaymentDate { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("method")]
        public string Method { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("payment_type")]
        public string PaymentType { get; set; }

        [BsonElement("employee_id")]
        public string EmployeeId { get; set; }  // Thêm trường employee_id
        [BsonElement("customer_id")]
        public string CustomerID { get; set; } // Thêm thuộc tính CustomerID

        [BsonElement("related_transaction")]
        public RelatedTransaction RelatedTransaction { get; set; }  // Thêm liên kết đến giao dịch liên quan
    }

    // Class quản lý thông tin giao dịch liên quan
    public class RelatedTransaction
    {
        [BsonElement("policy_number")]
        public string PolicyNumber { get; set; }
    }
    public class TransactionDetails
    {
        public string TransactionId { get; set; }
        public CustomerModal Customer { get; set; }
        public List<PaymentModal> Payments { get; set; }
    }
    public class Contract
    {
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; } // Hợp đồng hay yêu cầu bồi thường
                                         // Các trường khác nếu cần
    }
    public class ContractReport
    {
        public string CustomerId { get; set; }
        public string ContractId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ClaimDisplay
    {
        public string ClaimNumber { get; set; }
        public DateTime ClaimDate { get; set; }
        public decimal ClaimAmount { get; set; }
        public string Status { get; set; }
        public string CustomerId { get; set; } // Mã khách hàng
    }

    public class ContractAndClaimInfo
    {
        public string CustomerId { get; set; }
        public string PolicyId { get; set; }
        public DateTime? PolicyStartDate { get; set; }
        public string ClaimId { get; set; }
        public DateTime? ClaimDate { get; set; }
    }

    public class CustomerInfoDisplay
    {
        public string CustomerId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal CoverageAmount { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public string Status { get; set; }
        public string BeneficiaryFullName { get; set; }
        public string BeneficiaryRelationship { get; set; }
        public string BeneficiaryPhone { get; set; }
        public string BeneficiaryEmail { get; set; }
    }
}

