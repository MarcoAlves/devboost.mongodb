using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace devboost.dronedelivery.core.domain.Interfaces
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {

        

        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);

        Task<TDocument> FindByIdAsync(string id);

        Task<IEnumerable<TDocument>> FindAll(Expression<Func<TDocument, bool>> filterExpression);

        Task InsertOneAsync(TDocument document);

        void ReplaceOne(TDocument document);

        

    }
}
