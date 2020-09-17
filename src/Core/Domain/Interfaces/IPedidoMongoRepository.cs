using devboost.dronedelivery.core.domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace devboost.dronedelivery.core.domain.Interfaces
{
    public interface IPedidoMongoRepository : IMongoRepository<Pedido>
    {

        Task<List<Pedido>> ObterPedidos(int situacao);


        Task<IEnumerable<Pedido>> ObterTodosPedidos();


        Task<Pedido> PegaPedidoPendenteAsync(string GatewayId);



    }
}
