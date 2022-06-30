using PosStoneNfce.API.Portal.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace PosStoneNfce.API.Portal.Interfaces
{
    public interface IDBRepository<TConnection>
        where TConnection : struct
    {
        /*
        Task<IEnumerable<EmpresaEntity>> ListEmpresaAsync();
        Task<IEnumerable<GeneralEntity>> ListGeneralAsync();
        Task<IEnumerable<NfeEntity>> ListNfeAsync();
        Task<IEnumerable<NfeItensEntity>> ListNfeItensAsync();
        Task<IEnumerable<TokenEntity>> ListTokenAsync();
        */
        Task<DataTable> FindAll<T>(string TableName);
        Task<DataTable> FindOneBy<T>(string TableName, string ColumnName, string ColumnValue);
        Task<DataTable> Query(string sql);
        Task<dynamic> Execute(string sql);
        Task<dynamic> Save(IEntity entity);
    }
}