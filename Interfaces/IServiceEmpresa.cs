using PosStoneNfce.API.Portal.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Interfaces
{
    public interface IServiceEmpresa<IType, in TPayload> where IType : struct
    {
        /*
        Task<IEnumerable<EmpresaEntity>> ListEmpresaAsync(TPayload payload);
        */
        IDBRepository<PosStone> GetDBRepository();
    }
}