using System;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.App.Common.Controller;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models;
using FiscalFlow.Common.Services.Cryptography;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Response;
using PosStoneNfce.API.Portal.App.Common.Utils;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using PosStoneNfce.API.Portal.Configuration;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Exceptions;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services
{
    public class GeneralProcessor : AuthProcessor
    {
        public GeneralProcessor(IDBRepository<PosStone> dbRepository): base(dbRepository) {
            _dbRepository = dbRepository;
        }

        public dynamic GetCidadesByUf(SiglaRequest siglaRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/UnidadeFederativa/buscar-cidades";
            object response = new {};
            try {
                var res = GetUfBySigla(siglaRequest);
                if (res.error == false) {
                    var ufIbgeCode = res.uf.CodigoIbge;
                    
                    var resAuth = AuthMaster();
                    if (resAuth.error == false) {
                        string bearer = resAuth.bearer.ToString();

                        object dataToFF = new {
                            Data = ufIbgeCode
                        };

                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        JObject o = JObject.Parse(httpResponse);
                        JArray a = (JArray)o["Data"];
                        IList<DataFiscalFlowCidadesResponse> cids = a.ToObject<IList<DataFiscalFlowCidadesResponse>>();
                        // dynamic res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        // List<DataFiscalFlowUfsResponse> apiData = res.Data;
                        object[] cidsas = new object[cids.Count];
                        int i = 0;
                        foreach (DataFiscalFlowCidadesResponse cid in cids) {
                            cidsas[i] = new {
                                CodigoIbge = cid.CodigoIbge,
                                Nome = cid.Nome,
                                EstadoId = cid.EstadoId,
                                CodigoMID = cid.CodigoMID,
                            };
                            i++;
                        }
                        response = new { error = false, cids = cidsas};
                    }
                    else {
                        response = new { error = true, msg = url + " - FiscalFlow Error: Erro ao autenticar usuario master: " + resAuth.msg};
                    }
                }
                else {
                    response = new { error = true, msg = res.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;
        }




        public dynamic GetPoliticaPrivacidade() {
            object response = new {};
            try 
            {
                GeneralEntity generalEntity = new GeneralEntity();
                generalEntity.tipo = "PP";
                DataTable dt = _dbRepository.FindOneBy<TokenEntity>(generalEntity.GetTableName(), generalEntity.GetPkName(), generalEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    response = new { error = false, politica_privacidade = dt.Rows[0]["texto"].ToString()};
                }
                else {
                    response = new { error = false, politica_privacidade = ""};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }


        public dynamic GetTermosUso() {
            object response = new {};
            try 
            {
                GeneralEntity generalEntity = new GeneralEntity();
                generalEntity.tipo = "TU";
                DataTable dt = _dbRepository.FindOneBy<TokenEntity>(generalEntity.GetTableName(), generalEntity.GetPkName(), generalEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    response = new { error = false, termos_uso = dt.Rows[0]["texto"].ToString()};
                }
                else {
                    response = new { error = false, termos_uso = ""};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }

        public dynamic GetRawData(SqlRequest sqlRequest) {
            object response = new {};
            try 
            {
                DataTable dt = _dbRepository.Query(sqlRequest.raw_instruction).Result;
                if (dt.Rows.Count > 0) {
                    response = new { error = false, result = ObjectExtensions.FromDataTableToJSON(dt)};
                }
                else {
                    response = new { error = false, result = ""};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }
        public dynamic ExecuteRawData(SqlRequest sqlRequest) {
            object response = new {};
            try 
            {
                var res = _dbRepository.Execute(sqlRequest.raw_instruction).Result;
                response = new { error = false, result = res.msg};
            }
            catch (Exception e) {
                response = new { error = true, msg = "PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }
        
    }
}