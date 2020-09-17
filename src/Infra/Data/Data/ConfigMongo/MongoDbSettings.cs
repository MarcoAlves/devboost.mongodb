using devboost.dronedelivery.core.domain.Interfaces;

namespace devboost.dronedelivery.Infra.Data.Data.ConfigMongo
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
