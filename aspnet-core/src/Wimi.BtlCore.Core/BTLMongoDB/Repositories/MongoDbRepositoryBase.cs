using Abp.Localization;
using Abp.UI;
using Castle.Core.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.BTLMongoDB.Repositories
{
    public class MongoDbRepositoryBase<TEntity> : IMongoRepository<TEntity> where TEntity : class
    {
        private readonly int Limit = 10000;
        
        public virtual IMongoDatabase Database => databaseProvider.Database;

        public virtual IMongoCollection<TEntity> Collection =>
            databaseProvider.Database.GetCollection<TEntity>(typeof(TEntity).GetAttribute<MongoTableAttribute>().GetTableName());

        public virtual IMongoCollection<BsonDocument> BsonDocumentCollection =>
             databaseProvider.Database.GetCollection<BsonDocument>(typeof(TEntity).GetAttribute<MongoTableAttribute>().GetTableName());

        public virtual IMongoCollection<BsonDocument> GetCollectionByName(string collectionName) =>
            databaseProvider.Database.GetCollection<BsonDocument>(collectionName);

        private readonly IMongoDatabaseProvider databaseProvider;
        private readonly ILocalizationManager localizationManager;

        public MongoDbRepositoryBase(IMongoDatabaseProvider databaseProvider, ILocalizationManager localizationManager)
        {
            this.databaseProvider = databaseProvider;
            this.localizationManager = localizationManager;
        }

        public long Count()
        {
            return Collection.CountDocuments(Builders<TEntity>.Filter.Empty);
        }

        public long Count(FilterDefinition<TEntity> filter)
        {
            return Collection.CountDocuments(filter);
        }

        public void DeleteMany(FilterDefinition<TEntity> filter)
        {
            Collection.DeleteMany(filter);
        }

        public void DeleteOne(FilterDefinition<TEntity> filter)
        {
            Collection.DeleteOne(filter);
        }

        public TEntity Get(FilterDefinition<TEntity> filter)
        {
            return Collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Collection.Find(Builders<TEntity>.Filter.Empty).ToList();
        }

        public IEnumerable<TEntity> GetMany(FilterDefinition<TEntity> filter)
        {
            return Collection.Find(filter).Limit(Limit).ToEnumerable();
        }

        public void InsertMany(IEnumerable<TEntity> entities)
        {
            Collection.InsertMany(entities);
        }

        public void InsertOne(TEntity entity)
        {
            try
            {
                Collection.InsertOne(entity);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"{localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "DataInsertFailed")}:{ex}");
            }
        }

        public void UpdateMany(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            Collection.UpdateMany(filter, update);
        }

        public void UpdateOne(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            Collection.UpdateOne(filter, update);
        }

        public IEnumerable<TEntity> GetManyByPage(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort, int skipNum, int limitNum)
        {
            return Collection.Find(filter).Sort(sort).Skip(skipNum).Limit(limitNum).ToList();
        }


        public IEnumerable<TEntity> GetManyByLimit(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort, int limitNum)
        {
            return Collection.Find(filter).Sort(sort).Limit(limitNum).ToList();
        }

    }
}