

using Abp.Domain.Entities;
using MongoDB.Bson;

namespace Wimi.BtlCore.MongoEntities
{
    public  interface IEntity : IEntity<ObjectId>
    {

    }
}
