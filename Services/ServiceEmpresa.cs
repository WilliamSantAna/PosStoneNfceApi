using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Services
{
    public class ServiceEmpresa<IType> : IServiceEmpresa<IType, EmpresaEntity>
        where IType : struct
    {
        private readonly IDBRepository<PosStone> _dbRepository;
        private readonly ILogger _logger;

        public ServiceEmpresa(IDBRepository<PosStone> dbRepository, ILogger logger)
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }
        public IDBRepository<PosStone> GetDBRepository() {
            return _dbRepository;
        }

        /*
        public async Task<IEnumerable<EmpresaEntity>> ListEmpresaAsync(EmpresaEntity payload)
        {
            try
            {
                IEnumerable<EmpresaEntity> lstEmpresa = await _dbRepository.ListEmpresaAsync();

                _logger.LogInformation($"{Consts.PrefixLog}{typeof(ServiceEmpresa).Name.ToLower()} Consulta no banco executada com sucesso: {payload}");

                return lstEmpresa;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Consts.PrefixLog}{typeof(ServiceEmpresa).Name.ToLower()} HandleAsync: Erro no banco de dados - Exception: {ex.Message}");

                return null;
            }
        }
        */

    }
}