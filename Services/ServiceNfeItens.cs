using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Services
{
    public class ServiceNfeItens<IType> : IServiceNfeItens<IType, NfeItensEntity>
        where IType : struct
    {
        private readonly IDBRepository<PosStone> _dbRepository;
        private readonly ILogger _logger;

        public ServiceNfeItens(IDBRepository<PosStone> dbRepository, ILogger logger)
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }
        public IDBRepository<PosStone> GetDBRepository() {
            return _dbRepository;
        }

        /*
        public async Task<IEnumerable<NfeItensEntity>> ListNfeItensAsync(NfeItensEntity payload)
        {
            try
            {
                IEnumerable<NfeItensEntity> lstNfeItens = await _dbRepository.ListNfeItensAsync();

                _logger.LogInformation($"{Consts.PrefixLog}{typeof(ServiceNfeItens).Name.ToLower()} Consulta no banco executada com sucesso: {payload}");

                return lstNfeItens;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Consts.PrefixLog}{typeof(ServiceNfeItens).Name.ToLower()} HandleAsync: Erro no banco de dados - Exception: {ex.Message}");

                return null;
            }
        }
        */

    }
}