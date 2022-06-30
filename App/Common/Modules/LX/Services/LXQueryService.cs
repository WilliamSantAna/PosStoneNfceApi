using System.Data.SqlClient;
using System.Threading.Tasks;
using PosStoneNfce.API.Portal.App.Common.Services;
using SqlKata;
using SqlKata.Execution;

namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{
    public interface ILXQueryService
    {
        Task<LXFindAll<T>> FindAll<T>(LXFindAllInput input);
        Task<T> FindOne<T>(LXFindOneInput input);
    }

    public class LXQueryService : ILXQueryService
    {

        private QueryFactory _db;

        public LXQueryService(
            QueryFactory queryFactory)
        {
            _db = queryFactory;
        }


        public async Task<LXFindAll<T>> FindAll<T>(LXFindAllInput input)
        {
            var queryCount = _db.Query(input.Table +" AS "+ input.As);

            var attribute = (input.Attributes != null && input.Attributes.Length > 0) ? input.Attributes :  new string[] { "*" };
            var queryRows = 
                _db.Query(input.Table +" AS "+ input.As)
                    .Select(attribute)
                    .Limit(input.PageSize)
                    .Offset(input.PageSize * (input.PageIndex - 1));

            if (input.SortField != null)
            {
                if (input.SortOrder.ToString() == "descend") 
                    queryRows.OrderByDesc(input.SortField);
                else 
                    queryRows.OrderBy(input.SortField);
            }

            if (input.Filter != null &&  input.Filter.Length > 0)
            {
                SetConditions(queryCount, input.Filter);
                SetConditions(queryRows, input.Filter);
            }

            if (input.BeforeExecute != null) 
            {
                input.BeforeExecute(queryCount);
                input.BeforeExecute(queryRows);
            }

            var _count = await queryCount.CountAsync<int>();
            var _rows = await queryRows.GetAsync<T>();

            return new LXFindAll<T>
            {
                Count = _count,
                Rows = _rows
            };
        }



        public async Task<T> FindOne<T>(LXFindOneInput input)
        {
            var query = 
                _db.Query(input.Table +" AS "+ input.As)
                    .Select(input.Attributes)
                    .Limit(1);

            SetConditions(query, input.Filter);

            if (input.BeforeExecute != null)
                input.BeforeExecute(query);

            var _item = await query.FirstOrDefaultAsync<T>();

            return _item;
        }


        public async Task<LXCreate<A>> Create<T, A>(LXCreateInput<T> input, SqlTransaction transaction = null) 
        {
            var result = await _db.Query(input.Table)
                                .InsertGetIdAsync<A>(input.Data, transaction);

            return new LXCreate<A> {
                Id = result
            };
        }


        public async Task<LXUpdate> Update<T>(LXUpdateInput<T> input, SqlTransaction transaction = null) 
        {
            var query = _db.Query(input.Table);
            this.SetConditions(query, input.Where);

            var result = await query.UpdateAsync(input.Data, transaction);

            return new LXUpdate {
                AffectedRows = result
            };
        }


        public async Task<LXDelete> Delete(LXDeleteInput input, SqlTransaction transaction = null) 
        {
            var query = _db.Query(input.Table);
            this.SetConditions(query, input.Where);

            var result = await query.DeleteAsync(transaction);

            return new LXDelete {
                AffectedRows = result
            };
        }



        private void SetConditions(Query query, string[][] filter)
        {
            if (filter != null)
            {
                foreach (var item in filter)
                {
                    query.Where(item[0], item[1], item[2]);
                }
            }
        }


        private void SetJoins(Query query, LXJoin[] joins, string sourceTable)
        {
            foreach (var join in joins)
            {
                query.LeftJoin(join.Table + " As " + join.As, j => {
                    var r = j.On((join.As != null ? join.As : join.Table) +"."+ join.SourceKey, sourceTable +"."+ join.TargetKey);
                    return r;
                });
            }
        }


        private async Task<T> GetEntityId<T>(string table, string select, string[] where)
        {
            var q = await _db.Query(table).SelectRaw(select + " AS Id")
                            .Where(where[0], where[1], where[2])
                            .FirstOrDefaultAsync<EntityID<T>>();
                                    
            return q.Id;
        }

    }
}