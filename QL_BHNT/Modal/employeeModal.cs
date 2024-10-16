using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QL_BHNT.Modal
{
    public class employeeModal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("employee_id")]
        public string EmployeeId { get; set; }

        [BsonElement("full_name")]
        public string FullName { get; set; }

        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("contact_info")]
        public EmployeeContactInfo ContactInfo { get; set; }

        [BsonElement("position")]
        public string Position { get; set; }

        [BsonElement("department")]
        public string Department { get; set; }

        [BsonElement("hire_date")]
        public DateTime HireDate { get; set; }

        [BsonElement("salary")]
        public decimal Salary { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public class EmployeeContactInfo
        {
            [BsonElement("phone")]
            public string Phone { get; set; }

            [BsonElement("email")]
            public string Email { get; set; }

            [BsonElement("address")]
            public string Address { get; set; }
        }
    }
}
