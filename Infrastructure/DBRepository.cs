using Dapper;
using Microsoft.Extensions.Configuration;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.Infrastructure
{
    public class DBRepository<TConnection> : IDBRepository<TConnection>
        where TConnection : struct
    {
        private readonly int _commandTimeoutSeconds;
        private readonly string _connectionString;
        public IConfiguration configuration;

        public DBRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            _connectionString = this.configuration[$"DBConnectionString{typeof(TConnection).Name}"];
            _commandTimeoutSeconds = int.Parse(this.configuration["DBConnectionTimeout"]);
        }

        private static SqlConnection CreateConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            return connection;
        }

        private async Task<SqlConnection> OpenConnectionAsync()
        {
            SqlConnection cn = CreateConnection(_connectionString);
            await cn.OpenAsync();
            return cn;
        }

        public async Task<DataTable> ExecuteQueryTableAsync(string sql)
        {
            DataTable dt = null;
            SqlConnection cn = await OpenConnectionAsync();

            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandTimeout = _commandTimeoutSeconds,
                    CommandText = sql,
                    CommandType = CommandType.Text,
                    Connection = cn
                };

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                DataTable dtAux = new();
                dtAux.Load(reader);
                dt = new DataTable();
                dt = dtAux;
            }
            catch (Exception ex)
            {
                Exception exSql = new(ex.Message + " - Sql: " + sql, ex);
                throw exSql;
            }
            finally
            {
                cn.Close();
            }

            return dt;
        }

        /*
        public async Task<IEnumerable<EmpresaEntity>> ListEmpresaAsync()
        {
            using (var sqlConnection = CreateConnection(_connectionString))
            {
                var sql = "SELECT stone_code as 'StoneCode', cnpj, aceite_termos_uso as 'AceiteTermosUso', num_nfe as 'NumNFe', serie_nfe as 'SerieNFe' FROM dbo.empresa";

                var value = await sqlConnection.QueryAsync<EmpresaEntity>(sql, commandTimeout: _commandTimeoutSeconds);

                sqlConnection.Close();

                return value;
            }
        }

        public async Task<IEnumerable<GeneralEntity>> ListGeneralAsync()
        {
            using (var sqlConnection = CreateConnection(_connectionString))
            {
                var sql = "SELECT tipo as 'Tipo', texto as 'Texto' FROM dbo.general";

                var value = await sqlConnection.QueryAsync<GeneralEntity>(sql, commandTimeout: _commandTimeoutSeconds);

                sqlConnection.Close();

                return value;
            }
        }

        public async Task<IEnumerable<NfeEntity>> ListNfeAsync()
        {
            using (var sqlConnection = CreateConnection(_connectionString))
            {
                var sql = "SELECT stone_code as 'StoneCode', client_id_nota as 'ClientIdNota', chave_nfe as 'ChaveNfe', serie_nfe as 'SerieNFe', status as 'Status', motivo as 'Motivo', contingencia as 'Contingencia', dh_emissao as 'DhEmissao', dh_retorno as 'DhRetorno', tipo_pagamento as 'TipoPagamento', valor_pagamento as 'ValorPagamento', cpf_cnpj_destinatario as 'CpfCnpjDestinatario', cnpj_cc as 'CnpjCc', bandeira as 'Bandeira', cod_pgto as 'CodPgto', url_danfe as 'UrlDanfe' FROM dbo.nfe";

                var value = await sqlConnection.QueryAsync<NfeEntity>(sql, commandTimeout: _commandTimeoutSeconds);

                sqlConnection.Close();

                return value;
            }
        }

        public async Task<IEnumerable<NfeItensEntity>> ListNfeItensAsync()
        {
            using (var sqlConnection = CreateConnection(_connectionString))
            {
                var sql = "SELECT nfe_id as 'NfeId', produto_id as 'ProdutoId', qty as 'Qty', vlr as 'Vlr' FROM dbo.nfe_itens";

                var value = await sqlConnection.QueryAsync<NfeItensEntity>(sql, commandTimeout: _commandTimeoutSeconds);

                sqlConnection.Close();

                return value;
            }
        }

        public async Task<IEnumerable<TokenEntity>> ListTokenAsync()
        {
            using (var sqlConnection = CreateConnection(_connectionString))
            {
                var sql = "SELECT stone_code as 'StoneCode', usuario as 'Usuario', bearer as 'Bearer', expires as 'Expires' FROM dbo.token";

                var value = await sqlConnection.QueryAsync<TokenEntity>(sql, commandTimeout: _commandTimeoutSeconds);

                sqlConnection.Close();

                return value;
            }
        }
        */

        public async Task<DataTable> FindAll<T>(string TableName) {
            DataTable dt = new DataTable();
            var sql = "";
            try {
                using (var sqlConnection = CreateConnection(_connectionString))
                {
                    sql = $"SELECT * from [dbo].[" + TableName + "]";
                    dt = await ExecuteQueryTableAsync(sql);
                    return dt;
                }
            }
            catch (Exception e) {
                var res = new { error = true, msg = "Erro no metodo FindAll do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + e.ToString() };
            }

            return dt;
        }

        public async Task<DataTable> FindOneBy<T>(string TableName, string ColumnName, string ColumnValue) {
            DataTable dt = new DataTable();
            var sql = "";
            try {
                using (var sqlConnection = CreateConnection(_connectionString))
                {
                    sql = $"SELECT TOP 1 * from [dbo].[" + TableName + "] where " + ColumnName + " = '" + ColumnValue + "'";
                    dt = await ExecuteQueryTableAsync(sql);
                    return dt;
                }
            }
            catch (Exception e) {
                var res = new { error = true, msg = "Erro no metodo FindOneBy do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + e.ToString() };
            }

            return dt;
        }


        public async Task<DataTable> Query(string sql) {
            DataTable dt = new DataTable();
            try {
                using (var sqlConnection = CreateConnection(_connectionString))
                {
                    //var value = await sqlConnection.QueryAsync<T>(sql, commandTimeout: _commandTimeoutSeconds);
                    dt = await ExecuteQueryTableAsync(sql);
                    
                }
            }
            catch (Exception e) {
                var res = new { error = true, msg = "Erro no metodo Query do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + e.ToString() };
            }

            return dt;
        }

        public async Task<dynamic> Save(IEntity entity) {
            object res = new {};
            var sql = "";
            try {
                string TableName = entity.GetTableName();
                string pkName = entity.GetPkName();
                string pkValue = entity.GetPkValue();
                dynamic executed = new {};
                dynamic resExecution = new { error = false };
                DataTable dt = await FindOneBy<IEntity>(TableName, pkName, pkValue);
                PropertyInfo[] properties = entity.GetType().GetProperties();

                if (dt.Rows.Count > 0) {
                    // Update no registro no banco
                    List<string> sets = new List<string>();
                    foreach (PropertyInfo property in properties)
                    {
                        string name = property.Name;
                        string value = entity.GetType().GetProperty(name).GetValue(entity).ToString();
                        sets.Add($"{name} = '{value}'");
                    }

                    string sComma = string.Join(", ", sets);
                    sql = $"UPDATE [dbo].[{TableName}] SET {sComma} WHERE {pkName} = {pkValue}";
                    executed = await Execute(sql);
                    if (executed.error == false) {
                        resExecution = new { error = false, pk = pkValue };
                    }
                    else {
                        resExecution = new { error = true, msg = "EXECUTE: Erro no metodo Save do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + executed.msg };
                    }
                }
                else {
                    // Insert novo registro no banco
                    List<string> fields = new List<string>();
                    List<string> values = new List<string>();
                    foreach (PropertyInfo property in properties)
                    {
                        string name = property.Name;
                        fields.Add(name);

                        string value = entity.GetType().GetProperty(name).GetValue(entity).ToString();
                        values.Add($"'{value}'");
                    }

                    string sFields = string.Join(", ", fields);
                    string sValues = string.Join(", ", values);

                    sql = $"INSERT INTO [dbo].[{TableName}]({sFields}) OUTPUT INSERTED.id VALUES({sValues})";

                    executed = await ExecuteScalar(sql);
                    if (executed.error == false) {
                        resExecution = new { error = false, pk = executed.pk };
                    }
                    else {
                        resExecution = new { error = true, msg = "EXECUTESCALAR: Erro no metodo Save do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + executed.msg };
                    }
                }

                if (resExecution.error == false) {
                    res = new { error = false, pk = resExecution.pk};
                }
                else {
                    res = new { error = true, msg = "Erro no metodo Save do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + resExecution.msg };
                }
            }
            catch (Exception e) {
                res = new { error = true, msg = " - EXCEPTION: Erro no metodo Save do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + e.ToString() };
            }

            return res;
        }

        public async Task<dynamic> Execute(string sql) {
            object response = new {};
            try {
                SqlConnection cn = await OpenConnectionAsync();
                SqlCommand cmd = new SqlCommand
                {
                    CommandTimeout = _commandTimeoutSeconds,
                    CommandText = sql,
                    CommandType = CommandType.Text,
                    Connection = cn
                };
                
                var executed = await cmd.ExecuteNonQueryAsync();
                response = new { error = false, msg = ""};
            }
            catch (Exception e) {
                response = new { error = true, msg = "Erro no metodo Execute do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + e.ToString()};
            }

            return response;
        }


        public async Task<dynamic> ExecuteScalar(string sql) {
            object response = new {};
            try {
                SqlConnection cn = await OpenConnectionAsync();
                SqlCommand cmd = new SqlCommand
                {
                    CommandTimeout = _commandTimeoutSeconds,
                    CommandText = sql,
                    CommandType = CommandType.Text,
                    Connection = cn
                };
                
                int id = (int) cmd.ExecuteScalar();
                response = new { error = false, pk = id};
            }
            catch (Exception e) {
                response = new { error = true, msg = "Erro no metodo ExecuteScalar do DBRepository: SQL = " + sql + " --- Using conn = " + _connectionString + " --- " + e.ToString()};
            }

            return response;
        }

    }
}