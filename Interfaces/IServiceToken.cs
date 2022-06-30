using PosStoneNfce.API.Portal.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Interfaces
{
    public interface IServiceToken<IType, in TPayload> where IType : struct
    {
        IDBRepository<PosStone> GetDBRepository();

        /*
        Task<IEnumerable<TokenEntity>> ListTokenAsync(TPayload payload);
        */

        Task<bool> SaveLoggedBearer(TokenEntity tokenEntity);

    }
}