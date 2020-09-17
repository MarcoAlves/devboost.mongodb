using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Interfaces;
using devboost.dronedelivery.domain.Interfaces.Repositories;
using devboost.dronedelivery.Infra.Data;
using devboost.dronedelivery.Infra.Data.Data.ConfigMongo;
using devboost.dronedelivery.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devboost.dronedelivery.felipe.EF.Repositories
{
    public class PedidoRepository : RepositoryBaseMongoDb<Pedido>, IPedidoMongoRepository
    {
        public PedidoRepository(MongoDbSettings settings) : base(settings)
        {
        }

        public async Task<List<Pedido>> ObterPedidos(int situacao)
        {

            var pedidos = await FindAll(p => p.Situacao == situacao);

            return pedidos.ToList();
        }

        public async Task<IEnumerable<Pedido>> ObterTodosPedidos()
        {
            return (await FindAll()).ToList();
        }

        public async Task<Pedido> PegaPedidoPendenteAsync(string GatewayId)
        {
            return (await FindAll(p => p.GatewayPagamentoId == GatewayId)).FirstOrDefault();
        }

    }
}
