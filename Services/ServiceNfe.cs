using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Services
{
    public class ServiceNfe<IType> : IServiceNfe<IType, NfeEntity>
        where IType : struct
    {
        private readonly IDBRepository<PosStone> _dbRepository;
        private readonly ILogger _logger;

        public ServiceNfe(IDBRepository<PosStone> dbRepository, ILogger logger)
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }
        public IDBRepository<PosStone> GetDBRepository() {
            return _dbRepository;
        }

        /*
        public async Task<IEnumerable<NfeEntity>> ListNfeAsync(NfeEntity payload)
        {
            try
            {
                IEnumerable<NfeEntity> lstNfe = await _dbRepository.ListNfeAsync();

                _logger.LogInformation($"{Consts.PrefixLog}{typeof(ServiceNfe).Name.ToLower()} Consulta no banco executada com sucesso: {payload}");

                return lstNfe;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Consts.PrefixLog}{typeof(ServiceNfe).Name.ToLower()} HandleAsync: Erro no banco de dados - Exception: {ex.Message}");

                return null;
            }
        }
        */

    }
}