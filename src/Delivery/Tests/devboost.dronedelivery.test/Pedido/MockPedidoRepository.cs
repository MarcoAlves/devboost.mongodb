using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Interfaces;
using devboost.dronedelivery.domain.Enums;
using devboost.dronedelivery.domain.Interfaces.Repositories;
using devboost.dronedelivery.felipe.EF.Repositories;
using devboost.dronedelivery.Infra.Data;
using devboost.dronedelivery.Infra.Data.Data.ConfigMongo;
using devboost.dronedelivery.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devboost.dronedelivery.test
{
    public class MockPedidoRepository : RepositoryBaseMongoDb<Pedido>, IMongoRepository<Pedido>
    {
        private readonly List<core.domain.Entities.Pedido> _pedidos;

        public MockPedidoRepository(MongoDbSettings settings) : base(settings)
        {

        }

        public List<Pedido> ObterPedidos(int situacao)
        {
            const string clientePassword = "12543GTFrd@65";

            var cliente = new Cliente
            {
                Id = 1,
                Latitude = -23.5880684,
                Longitude = -46.6564195,
                Nome = "João Silva Antunes",
                UserId = "joaoantunes",
                Password = clientePassword
            };

            var pedido = new Pedido
            {
                Peso = 100,
                Situacao = (int)StatusPedido.AGUARDANDO,
                DataHoraInclusao = DateTime.Now,
                DataHoraFinalizacao = DateTime.Now,
                Cliente = cliente
            };
            _pedidos.Add(pedido);
            return _pedidos;
        }

        public Task<core.domain.Entities.Pedido> PegaPedidoPendenteAsync(string GatewayId)
        {
            throw new System.NotImplementedException();
        }

        public void SetState(core.domain.Entities.Pedido pedido, EntityState entityState)
        {
            throw new System.NotImplementedException();
        }
    }
}
