using devboost.dronedelivery.core.domain;
using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Enums;
using devboost.dronedelivery.domain.Interfaces.Repositories;
using devboost.dronedelivery.felipe.EF.Repositories;
using devboost.dronedelivery.felipe.Facade;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace devboost.dronedelivery.test
{
    public class ProcessaPagamentoTest
    {
        private readonly PedidoRepository _pedidoRepository;

        public ProcessaPagamentoTest()
        {
            _pedidoRepository = Substitute.For<PedidoRepository>();
        }

        [Fact]
        public async Task ProcessaPagamentoTeste()
        {

            var pagamentoFacade = new PagamentoFacade(_pedidoRepository);

            var listaPagamentos = new List<PagamentoStatusDto>();
            listaPagamentos.Add(new PagamentoStatusDto()
            {
                Descricao = "Pagamento Teste",
                IdPagamento = 1,
                Status = EStatusPagamento.APROVADO
            });

            _pedidoRepository.PegaPedidoPendenteAsync(Arg.Any<string>()).Returns(SetupTests.GetPedido());

            await pagamentoFacade.ProcessaPagamentosAsync(listaPagamentos);

            _pedidoRepository.Received().ReplaceOne(Arg.Any<Pedido>());

        }



    }
}
