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
    public class EmpresaProcessor : AuthProcessor
    {

        public EmpresaProcessor(IDBRepository<PosStone> dbRepository): base(dbRepository) {
            _dbRepository = dbRepository;
        }

        public dynamic EmpresaSave(EmpresaSaveRequest empresaSaveRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/Empresa";
            object response = new {};
            dynamic apiData = "";
            try 
            {
                var resAuth = AuthMaster();
                if (resAuth.error == false) {
                    string bearer = resAuth.bearer.ToString();

                    // Salvar o stone_code e cnpj no banco
                    EmpresaEntity empresaEntity = new EmpresaEntity();
                    empresaEntity.stone_code = Int32.Parse(empresaSaveRequest.stone_code);
                    empresaEntity.cnpj = empresaSaveRequest.cnpj;
                    var saved = _dbRepository.Save(empresaEntity).Result;
                    if (saved.error == false) {
                        DateTime date = DateTime.Now;
                        string dateNowFormatted = date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff");

                        object[] Produtos = new object[3];
                        Produtos[0] = new {
                            Id = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:PRODUTO_1"]),
                            TipoAmbiente = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_AMBIENTE"]),
                            InicioUtilizacao = dateNowFormatted,
                            FinalUtilizacao = "2030-03-01T00:00:00.0000000"
                        };
                        Produtos[1] = new {
                            Id = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:PRODUTO_2"]),
                            TipoAmbiente = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_AMBIENTE"]),
                            InicioUtilizacao = dateNowFormatted,
                            FinalUtilizacao = "2030-03-01T00:00:00.0000000"
                        };
                        Produtos[2] = new {
                            Id = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:PRODUTO_3"]),
                            TipoAmbiente = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_AMBIENTE"]),
                            InicioUtilizacao = dateNowFormatted,
                            FinalUtilizacao = "2030-03-01T00:00:00.0000000"
                        };

                        object[] ResponsavelLegal = new object[1];
                        ResponsavelLegal[0] = new {
                            Nome = empresaSaveRequest.razao_social,
                            Email = empresaSaveRequest.email
                        };

                        object dataToFF = new {
                            Data = new {
                                NumeroOp = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:NUMERO_OP"]),
                                Source = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:SOURCE"]),
                                usuariosPortal = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:USUARIOS_PORTAL"]),
                                GrupoEconomico = new {
                                    CodigoIndicadorMoeda = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:CODIGO_INDICADOR_MOEDA"]),
                                    Descricao = empresaSaveRequest.razao_social
                                },
                                Pessoa = new {
                                    Tipo = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_PESSOA"]),
                                    NomeFantasia = empresaSaveRequest.razao_social,
                                    CNPJCPF = empresaSaveRequest.cnpj,
                                    RazaoSocialNome = empresaSaveRequest.razao_social,
                                    InscricaoEstadual = empresaSaveRequest.ie,
                                    CodigoRegimeTributario = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:CODIGO_REGIME_TRIBUTARIO"]),
                                    CNAE = ApiConfig.Configuration["ApiDefinitions:CNAE"],
                                    CNPJFaturamento = empresaSaveRequest.cnpj,
                                    GrupoEconomicoId = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:GRUPO_ECONOMICO_ID"]),
                                    IdSolucao = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:ID_SOLUCAO"]),
                                    Endereco = new {
                                        TipoLogradouro = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_LOGRADOURO"]),
                                        Logradouro = empresaSaveRequest.endereco,
                                        Numero = empresaSaveRequest.numero,
                                        Bairro = empresaSaveRequest.bairro,
                                        Cep = empresaSaveRequest.cep,
                                        CodigoIBGEMunicipio = Int32.Parse(empresaSaveRequest.cod_ibge_municipio),
                                        UF = empresaSaveRequest.uf
                                    },
                                    Contato = new {
                                        Email = empresaSaveRequest.email
                                    }
                                },
                                Cadastro = new {
                                    CNPJMatriz = empresaSaveRequest.cnpj,
                                    TipoFilial = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_FILIAL"])
                                },
                                Produtos = Produtos,
                                ResponsavelLegal = ResponsavelLegal,
                                Logo = new {
                                    LogoBase64 = "",
                                    LogoEmbranco = true
                                }
                            }
                        };

                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        if (!ObjectExtensions.PropertyExist(res, "errors")) {
                            // Criou empresa com sucesso
                            // Vamos atrelar o Csc a empresa
                            if (empresaSaveRequest.id_csc != null) {
                                res = CscCreate(empresaSaveRequest);
                                if (res.error == false) {
                                    response = new { error = false, msg = "Empresa salva com sucesso"};
                                }
                                else {
                                    response = res;
                                }
                            }
                            else {
                                response = new { error = false, msg = "Empresa salva com sucesso"};
                            }
                        }
                        else {
                            string payload = CurlService.GetPayload(url, dataToFF, bearer);
                            response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                        }
                    }
                    else {
                        response = new { error = true, msg = url + " - PosStone Error: Erro ao salvar empresa no ambiente da api: " + saved.msg};
                    }
                }
                else {
                    response = new { error = true, msg = url + " - FiscalFlow Error: Erro ao autenticar usuario master: " + resAuth.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }

        public dynamic CscCreate(EmpresaSaveRequest empresaSaveRequest) {
            string httpResponse = "";
            dynamic apiData = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/Csc/Create";
            object response = new {};
            try 
            {
                var resAuth = AuthMaster();
                if (resAuth.error == false) 
                {
                    string bearer = resAuth.bearer.ToString();

                    SiglaRequest siglaRequest = new SiglaRequest();
                    siglaRequest.sigla = empresaSaveRequest.uf;
                    var resSigla = GetUfBySigla(siglaRequest);
                    if (resSigla.error == false) {
                        object dataToFF = new {
                            Data = new {
                                Token = empresaSaveRequest.cod_csc,
                                UfId = resSigla.uf.CodigoIbge,
                                TokenGovId = empresaSaveRequest.id_csc,
                                EmpresaCNPJ = empresaSaveRequest.cnpj,
                                GrupoEconomicoId = ApiConfig.Configuration["ApiDefinitions:GRUPO_ECONOMICO_ID"],
                            }
                        };
                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        if (!ObjectExtensions.PropertyExist(res, "errors")) {
                            response = new { error = false, empresa = res};
                        }
                        else {
                            response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                        }
                    }
                    else {
                        response = new { error = true, msg = url + " - " + resSigla.msg};
                    }
                }
                else {
                    response = new { error = true, msg = url + " - FiscalFlow Error: Erro ao autenticar usuario master: " + resAuth.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }

        public dynamic EmpresaGetByStoneCode(StoneCodeRequest stoneCodeRequest) {
            string httpResponse = "";
            dynamic apiData = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/Empresa/BuscarEmpresaPorCNPJ";
            object response = new {};
            try 
            {
                var resAuth = IsLoggedIn(stoneCodeRequest);
                if (resAuth.error == false) {
                    if (resAuth.is_logged_in == true) {
                        string bearer = resAuth.bearer.ToString();

                        EmpresaEntity empresaEntity = new EmpresaEntity();
                        empresaEntity.stone_code = Int32.Parse(stoneCodeRequest.stone_code);
                        DataTable dt = _dbRepository.FindOneBy<EmpresaEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                        if (dt.Rows.Count > 0) {
                            string cnpj = dt.Rows[0]["cnpj"].ToString();

                            object dataToFF = new {
                                Data = cnpj
                            };

                            httpResponse = CurlService.Post(url, dataToFF, bearer);
                            var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                            if (!ObjectExtensions.PropertyExist(res, "errors")) {
                                response = new { error = false, empresa = new {
                                    IdMidPfj = res.Data.IdMidPfj.ToString(),
                                    NomeFantasiaApelido = res.Data.NomeFantasiaApelido.ToString(),
                                    CnpjCpf = res.Data.CnpjCpf.ToString(),
                                    RazaoSocialNomeCompleto = res.Data.RazaoSocialNomeCompleto.ToString(),
                                    IdGpecon = res.Data.IdGpecon.ToString(),
                                }};
                            }
                            else {
                                response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                            }
                        }
                        else {
                            response = new { error = true, msg = url + " - PosStone Msg: Empresa nao existe na api"};
                        }
                    }
                    else {
                        response = new { error = true, msg = url + " - PosStone Msg: Usuario nao esta logado: "};
                    }
                }
                else {
                    response = new { error = true, msg = url + " - PosStone Msg: Usuario nao esta logado: " + resAuth.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }



        public dynamic SetAceiteTermosUso(SetAceiteTermosUsoRequest setAceiteTermosUsoRequest) {
            object response = new {};
            try 
            {

                EmpresaEntity empresaEntity = new EmpresaEntity();
                empresaEntity.stone_code = Int32.Parse(setAceiteTermosUsoRequest.stone_code);
                DataTable dt = _dbRepository.FindOneBy<EmpresaEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    empresaEntity.aceite_termos_uso = Int32.Parse(setAceiteTermosUsoRequest.aceite_termos_uso);
                    empresaEntity.cnpj = dt.Rows[0]["cnpj"].ToString();
                    empresaEntity.num_nfe = Int32.Parse(dt.Rows[0]["num_nfe"].ToString());
                    empresaEntity.serie_nfe = Int32.Parse(dt.Rows[0]["serie_nfe"].ToString());

                    var saved = _dbRepository.Save(empresaEntity).Result;
                    if (saved.error == false) {
                        response = new { error = false, aceite_termos_uso = setAceiteTermosUsoRequest.aceite_termos_uso };
                    }
                    else {
                        response = new { error = true, msg = saved.msg };
                    }
                }
                else {
                    response = new { error = true, msg = "PosStone Error: Empresa nao existe na api"};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }
        



        public dynamic GetAceiteTermosUso(StoneCodeRequest stoneCodeRequest) {
            object response = new {};
            try 
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();
                empresaEntity.stone_code = Int32.Parse(stoneCodeRequest.stone_code);
                DataTable dt = _dbRepository.FindOneBy<TokenEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    response = new { error = false, aceite_termos_uso = dt.Rows[0]["aceite_termos_uso"].ToString()};
                }
                else {
                    response = new { error = false, aceite_termos_uso = ""};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }


        public dynamic EmpresaAddProduto(ProdutoRequest produtoRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/SmartPOS/ProdutoEmpresa/Create";
            object response = new {};
            dynamic apiData = "";
            try 
            {
                StoneCodeRequest stoneCodeRequest = new StoneCodeRequest();
                stoneCodeRequest.stone_code = produtoRequest.stone_code;
                var resAuth = IsLoggedIn(stoneCodeRequest);
                if (resAuth.error == false) {
                    string bearer = resAuth.bearer.ToString();

                    EmpresaEntity empresaEntity = new EmpresaEntity();
                    empresaEntity.stone_code = Int32.Parse(produtoRequest.stone_code);
                    DataTable dt = _dbRepository.FindOneBy<EmpresaEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                    if (dt.Rows.Count > 0) {
                        object dataToFF = new {
                            Data = new {
                                Id = produtoRequest.produto_id,
                                ProdutoId = produtoRequest.produto_id,
                                EmpresaCNPJ = dt.Rows[0]["cnpj"].ToString(),
                                UnidadeMedidaId = "UND",
                                CFOP = "5101",
                            }
                        };

                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        string payload = CurlService.GetPayload(url, dataToFF, bearer);
                        var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        if (!ObjectExtensions.PropertyExist(res, "errors")) {
                            response = new { error = false, msg = ""};
                        }
                        else {
                            response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                        }

                    }
                    else {
                        response = new { error = true, msg = url + " - PosStone Error: Empresa nao existe na api"};
                    }
                }
                else {
                    response = new { error = true, msg = url + " - PosStone Error: Usuario nao esta logado: " + resAuth.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;
        }

        public dynamic EmpresaRemoveProduto(ProdutoRequest produtoRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/SmartPOS/ProdutoEmpresa/Delete";
            object response = new {};
            dynamic apiData = "";
            try 
            {
                StoneCodeRequest stoneCodeRequest = new StoneCodeRequest();
                stoneCodeRequest.stone_code = produtoRequest.stone_code;
                var resAuth = IsLoggedIn(stoneCodeRequest);
                if (resAuth.error == false) {
                    string bearer = resAuth.bearer.ToString();

                    object dataToFF = new {
                        Data = new {
                            Id = produtoRequest.produto_id,
                        }
                    };

                    httpResponse = CurlService.Post(url, dataToFF, bearer);
                    var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                    if (!ObjectExtensions.PropertyExist(res, "errors")) {
                        response = new { error = false, msg = ""};
                    }
                    else {
                        response = new { error = true, msg = url + " - FiscalFloww Error: " + res.errors.ToString() };
                    }

                }
                else {
                    response = new { error = true, msg = url + " - PosStone Error: Usuario nao esta logado: " + resAuth.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }

    }
}