using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace QL_BHNT.DAO
{
    public class MongoDBConnection
    {
        private static MongoDBConnection _instance;
        private readonly IMongoDatabase _database;
        //kết nối cở sở dữ liệu 
        private MongoDBConnection(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public static MongoDBConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MongoDBConnection("mongodb://localhost:27017", "QL_BHNT");
                }
                return _instance;
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
