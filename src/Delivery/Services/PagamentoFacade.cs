using devboost.dronedelivery.core.domain;
using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Extensions;
using devboost.dronedelivery.core.domain.Interfaces;
using devboost.dronedelivery.domain.Enums;
using devboost.dronedelivery.domain.Interfaces;
using devboost.dronedelivery.domain.Interfaces.Repositories;
using devboost.dronedelivery.Infra.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devboost.dronedelivery.felipe.Facade
{
    public class PagamentoFacade : IPagamentoFacade
    {
        private readonly IPedidoMongoRepository _pedidoRepository;

        public PagamentoFacade(IPedidoMongoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task ProcessaPagamentosAsync(List<PagamentoStatusDto> pagamentoStatus)
        {
            foreach (var item in pagamentoStatus)
            {
                var pedido = await _pedidoRepository.FindOneAsync(p => p.GatewayPagamentoId == item.IdPagamento.ToString());

                if (item.Status.IsSuccess())
                {
                    pedido.Situacao = (int)StatusPedido.AGUARDANDO;
                    pedido.Pagamento.StatusPagamento = item.Status;

                    _pedidoRepository.ReplaceOne(pedido);
                }
            }
        }

    }
}
