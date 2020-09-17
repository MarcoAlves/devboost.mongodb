using devboost.dronedelivery.core.domain;
using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Interfaces;
using devboost.dronedelivery.domain.Entities;
using devboost.dronedelivery.domain.Enums;
using devboost.dronedelivery.domain.Interfaces;
using devboost.dronedelivery.domain.Interfaces.Repositories;
using devboost.dronedelivery.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devboost.dronedelivery.felipe.EF.Repositories
{

    public class DroneRepository : RepositoryBase<Drone>, IDroneRepository
    {
        private readonly ICommandExecutor<DroneStatusResult> _droneStatusExecutor;
        private readonly ICommandExecutor<StatusDroneDto> _statusDroneExecutor;
        private readonly IPedidoMongoRepository _pedidoMongoRepository;


        public DroneRepository(DataContext context, ICommandExecutor<StatusDroneDto> statusDroneExecutor,
            ICommandExecutor<DroneStatusResult> droneStatusExecutor,
            IPedidoMongoRepository pedidoMongoRepository) : base(context)
        {
            _droneStatusExecutor = droneStatusExecutor;
            _statusDroneExecutor = statusDroneExecutor;
            _pedidoMongoRepository = pedidoMongoRepository;
        }


        public Drone RetornaDrone()
        {
            return Context.Drone.FirstOrDefault();
        }

        public List<StatusDroneDto> GetDroneStatus()
        {
            var droneStatus = _statusDroneExecutor.ExcecuteCommand(GetStatusSqlCommand()).ToList();

            foreach (var status in droneStatus)
            {

                var pedido = _pedidoMongoRepository.FindByIdAsync(status.PedidoId).Result;

                status.ClienteId = pedido.ClienteId;
                status.Nome = pedido.Cliente.Nome;
                status.Latitude = pedido.Cliente.Latitude;
                status.Longitude = pedido.Cliente.Longitude;

            }

            var pedidosPendentes = _pedidoMongoRepository.FindAll(_ => _.Situacao == 1).Result;

            droneStatus.AddRange(pedidosPendentes.Select(_ => new StatusDroneDto() 
            { 
                ClienteId = _.ClienteId,
                DroneId = 0,
                Latitude = _.Cliente.Latitude,
                Longitude = _.Cliente.Longitude,
                Nome = _.Cliente.Nome,
                PedidoId = _.Id.ToString(),
                Situacao = false
            }));


            return droneStatus;
        }


        public DroneStatusDto RetornaDroneStatus(int droneId)
        {
            var consulta = _droneStatusExecutor.ExcecuteCommand(GetSqlCommand(droneId)).ToList();
            if (consulta.Any())
            {
                var droneData = consulta.FirstOrDefault();
                return new DroneStatusDto
                {
                    Drone = new Drone()
                    {
                        Id = droneData.Id,
                        Velocidade = droneData.Velocidade,
                        Capacidade = droneData.Capacidade,
                        Autonomia = droneData.Autonomia,
                        Carga = droneData.Carga,
                        Perfomance = droneData.Perfomance,
                    },
                    SomaPeso = droneData.SomaPeso,
                    SomaDistancia = droneData.SomaDistancia,

                };
            }
            return new DroneStatusDto();
        }

        private string GetSelectPedidos(int situacao, StatusEnvio status)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("select a.DroneId,");
            stringBuilder.AppendLine($"{situacao} as Situacao,");
            stringBuilder.AppendLine(" a.PedidoId as PedidoId");
            stringBuilder.AppendLine(" from PedidoDrones a");
            stringBuilder.AppendLine($" where a.StatusEnvio <> {(int)status}");

            return stringBuilder.ToString();
        }

        private string GetStatusSqlCommand()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(GetSelectPedidos(0, StatusEnvio.AGUARDANDO));
            stringBuilder.AppendLine(" union");
            stringBuilder.Append(GetSelectPedidos(1, StatusEnvio.EM_TRANSITO));


            return stringBuilder.ToString();
        }

        private static string GetSqlCommand(int droneId)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT D.*,");
            stringBuilder.AppendLine("SUM(PD.Distancia) AS SomaDistancia ");
            stringBuilder.AppendLine("FROM dbo.PedidoDrones PD ");
            stringBuilder.AppendLine("JOIN dbo.Drone D");
            stringBuilder.AppendLine("on PD.DroneId = D.Id");
            stringBuilder.AppendLine($"WHERE PD.DroneId = {droneId}");
            stringBuilder.AppendLine("GROUP BY D.Id, D.Autonomia, D.Capacidade, D.Carga, D.Perfomance, D.Velocidade");

            return stringBuilder.ToString();
        }
    }
}
