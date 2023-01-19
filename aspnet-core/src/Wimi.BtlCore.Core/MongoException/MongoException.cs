using Abp.Domain.Entities;
using MongoDB.Bson;
using Wimi.BtlCore.BTLMongoDB;
using Wimi.BtlCore.Parameters.Dto;

namespace Wimi.BtlCore.MongoException
{
   [MongoTable("Exception",true)]
   public class MongoException : Entity<ObjectId>
    {
        public string MachineName { get; set; }

        public string ProcessName { get; set; }

        public string MachineCode { get; set; }

        public string Message { get; set; }

        public ErrorLevel ErrorLevel { get; set; }

        public string CreationTime { get; set; }
    }
}
