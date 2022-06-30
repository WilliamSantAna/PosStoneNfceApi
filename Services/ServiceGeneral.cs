using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Services
{
    public class ServiceGeneral<IType> : IServiceGeneral<IType, GeneralEntity>
        where IType : struct
    {
        private readonly IDBRepository<PosStone> _dbRepository;
        private readonly ILogger _logger;

        public ServiceGeneral(IDBRepository<PosStone> dbRepository, ILogger logger)
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }
        public IDBRepository<PosStone> GetDBRepository() {
            return _dbRepository;
        }

        /*
        public async Task<IEnumerable<GeneralEntity>> ListGeneralAsync(GeneralEntity payload)
        {
            try
            {
                IEnumerable<GeneralEntity> lstGeneral = await _dbRepository.ListGeneralAsync();

                _logger.LogInformation($"{Consts.PrefixLog}{typeof(ServiceGeneral).Name.ToLower()} Consulta no banco executada com sucesso: {payload}");

                return lstGeneral;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Consts.PrefixLog}{typeof(ServiceGeneral).Name.ToLower()} HandleAsync: Erro no banco de dados - Exception: {ex.Message}");

                return null;
            }
        }
        */

        public async Task<string> GetBearerByStoneCode(int stoneCode) {
            TokenEntity tokenEntity = new TokenEntity();
            tokenEntity.stone_code = stoneCode;
            DataTable dt = await _dbRepository.FindOneBy<TokenEntity>(tokenEntity.GetTableName(), tokenEntity.GetPkName(), tokenEntity.GetPkValue());
            if (dt.Rows.Count > 0) {
                return dt.Rows[0]["bearer"].ToString();
            }
            else {
                return null;
            }
        }

    }
}