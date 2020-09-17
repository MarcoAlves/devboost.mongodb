using devboost.dronedelivery.core.domain.Interfaces;
using MongoDB.Bson;
using System;

namespace devboost.dronedelivery.core.domain.Entities
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

    }
}
