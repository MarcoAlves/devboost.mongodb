using devboost.dronedelivery.core.domain.Entities;
using MongoDB.Driver;

namespace devboost.dronedelivery.Infra.Data.Data
{
    public class MongoDbContext
    {

        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _pedidoDatabase;

        public MongoDbContext()
        {
            _mongoClient = new MongoClient();
            _pedidoDatabase = _mongoClient.GetDatabase("Pedido", new MongoDatabaseSettings());
        }

        public IMongoCollection<Pedido> Pedidos { get => _pedidoDatabase.GetCollection<Pedido>("Pedidos"); }

    }

}
