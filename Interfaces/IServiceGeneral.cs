using PosStoneNfce.API.Portal.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Interfaces
{
    public interface IServiceGeneral<IType, in TPayload> where IType : struct
    {
        /*
        Task<IEnumerable<GeneralEntity>> ListGeneralAsync(TPayload payload);
        */
        IDBRepository<PosStone> GetDBRepository();

        Task<string> GetBearerByStoneCode(int stoneCode);
    }
}