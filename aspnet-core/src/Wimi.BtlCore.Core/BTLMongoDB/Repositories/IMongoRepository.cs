using System.Collections.Generic;
using MongoDB.Driver;

namespace Wimi.BtlCore.BTLMongoDB.Repositories
{
    public interface IMongoRepository<TEntity> where TEntity : class
    {
        long Count();

        long Count(FilterDefinition<TEntity> filter);

        void DeleteMany(FilterDefinition<TEntity> filter);

        void DeleteOne(FilterDefinition<TEntity> filter);

        TEntity Get(FilterDefinition<TEntity> filter);

        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> GetMany(FilterDefinition<TEntity> filter);

        void InsertMany(IEnumerable<TEntity> entities);

        void InsertOne(TEntity entity);

        void UpdateMany(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);

        void UpdateOne(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);
    }
}