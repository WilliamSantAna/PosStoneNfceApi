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
    public class ProdutoProcessor : AuthProcessor
    {
        public ProdutoProcessor(IDBRepository<PosStone> dbRepository): base(dbRepository) {
            _dbRepository = dbRepository;
        }

        public dynamic ProdutoGetAll(StoneCodeRequest stoneCodeRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/SmartPOS/Produto/FindAll";
            object response = new {};
            try 
            {
                var resAuth = IsLoggedIn(stoneCodeRequest);
                if (resAuth.error == false) {
                    if (resAuth.is_logged_in == true) {
                        string bearer = resAuth.bearer.ToString();

                        string[] Attributes = new string[12];
                        Attributes[0] = "produto.id";
                        Attributes[1] = "produto.codigo";
                        Attributes[2] = "produto.nome";
                        Attributes[3] = "produto.nomeCurto";
                        Attributes[4] = "produto.ncm";
                        Attributes[5] = "produto.gtin";
                        Attributes[6] = "produto.gtinTributavel";
                        Attributes[7] = "produto.imagemUrl";
                        Attributes[8] = "produto.tags";
                        Attributes[9] = "produto.unidadeMedidaId";
                        Attributes[10] = "produto.criadoEm";
                        Attributes[11] = "produto.atualizadoEm";

                        var Filter = new List<string>().ToArray();

                        object dataToFF = new {
                            PageSize = 9999,
                            PageIndex = 1,
                            Attributes = Attributes,
                            Filter = Filter
                        };

                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        string payload = CurlService.GetPayload(url, dataToFF, bearer);
                        var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        if (!ObjectExtensions.PropertyExist(res, "errors")) {
                            JObject o = JObject.Parse(httpResponse);
                            JArray a = (JArray)o["Rows"];
                            IList<DataFiscalFlowProdutosResponse> produtos = a.ToObject<IList<DataFiscalFlowProdutosResponse>>();
                            // dynamic res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                            // List<DataFiscalFlowUfsResponse> apiData = res.Data;
                            object[] aProdutos = new object[produtos.Count];
                            int i = 0;
                            foreach (DataFiscalFlowProdutosResponse produto in produtos) {
                                aProdutos[i] = new {
                                    Id = (produto.Id != null ? produto.Id.ToString() : ""),
                                    Codigo = (produto.Codigo != null ? produto.Codigo.ToString() : ""),
                                    Nome = (produto.Nome != null ? produto.Nome.ToString() : ""),
                                    NomeCurto = (produto.NomeCurto != null ? produto.NomeCurto.ToString() : ""),
                                    NCM = (produto.NCM != null ? produto.NCM.ToString() : ""),
                                    GTIN = (produto.GTIN != null ? produto.GTIN.ToString() : ""),
                                    GTINTributavel = (produto.GTINTributavel != null ? produto.GTINTributavel.ToString() : ""),
                                    Tags = (produto.Tags != null ? produto.Tags.ToString() : ""),
                                    UnidadeMedidaId = (produto.UnidadeMedidaId != null ? produto.UnidadeMedidaId.ToString() : ""),
                                    CriadoEm = (produto.CriadoEm != null ? produto.CriadoEm.ToString() : ""),
                                };
                                i++;
                            }
                            response = new { error = false, produtos = aProdutos};
                        }
                        else {
                            response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                        }
                    }
                    else {
                        response = new { error = true, msg = url + " - PosStone Error: Usuario nao esta logado: "};
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

        public dynamic ProdutoGetById(ProdutoRequest produtoRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/SmartPOS/Produto/FindOne";
            object response = new {};
            string bearerMaster = "";
            try 
            {

                var resAuthMaster = AuthMaster();
                if (resAuthMaster.error == false) {
                    bearerMaster = resAuthMaster.bearer.ToString();
                }
                
                StoneCodeRequest stoneCodeRequest = new StoneCodeRequest();
                stoneCodeRequest.stone_code = produtoRequest.stone_code;
                var resAuth = IsLoggedIn(stoneCodeRequest);
                if (resAuth.error == false) {
                    if (resAuth.is_logged_in == true) {
                        string bearer = resAuth.bearer.ToString();


                        object[] Filter = new object[1];
                        object[] Filter1 = new object[3];
                        Filter1[0] = "produto.Id";
                        Filter1[1] = "=";
                        Filter1[2] = produtoRequest.produto_id;
                        Filter[0] = Filter1;

                        
                        string[] Attributes = new string[12];
                        Attributes[0] = "produto.id";
                        Attributes[1] = "produto.codigo";
                        Attributes[2] = "produto.nome";
                        Attributes[3] = "produto.nomeCurto";
                        Attributes[4] = "produto.ncm";
                        Attributes[5] = "produto.gtin";
                        Attributes[6] = "produto.gtinTributavel";
                        Attributes[7] = "produto.imagemUrl";
                        Attributes[8] = "produto.tags";
                        Attributes[9] = "produto.unidadeMedidaId";
                        Attributes[10] = "produto.criadoEm";
                        Attributes[11] = "produto.atualizadoEm";

                        object dataToFF = new {
                            Filter = Filter,
                            Attributes = Attributes
                        };

                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        string payload = CurlService.GetPayload(url, dataToFF, bearer);
                        var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        if (!ObjectExtensions.PropertyExist(res, "errors")) {
                            object produto = new {
                                Id = (res.Id != null ? res.Id.ToString() : ""),
                                Codigo = (res.Codigo != null ? res.Codigo.ToString() : ""),
                                Nome = (res.Nome != null ? res.Nome.ToString() : ""),
                                NomeCurto = (res.NomeCurto != null ? res.NomeCurto.ToString() : ""),
                                NCM = (res.NCM != null ? res.NCM.ToString() : ""),
                                GTIN = (res.GTIN != null ? res.GTIN.ToString() : ""),
                                GTINTributavel = (res.GTINTributavel != null ? res.GTINTributavel.ToString() : ""),
                                Tags = (res.Tags != null ? res.Tags.ToString() : ""),
                                UnidadeMedidaId = (res.UnidadeMedidaId != null ? res.UnidadeMedidaId.ToString() : ""),
                                CriadoEm = (res.CriadoEm != null ? res.CriadoEm.ToString() : ""),
                            };
                            response = new { error = false, produto = produto};
                        }
                        else {
                            response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                        }
                    }
                    else {
                        response = new { error = true, msg = url + " - PosStone Error: Usuario nao esta logado: "};
                    }
                }
                else {
                    response = new { error = true, msg = url + " - PosStone Error: Usuario nao esta logado: " + resAuth.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - Exception Error: " + e.ToString()};
            }

            return response;
        }
    
        public dynamic ProdutoGetByStoneCode(StoneCodeRequest stoneCodeRequest) {
            string httpResponse = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_SERVER"] + "/api/SmartPOS/ProdutoEmpresa/FindAll";
            object response = new {};
            try 
            {
                var resAuth = IsLoggedIn(stoneCodeRequest);
                if (resAuth.error == false) {
                    if (resAuth.is_logged_in == true) {
                        string bearer = resAuth.bearer.ToString();

                        string[] Attributes = new string[14];
                        Attributes[0] = "produtoEmpresa.Id";
                        Attributes[1] = "produtoEmpresa.NomeComercial";
                        Attributes[2] = "produtoEmpresa.Preco";
                        Attributes[3] = "produtoEmpresa.Custo";
                        Attributes[4] = "produtoEmpresa.Codigo";
                        Attributes[5] = "produtoEmpresa.CodigoFornecedor";
                        Attributes[6] = "produtoEmpresa.Tags";
                        Attributes[7] = "produtoEmpresa.ImagemUrl";
                        Attributes[8] = "produtoEmpresa.ProdutoId";
                        Attributes[9] = "produtoEmpresa.EmpresaCNPJ";
                        Attributes[10] = "produtoEmpresa.UnidadeMedidaId";
                        Attributes[11] = "produtoEmpresa.CriadoEm";
                        Attributes[12] = "produtoEmpresa.AtualizadoEm";
                        Attributes[13] = "produtoEmpresa.CFOP";

                        object[] Joins = new object[2];
                        Joins[0] = new { Name = "Impostos" };

                        string[] JoinProdutoAttributes = new string[7];
                        JoinProdutoAttributes[0] = "Id";
                        JoinProdutoAttributes[1] = "Nome";
                        JoinProdutoAttributes[2] = "NCM";
                        JoinProdutoAttributes[3] = "UnidadeMedidaId";
                        JoinProdutoAttributes[4] = "GTINTributavel";
                        JoinProdutoAttributes[5] = "CriadoEm";
                        JoinProdutoAttributes[6] = "AtualizadoEm";
                        Joins[1] = new { Name = "Produto", Attributes = JoinProdutoAttributes };

                        var Filter = new List<string>().ToArray();

                        object dataToFF = new {
                            PageSize = 9999,
                            PageIndex = 1,
                            SortFiled = "CriadoEm",
                            SortOrder = "ascend",
                            Attributes = Attributes,
                            Joins = Joins,
                            Filter = Filter
                        };

                        httpResponse = CurlService.Post(url, dataToFF, bearer);
                        string payload = CurlService.GetPayload(url, dataToFF, bearer);
                        var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                        if (!ObjectExtensions.PropertyExist(res, "errors")) {
                            JObject o = JObject.Parse(httpResponse);
                            JArray a = (JArray)o["Rows"];
                            IList<DataFiscalFlowProdutosEmpresaResponse> produtos = a.ToObject<IList<DataFiscalFlowProdutosEmpresaResponse>>();
                            // dynamic res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                            // List<DataFiscalFlowUfsResponse> apiData = res.Data;
                            object[] aProdutos = new object[produtos.Count];
                            int i = 0;
                            foreach (DataFiscalFlowProdutosEmpresaResponse produto in produtos) {
                                object Impostos = new {
                                    PIS_CST = "", 
                                    COFINS_CST = "",  
                                    PIS_Aliquota = 0,  
                                    COFINS_Aliquota = 0,  
                                    IPI_Aliquota = 0,  
                                    ICMS_Aliquota = 0, 
                                    ICMS_CST_CSOSN = "", 
                                    ICMS_ReducaoBc = 0, 
                                    ICMS_ST_ReducaoBc = 0, 
                                    ICMS_ST_Aliquota = 0, 
                                    ICMS_CodigoBeneficio = "0", 
                                    ICMS_DiffAliquota = 0, 
                                    ICMS_AmparoLegal = "" 
                                };
                                if (produto.Impostos != null) {
                                    Impostos = new {
                                        PIS_CST = (produto.Impostos.PIS_CST != null ? produto.Impostos.PIS_CST : ""), 
                                        COFINS_CST = (produto.Impostos.COFINS_CST != null ? produto.Impostos.COFINS_CST : ""),  
                                        PIS_Aliquota = (produto.Impostos.PIS_Aliquota != null ? Double.Parse(produto.Impostos.PIS_Aliquota) : 0),  
                                        COFINS_Aliquota = (produto.Impostos.COFINS_Aliquota != null ? Double.Parse(produto.Impostos.COFINS_Aliquota) : 0),  
                                        IPI_Aliquota = (produto.Impostos.IPI_Aliquota != null ? Double.Parse(produto.Impostos.IPI_Aliquota) : 0),  
                                        ICMS_Aliquota = (produto.Impostos.ICMS_Aliquota != null ? Double.Parse(produto.Impostos.ICMS_Aliquota) : 0), 
                                        ICMS_CST_CSOSN = (produto.Impostos.ICMS_CST_CSOSN != null ? produto.Impostos.ICMS_CST_CSOSN : ""), 
                                        ICMS_ReducaoBc = (produto.Impostos.ICMS_ReducaoBc != null ? Double.Parse(produto.Impostos.ICMS_ReducaoBc) : 0), 
                                        ICMS_ST_ReducaoBc = (produto.Impostos.ICMS_ST_ReducaoBc != null ? Double.Parse(produto.Impostos.ICMS_ST_ReducaoBc) : 0), 
                                        ICMS_ST_Aliquota = (produto.Impostos.ICMS_ST_Aliquota != null ? Double.Parse(produto.Impostos.ICMS_ST_Aliquota) : 0), 
                                        ICMS_CodigoBeneficio = (produto.Impostos.ICMS_CodigoBeneficio != null ? produto.Impostos.ICMS_CodigoBeneficio : "0"), 
                                        ICMS_DiffAliquota = (produto.Impostos.ICMS_DiffAliquota != null ? Double.Parse(produto.Impostos.ICMS_DiffAliquota) : 0), 
                                        ICMS_AmparoLegal = (produto.Impostos.ICMS_AmparoLegal != null ? produto.Impostos.ICMS_AmparoLegal : "") 
                                    };
                                }

                                aProdutos[i] = new {
                                    Id = (produto.Id != null ? produto.Id.ToString() : ""),
                                    Codigo = (produto.Codigo != null ? produto.Codigo.ToString() : ""),
                                    Nome = (produto.NomeComercial != null ? produto.NomeComercial.ToString() : ""),
                                    NomeCurto = (produto.NomeComercial != null ? produto.NomeComercial.ToString() : ""),
                                    Preco = produto.Preco,
                                    NCM = (produto.Produto.NCM != null ? produto.Produto.NCM.ToString() : ""),
                                    GTIN = (produto.Produto.GTINTributavel != null ? produto.Produto.GTINTributavel.ToString() : ""),
                                    GTINTributavel = (produto.Produto.GTINTributavel != null ? produto.Produto.GTINTributavel.ToString() : ""),
                                    Tags = (produto.Tags != null ? produto.Tags.ToString() : ""),
                                    UnidadeMedidaId = (produto.UnidadeMedidaId != null ? produto.UnidadeMedidaId.ToString() : ""),
                                    CriadoEm = (produto.CriadoEm != null ? produto.CriadoEm.ToString() : ""),
                                    CFOP = (produto.CFOP != null ? produto.CFOP.ToString() : ""),
                                    Impostos = Impostos
                                };
                                i++;
                            }
                            response = new { error = false, produtos = aProdutos };
                        }
                        else {
                            response = new { error = true, msg = url + " - FiscalFlow Error: " + res.errors.ToString() };
                        }
                    }
                    else {
                        response = new { error = true, msg = url + " - PosStone Error: Usuario nao esta logado: "};
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

        public dynamic ProdutoByStoneCodeAndProdutoId(ProdutoRequest produtoRequest) {
            object response = new { error = false, produto = new {
                Id = "",
                Codigo = "",
                Nome = "",
                NomeCurto = "",
                NCM = "",
                GTIN = "",
                GTINTributavel = "",
                Tags = "",
                UnidadeMedidaId = "",
                CriadoEm = "",
            }};

            StoneCodeRequest stoneCodeRequest = new StoneCodeRequest();
            stoneCodeRequest.stone_code = produtoRequest.stone_code;
            var res = ProdutoGetByStoneCode(stoneCodeRequest);
            if (res.error == false) {
                foreach (var produto in res.produtos) {
                    if (produto.Id == produtoRequest.produto_id) {

                        response = new { error = false, produto = new {
                            Id = (produto.Id != null ? produto.Id.ToString() : ""),
                            Codigo = (produto.Codigo != null ? produto.Codigo.ToString() : ""),
                            Nome = (produto.Nome != null ? produto.Nome.ToString() : ""),
                            NomeCurto = (produto.NomeCurto != null ? produto.NomeCurto.ToString() : ""),
                            Preco = produto.Preco,
                            NCM = (produto.NCM != null ? produto.NCM.ToString() : ""),
                            GTIN = (produto.GTIN != null ? produto.GTIN.ToString() : ""),
                            GTINTributavel = (produto.GTINTributavel != null ? produto.GTINTributavel.ToString() : ""),
                            Tags = (produto.Tags != null ? produto.Tags.ToString() : ""),
                            UnidadeMedidaId = (produto.UnidadeMedidaId != null ? produto.UnidadeMedidaId.ToString() : ""),
                            CriadoEm = (produto.CriadoEm != null ? produto.CriadoEm.ToString() : ""),
                            CFOP = (produto.CFOP != null ? produto.CFOP.ToString() : ""),
                            Impostos = new {
                                PIS_CST = produto.Impostos.PIS_CST, 
                                COFINS_CST = produto.Impostos.COFINS_CST,  
                                PIS_Aliquota = produto.Impostos.PIS_Aliquota,  
                                COFINS_Aliquota = produto.Impostos.COFINS_Aliquota,  
                                IPI_Aliquota = produto.Impostos.IPI_Aliquota,  
                                ICMS_Aliquota = produto.Impostos.ICMS_Aliquota, 
                                ICMS_CST_CSOSN = produto.Impostos.ICMS_CST_CSOSN, 
                                ICMS_ReducaoBc = produto.Impostos.ICMS_ReducaoBc, 
                                ICMS_ST_ReducaoBc = produto.Impostos.ICMS_ST_ReducaoBc, 
                                ICMS_ST_Aliquota = produto.Impostos.ICMS_ST_Aliquota, 
                                ICMS_CodigoBeneficio = produto.Impostos.ICMS_CodigoBeneficio, 
                                ICMS_DiffAliquota = produto.Impostos.ICMS_DiffAliquota, 
                                ICMS_AmparoLegal = produto.Impostos.ICMS_AmparoLegal, 
                            }
                        }};
                        break;
                    }
                }
            }
            else {
                response = new { error = true, msg = res.msg };
            }

            return response;
        }

        
    }
}