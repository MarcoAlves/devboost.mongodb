using devboost.dronedelivery.domain.Enums;
using devboost.dronedelivery.felipe.EF.Repositories;
using devboost.dronedelivery.Infra.Data;
using devboost.dronedelivery.Infra.Data.Data.ConfigMongo;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace devboost.dronedelivery.test.Repositories
{
    public class PedidoRepositoryTests
    {
        private PedidoRepository GetRepository()
        {
            var data = SetupTests.GetPedidosList();
            var context = ContextProvider<core.domain.Entities.Pedido>.GetContext(data);
            var settings = new MongoDbSettings();
            return new PedidoRepository(settings);

        }

        [Fact]
        public async Task GetPedidoTest()
        {

            var pedido = await GetRepository().FindByIdAsync("1");
            Assert.True(pedido != null);
        }
        [Fact]
        public void ObterPedidosTest()
        {
            var pedido = GetRepository().ObterPedidos((int)StatusPedido.AGUARDANDO);
            Assert.True(pedido != null);
        }
        [Fact]
        public async Task SavePedidosTest()
        {
            var pedidoTests = SetupTests.GetPedido();
            var repository = GetRepository();
            await repository.InsertOneAsync(pedidoTests);
            repository.ReplaceOne(pedidoTests);
            
            Assert.True(true);
        }

        [Fact]
        public void PegaPedidoPendenteTest()
        {
            var repository = GetRepository();


            var pedido = repository.PegaPedidoPendenteAsync("1");


            Assert.True(pedido != null);
        }


    }
}
