using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Services
{
    public class ServiceToken<IType> : IServiceToken<IType, TokenEntity>
        where IType : struct
    {
        private readonly IDBRepository<PosStone> _dbRepository;
        private readonly ILogger _logger;

        public ServiceToken(IDBRepository<PosStone> dbRepository, ILogger logger)
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }
        public IDBRepository<PosStone> GetDBRepository() {
            return _dbRepository;
        }

        /*
        public async Task<IEnumerable<TokenEntity>> ListTokenAsync(TokenEntity payload)
        {
            try
            {
                IEnumerable<TokenEntity> lstToken = await _dbRepository.ListTokenAsync();

                _logger.LogInformation($"{Consts.PrefixLog}{typeof(ServiceToken).Name.ToLower()} Consulta no banco executada com sucesso: {payload}");

                return lstToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Consts.PrefixLog}{typeof(ServiceToken).Name.ToLower()} HandleAsync: Erro no banco de dados - Exception: {ex.Message}");

                return null;
            }
        }
        */
        

        public async Task<bool> SaveLoggedBearer(TokenEntity tokenEntity)
        {
            bool saved = await _dbRepository.Save(tokenEntity);
            return true;
        }

    }
}