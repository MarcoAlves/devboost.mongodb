using devboost.dronedelivery.core.domain;
using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Interfaces;
using devboost.dronedelivery.domain.Enums;
using devboost.dronedelivery.domain.Extensions;
using devboost.dronedelivery.domain.Interfaces;
using devboost.dronedelivery.domain.Interfaces.Repositories;
using devboost.dronedelivery.Infra.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Services
{
    public class DroneService : IDroneService
    {
        private readonly ICoordinateService _coordinateService;
        private readonly IPedidoDroneRepository _pedidoDroneRepository;
        private readonly IDroneRepository _droneRepository;
        private readonly IPedidoMongoRepository _pedidoRepository;

        public DroneService(ICoordinateService coordinateService,
            IPedidoDroneRepository pedidoDroneRepository,
            IDroneRepository droneRepository,
            IPedidoMongoRepository pedidoRepository)
        {
            _coordinateService = coordinateService;
            _pedidoDroneRepository = pedidoDroneRepository;
            _droneRepository = droneRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<DroneStatusDto> GetAvailiableDroneAsync(double distance, Pedido pedido)
        {
            var drones = (await _pedidoDroneRepository.RetornaPedidosEmAbertoAsync())
                .Select(d => new
                {
                    distance = _coordinateService.GetKmDistance(pedido.GetPoint(), pedido.GetPoint()),
                    droneId = d.DroneId
                }).OrderBy(p => p.distance);


            if (drones.FirstOrDefault() != null)
            {

                foreach (var drone in drones)
                {
                    var pedidosDrone = await _pedidoDroneRepository.RetornarPedidosPorDrone(drone.droneId);
                    var pesoAux = 0;

                    foreach (var pedDrone in pedidosDrone)
                    {
                        var pedidoTemp = await _pedidoRepository.FindByIdAsync(pedDrone.PedidoId);
                        pesoAux += pedidoTemp.Peso;
                    }

                    var resultado = _droneRepository.RetornaDroneStatus(drone.droneId);
                    resultado.SomaPeso = pesoAux;

                    if (ConsegueCarregar(resultado, drone.distance, distance, pedido))
                    {
                        return resultado;
                    }
                    else
                    {
                        var distanciaPedido = resultado.SomaDistancia + distance + drone.distance;

                        await _pedidoDroneRepository.UpdatePedidoDroneAsync(resultado, distanciaPedido);
                    }
                }
                return null;
            }
            else
            {
                await FinalizaPedidosAsync();
                var drone = _droneRepository.RetornaDrone();
                return new DroneStatusDto(drone);
            }
        }

        public List<StatusDroneDto> GetDroneStatus()
        {
            return _droneRepository.GetDroneStatus().ToList();
        }

        public async Task FinalizaPedidosAsync()
        {
            var pedidos = await _pedidoDroneRepository.RetornaPedidosParaFecharAsync();

            if (pedidos.Count > 0)
            {
                foreach (var pedido in pedidos)
                {
                    pedido.StatusEnvio = (int)StatusEnvio.FINALIZADO;
                    await _pedidoDroneRepository.UpdateAsync(pedido);
                }
            }
        }

        public bool ConsegueCarregar(DroneStatusDto droneStatus,
            double PedidoDroneDistance,
            double DistanciaRetorno,
            Pedido pedido)
        {
            return droneStatus != null
                    && (ValidaDistancia(droneStatus, PedidoDroneDistance, DistanciaRetorno))
                    && ValidaPeso(droneStatus, pedido);
        }

        public static bool ValidaPeso(DroneStatusDto droneStatus, Pedido pedido)
        {
            return droneStatus.SomaPeso + pedido.Peso <= droneStatus.Drone.Capacidade;
        }

        public static bool ValidaDistancia(DroneStatusDto droneStatus, double PedidoDroneDistance, double DistanciaRetorno)
        {
            return droneStatus.SomaDistancia + DistanciaRetorno + PedidoDroneDistance <= droneStatus.Drone.Perfomance;
        }

    }
}
