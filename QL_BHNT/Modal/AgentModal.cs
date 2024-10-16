using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_BHNT.Modal
{
    public  class AgentModal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("agent_id")]
        public string AgentId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("contact_info")]
        public AgentContactInfo ContactInfo { get; set; }

        public class AgentContactInfo
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
