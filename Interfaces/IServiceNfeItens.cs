using PosStoneNfce.API.Portal.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Interfaces
{
    public interface IServiceNfeItens<IType, in TPayload> where IType : struct
    {
        IDBRepository<PosStone> GetDBRepository();

        /*
        Task<IEnumerable<NfeItensEntity>> ListNfeItensAsync(TPayload payload);
        */
    }
}