using devboost.dronedelivery.core.domain.Interfaces;
using devboost.dronedelivery.Infra.Data.Data;
using devboost.dronedelivery.Infra.Data.Data.ConfigMongo;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Infra.Data.Repositories
{
    public class RepositoryBaseMongoDb<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {

        private readonly IMongoCollection<TDocument> _collection;

        public RepositoryBaseMongoDb(MongoDbSettings settings)
        {

            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>("Pedido");

        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }


        public Task<TDocument> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }

        public Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<TDocument>> FindAll(Expression<Func<TDocument, bool>> filterExpression = null)
        {
            IEnumerable<TDocument> result;
            if (filterExpression != null)
                result = await _collection.Find(filterExpression).ToListAsync();
            else
                result = await _collection.Find(Builders<TDocument>.Filter.Empty).ToListAsync();

            return result;
        }


        public Task InsertOneAsync(TDocument document)
        {
            return Task.Run(() => _collection.InsertOneAsync(document));
        }

        public void ReplaceOne(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(filter, document);
        }

    }
}
