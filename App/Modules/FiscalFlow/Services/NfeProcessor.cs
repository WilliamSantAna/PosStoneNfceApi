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
using PosStoneNfce.API.Portal.App.Common.Extensions;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services
{
    public class NfeProcessor : AuthProcessor
    {
        public NfeProcessor(IDBRepository<PosStone> dbRepository): base(dbRepository) {
            _dbRepository = dbRepository;
        }

        private dynamic GetPagNFCeList(NfeGenerateRequest nfeGenerateRequest) {
            object response = new {};
            try {
                List<NfeCardNFCeListDataToClientRequest> CardNFCeList = new List<NfeCardNFCeListDataToClientRequest>();
                NfeCardNFCeListDataToClientRequest cardNFCe = new NfeCardNFCeListDataToClientRequest();
                cardNFCe.CNPJx = nfeGenerateRequest.cnpj_cc;
                cardNFCe.tBand = (nfeGenerateRequest.bandeira != null ? Int32.Parse(nfeGenerateRequest.bandeira) : 0);
                cardNFCe.cAut = nfeGenerateRequest.cod_pgto;
                cardNFCe.tpIntegra = ApiConfig.Configuration["ApiDefinitions:TP_INTEGRA"];
                CardNFCeList.Add(cardNFCe);


                List<NfePagNFCeListDataToClientRequest> PagNFCeList = new List<NfePagNFCeListDataToClientRequest>();
                NfePagNFCeListDataToClientRequest PagNFCe = new NfePagNFCeListDataToClientRequest();
                PagNFCe.tPag = Int32.Parse(nfeGenerateRequest.tipo_pagamento);
                PagNFCe.vPag = Double.Parse(nfeGenerateRequest.valor_pagamento);
                PagNFCe.CardNFCeList = CardNFCeList;
                PagNFCeList.Add(PagNFCe);

                response = new { error = false, PagNFCeList = PagNFCeList };
            }
            catch (Exception e) {
                response = new { error = true, msg = "GetPagNFCeList - PosStone Error: Nfe Exception: " + e.ToString()};
            }

            return response;
        }

        private dynamic GetItemNFCeList(NfeGenerateRequest nfeGenerateRequest) {
            object response = new {};
            bool hasErrors = false;
            try {
                List<NfeItemNFCeListDataToClientRequest> ItemNFCeList = new List<NfeItemNFCeListDataToClientRequest>();

                int i = 0;
                foreach (NfeGenerateItemsRequest item in nfeGenerateRequest.items) {
                    ProdutoProcessor produtoProcessor = new ProdutoProcessor(_dbRepository);
                    ProdutoRequest produtoRequest = new ProdutoRequest();
                    produtoRequest.stone_code = nfeGenerateRequest.stone_code;
                    produtoRequest.produto_id = item.id;
                    var res = produtoProcessor.ProdutoByStoneCodeAndProdutoId(produtoRequest);
                    if (res.error == false) {
                        if (res.produto.Id.Length > 0) {
                            var produtoEmpresa = res.produto;

                            string cEAN = produtoEmpresa.GTINTributavel;
                            string cEANTrib = produtoEmpresa.GTINTributavel;
                            string NCM = produtoEmpresa.NCM;
                            if (ApiConfig.Configuration["ApiDefinitions:EAN_TESTE"].ToString().Length > 0) {
                                cEAN = ApiConfig.Configuration["ApiDefinitions:EAN_TESTE"];
                                cEANTrib = ApiConfig.Configuration["ApiDefinitions:EAN_TESTE"];
                            }
                            if (ApiConfig.Configuration["ApiDefinitions:NCM_TESTE"].ToString().Length > 0) {
                                NCM = ApiConfig.Configuration["ApiDefinitions:NCM_TESTE"];
                            }
                
                            string uCom = produtoEmpresa.UnidadeMedidaId;
                            string uTrib = produtoEmpresa.UnidadeMedidaId;
                            string xProd = produtoEmpresa.Nome;
                            string CFOP = produtoEmpresa.CFOP;
                            
                            double pICMS = produtoEmpresa.Impostos.ICMS_Aliquota;
                            string CST_CSOSN = produtoEmpresa.Impostos.ICMS_CST_CSOSN;
                            string CST_PIS = produtoEmpresa.Impostos.PIS_CST; 
                            string CST_COF = produtoEmpresa.Impostos.COFINS_CST;
                
                            // Calculados para Nfce
                            int nItem = i;
                            int cProd = i;
                            double qCom = Double.Parse(item.qty);
                            double qTrib = Double.Parse(item.qty);
                            double vUnCom = Double.Parse(item.vlr);
                            double vProd = qCom * vUnCom;
                            double vBC = vProd;
                            double vICMS = (vBC * (pICMS / 100));

                            NfeItemNFCeListDataToClientRequest Item = new NfeItemNFCeListDataToClientRequest();
                            Item.nItem = nItem;
                            Item.cProd = cProd.ToString();
                            Item.xProd = xProd.ToString().ToUpper();
                            Item.cEAN = cEAN.ToString();
                            Item.cEANTrib = cEANTrib.ToString();
                            Item.NCM = NCM.ToString();
                            Item.CFOP = Int32.Parse(CFOP);
                            Item.uCom = uCom;
                            Item.uTrib = uTrib;
                            Item.qCom = qCom;
                            Item.qTrib = qTrib;
                            Item.vUnCom = vUnCom;
                            Item.vProd = vProd;
                            Item.orig = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:ORIG"]);
                            Item.modBC = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:MOD_BC"]);
                            Item.vBC = vBC;
                            Item.pICMS = pICMS;
                            Item.vICMS = vICMS;
                            Item.CST_CSOSN = CST_CSOSN;
                            Item.CST_PIS = CST_PIS;
                            Item.CST_COF = CST_COF;

                            ItemNFCeList.Add(Item);

                            i++;
                        }
                        else {
                            hasErrors = true;
                            response = new { error = true, msg = $"GetItemNFCeList - PosStone Error Foreach: Nao encontrou produto com esse id = {item.id}"};
                            break;
                        }
                    }
                    else {
                        hasErrors = true;
                        response = new { error = true, msg = res.msg};
                        break;
                    }
                }

                if (!hasErrors) {
                    response = new { error = false, ItemNFCeList = ItemNFCeList };
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "GetItemNFCeList - PosStone Error: Nfe Exception: " + e.ToString()};
            }

            return response;
        }


        private dynamic GetNfeDataToFF(NfeGenerateRequest nfeGenerateRequest) {
            object response = new {};
            try {
                var res = GetItemNFCeList(nfeGenerateRequest);
                if (res.error == false) {
                    List<NfeItemNFCeListDataToClientRequest> ItemNFCeList = res.ItemNFCeList;
                    res = GetPagNFCeList(nfeGenerateRequest);
                    if (res.error == false) {
                        List<NfePagNFCeListDataToClientRequest> PagNFCeList = res.PagNFCeList;

                        StoneCodeRequest stoneCodeRequest = new StoneCodeRequest();
                        stoneCodeRequest.stone_code = nfeGenerateRequest.stone_code;
                        var resSerie = GetSerieNfceByStoneCode(stoneCodeRequest);
                        if (resSerie.error == false) {
                            string serie = resSerie.serie_nfe.ToString();
                            var resNumNfe = GetNextNumNfceByStoneCode(stoneCodeRequest, serie);
                            if (resNumNfe.error == false) {
                                int nNF = resNumNfe.num_nfe;
                                string cnpj = GetCnpjNfe(nfeGenerateRequest);
                                string dhEmi = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");

                                double vBC = 0;
                                double vICMS = 0;
                                double vProd = 0;
                                double vNF = 0;
                                double vIPI = 0;
                                double vPIS = 0;
                                double vCOFINS = 0;
                                double vBCST = 0;
                                double vST = 0;
                        
                                foreach (NfeItemNFCeListDataToClientRequest ItemNFCe in ItemNFCeList) {
                                    vBC += ItemNFCe.vBC;
                                    vICMS += ItemNFCe.vICMS;
                                    vProd += ItemNFCe.vProd;
                                    vNF += ItemNFCe.vProd;
                                    vIPI += 0;
                                    vPIS += 0;
                                    vCOFINS += 0;
                                    vBCST += 0;
                                    vST += 0;
                                }


                                NfeDataToClientRequest nfeDataToClientRequest = new NfeDataToClientRequest();
                                nfeDataToClientRequest.GeraDanfe = Boolean.Parse(ApiConfig.Configuration["ApiDefinitions:GERA_DANFE"]);
                                nfeDataToClientRequest.GeraDanfe56mm = Boolean.Parse(ApiConfig.Configuration["ApiDefinitions:GERA_DANFE_56MM"]);
                                nfeDataToClientRequest.ExtensaoDanfe = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:EXTENSAO_DANFE"]);
                                nfeDataToClientRequest.DevolveXml = Boolean.Parse(ApiConfig.Configuration["ApiDefinitions:DEVOLVE_XML"]);
                                nfeDataToClientRequest.tipoemissao = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TIPO_EMISSAO"]);
                                nfeDataToClientRequest.versao = ApiConfig.Configuration["ApiDefinitions:VERSAO"];
                                nfeDataToClientRequest.natOp = ApiConfig.Configuration["ApiDefinitions:NAT_OP"];
                                nfeDataToClientRequest.serie = serie;
                                nfeDataToClientRequest.nNF = nNF;
                                nfeDataToClientRequest.dhEmi = dhEmi;
                                nfeDataToClientRequest.tpNF = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TP_NF"]);
                                nfeDataToClientRequest.idDest = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:ID_DEST"]);
                                nfeDataToClientRequest.tpImp = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TP_IMP"]);
                                nfeDataToClientRequest.tpEmis = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TP_EMIS"]);
                                nfeDataToClientRequest.finNFe = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:FIN_NFE"]);
                                nfeDataToClientRequest.tpAmb = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:TP_AMB"]);
                                nfeDataToClientRequest.IndFinal = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:IND_FINAL"]);
                                nfeDataToClientRequest.IndPres = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:IND_PRES"]);
                                nfeDataToClientRequest.CNPJ = cnpj;
                                nfeDataToClientRequest.CRT = ApiConfig.Configuration["ApiDefinitions:CRT_CLIENT"];
                                nfeDataToClientRequest.TIPOIDCONS = (nfeGenerateRequest.cpf_cnpj_destinatario != null ? (nfeGenerateRequest.cpf_cnpj_destinatario.Length == 11 ? "1" : "2") : "1");
                                nfeDataToClientRequest.IDCONS = (nfeGenerateRequest.cpf_cnpj_destinatario != null ? nfeGenerateRequest.cpf_cnpj_destinatario : "");
                                nfeDataToClientRequest.xNome = (ApiConfig.Configuration["ApiDefinitions:X_NOME"] != null ? ApiConfig.Configuration["ApiDefinitions:X_NOME"] : "");
                                nfeDataToClientRequest.indIEDest = ApiConfig.Configuration["ApiDefinitions:IND_IE_DEST"];
                                nfeDataToClientRequest.vProd = vProd;
                                nfeDataToClientRequest.vNF = vNF;
                                nfeDataToClientRequest.vIPI = vIPI;
                                nfeDataToClientRequest.vPIS = vPIS;
                                nfeDataToClientRequest.vCOFINS = vCOFINS;
                                nfeDataToClientRequest.vBCST = vBCST;
                                nfeDataToClientRequest.vST = vST;
                                nfeDataToClientRequest.idProduto = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:ID_PRODUTO"]);
                                nfeDataToClientRequest.ItemNFCeList = ItemNFCeList;
                                nfeDataToClientRequest.PagNFCeList = PagNFCeList;

                                // if (nfeGenerateRequest.cpf_cnpj_destinatario != null) {
                                //     DataToFF["TIPOIDCONS"] = (nfeGenerateRequest.cpf_cnpj_destinatario.Length == 11 ? "1" : "2");
                                //     DataToFF["IDCONS"] = nfeGenerateRequest.cpf_cnpj_destinatario;
                                // }

                                response = new { error = false, DataToFF = nfeDataToClientRequest };

                            }
                            else {
                                response = new { error = true, msg = resNumNfe.msg };
                            }
                        }
                        else {
                            response = new { error = true, msg = resSerie.msg };
                        }
                    }
                    else {
                        response = new { error = true, msg = res.msg };
                    }
                }
                else {
                    response = new { error = true, msg = res.msg };
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "GetNfeDataToFF - PosStone Error - Nfe Exception: " + e.ToString()};
            }

            return response;
        }

        public dynamic NfeGenerate(NfeGenerateRequest nfeGenerateRequest) {
            dynamic response = new object();
            string url = ApiConfig.Configuration["ApiDefinitions:CLIENT_20_SERVER"] + "/NFCe/Autoriza";
            dynamic jsonDaNota = new {};
            try {
                var res = GetNfeDataToFF(nfeGenerateRequest);
                if (res.error == false) {
                    jsonDaNota = res.DataToFF;
                    NfeDataToClientRequest nfeDataToClientRequest = jsonDaNota;
                    string payload = CurlService.GetPayload(url, nfeDataToClientRequest, null);
                    var httpResponse = CurlService.Post(url, nfeDataToClientRequest, null);
                    res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                    if (ObjectExtensions.PropertyExist(res, "cStat")) {
                        bool HasErrors = false;
                        string errMsg = "";

                        string chNfe = (res.chNFe != null ? res.chNFe.ToString() : "");
                        string cStat = (res.cStat != null ? res.cStat.ToString() : "");
                        string xMotivo = (res.xMotivo != null ? res.xMotivo.ToString() : "");
                        string idNota = (res.idNota != null ? res.idNota.ToString() : "");
                        string xUrlDanfe = (res.xUrlDanfe != null ? res.xUrlDanfe.ToString() : "");
                        string Danfe = (res.Danfe != null ? res.Danfe.ToString() : "");
                        int CentralContingencia = (res.Central_Contingencia != null ? (res.Central_Contingencia.ToString() == "true" ? 1 : 0) : 0);
                        int DataAlteracao = (res.DataAlteracao != null ? DateExtensions.DateToTimestamp(res.DataAlteracao.ToString()) : 0);
                        int DataEmissao = DateExtensions.GetTimestampNow();

                        // Salvar a nfe no banco
                        NfeEntity nfeEntity = new NfeEntity();
                        nfeEntity.stone_code = Int32.Parse(nfeGenerateRequest.stone_code);
                        nfeEntity.chave_nfe = chNfe;
                        nfeEntity.status = cStat;
                        nfeEntity.serie_nfe = nfeDataToClientRequest.serie;
                        nfeEntity.num_nfe = nfeDataToClientRequest.nNF;
                        nfeEntity.client_id_nota = Int32.Parse(idNota);
                        nfeEntity.motivo = xMotivo;
                        nfeEntity.contingencia = CentralContingencia;
                        nfeEntity.dh_emissao = DataEmissao;
                        nfeEntity.dh_retorno = DataAlteracao;
                        nfeEntity.tipo_pagamento = Int32.Parse(nfeGenerateRequest.tipo_pagamento);
                        nfeEntity.valor_pagamento = Double.Parse(nfeGenerateRequest.valor_pagamento);
                        nfeEntity.cpf_cnpj_destinatario = (nfeGenerateRequest.cpf_cnpj_destinatario != null ? nfeGenerateRequest.cpf_cnpj_destinatario : "");
                        nfeEntity.cnpj_cc = (nfeGenerateRequest.cnpj_cc != null ? nfeGenerateRequest.cnpj_cc : "");
                        nfeEntity.bandeira = (nfeGenerateRequest.bandeira != null ? nfeGenerateRequest.bandeira : "0");
                        nfeEntity.cod_pgto = (nfeGenerateRequest.cod_pgto != null ? nfeGenerateRequest.cod_pgto : "");
                        nfeEntity.url_danfe = xUrlDanfe;
                        var saved = _dbRepository.Save(nfeEntity).Result;
                        if (saved.error == false) {
                            int nfeId = saved.pk;
                            foreach (var itemNota in nfeGenerateRequest.items) {
                                NfeItensEntity nfeItensEntity = new NfeItensEntity();
                                nfeItensEntity.nfe_id = nfeId;
                                nfeItensEntity.produto_id = itemNota.id;
                                nfeItensEntity.qty = Double.Parse(itemNota.qty);
                                nfeItensEntity.vlr = Double.Parse(itemNota.vlr);
                                saved = _dbRepository.Save(nfeItensEntity).Result;
                                if (saved.error == true) {
                                    HasErrors = true;
                                    errMsg = $"NfeGenerate - PosStone Error: Erro ao salvar o item da NFE no banco: Item = {itemNota.id} - " + saved.ToString();
                                }
                            }
                        }
                        else {
                            HasErrors = true;
                            errMsg = "NfeGenerate - PosStone Error: Erro ao salvar a NFE no banco: " + saved.ToString();
                        }


                        if (!HasErrors) {
                            if (res.cStat == "100" || res.cStat == "MC015") {
                                var fitImage = ImageExtension.FitImage(Danfe, 400, 1000);

                                response = new { 
                                    error = false, 
                                    nfe = new {
                                        chNfe = chNfe,
                                        cStat = cStat,
                                        xMotivo = xMotivo,
                                        idNota = idNota,
                                        xUrlDanfe = xUrlDanfe,
                                        Danfe = fitImage
                                    }
                                };
                            }
                            else {
                                response = new { error = true, msg = " - NfeGenerate - Client2 Error: Erro ao gerar Nfe: " + res.xMotivo + " ---- " + ObjectExtensions.ToJSON(jsonDaNota)};
                            }
                        }
                        else {
                            response = new { error = true, msg = errMsg};
                        }
                    }
                    else {
                        response = new { error = true, msg = " - NfeGenerate - Client Comunication Error: " + res.ToString() + " --- " + httpResponse + " ---- " + ObjectExtensions.ToJSON(jsonDaNota)};
                    }
                }
                else {
                    response = new { error = true, msg = res.msg + " ---- " + ObjectExtensions.ToJSON(jsonDaNota) };
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - NfeGenerate - PosStone Error: Nfe Exception: " + e.ToString() + " ---- " + ObjectExtensions.ToJSON(jsonDaNota)};
            }

            return response;

        }

        public dynamic GetSerieNfceByStoneCode(StoneCodeRequest stoneCodeRequest) {
            object response = new {};
            try {
                string serie_nfe = ApiConfig.Configuration["ApiDefinitions:DEFAULT_SERIE_NFE"];
                // Load serie from company database config
                EmpresaEntity empresaEntity = new EmpresaEntity();
                empresaEntity.stone_code = Int32.Parse(stoneCodeRequest.stone_code);
                DataTable dt = _dbRepository.FindOneBy<EmpresaEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    int s = Int32.Parse(dt.Rows[0]["serie_nfe"].ToString());
                    if (s > 0) {
                        serie_nfe = s.ToString();
                    }
                }

                response = new { error = false, serie_nfe = serie_nfe };
            }
            catch (Exception e) {
                response = new { error = true, msg = "GetSerieNfceByStoneCode - PosStone Error: Nfe Exception: " + e.ToString()};
            }

            return response;
        }

        public dynamic GetNextNumNfceByStoneCode(StoneCodeRequest stoneCodeRequest, string serie_nfe) {
            object response = new {};
            try {
                int num_nfe = Int32.Parse(ApiConfig.Configuration["ApiDefinitions:DEFAULT_NUM_INI_NFE"]);
                // Load serie from company database config
                int stone_code = Int32.Parse(stoneCodeRequest.stone_code);
                string query = $"select max(num_nfe) as num_nfe from nfe where stone_code = {stone_code} and serie_nfe = {serie_nfe}";
                DataTable dt = _dbRepository.Query(query).Result;
                if (dt.Rows.Count > 0) {
                    if (dt.Rows[0]["num_nfe"].ToString().Length > 0) {
                        int n = Int32.Parse(dt.Rows[0]["num_nfe"].ToString());
                        if (n > 0) {
                            num_nfe = n + 1;
                        }
                    }
                }

                response = new { error = false, num_nfe = num_nfe };
            }
            catch (Exception e) {
                response = new { error = true, msg = "GetNextNumNfceByStoneCode - PosStone Error: Nfe Exception: " + e.ToString()};
            }

            return response;
        }

        private string GetCnpjNfe(NfeGenerateRequest nfeGenerateRequest) {
            string cnpj = ApiConfig.Configuration["ApiDefinitions:CNPJ_NFE_TESTE"];
            if (Boolean.Parse(ApiConfig.Configuration["ApiDefinitions:PRODUCTION"]) == true) {
                EmpresaEntity empresaEntity = new EmpresaEntity();
                empresaEntity.stone_code = Int32.Parse(nfeGenerateRequest.stone_code);
                DataTable dt = _dbRepository.FindOneBy<EmpresaEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    cnpj = dt.Rows[0]["cnpj"].ToString();
                }
            }
            return cnpj;
        }

        public dynamic SetNfeConfigRequest(SetNfeConfigRequest setNfeConfigRequest) {
            object response = new {};
            string serie_nfe = ApiConfig.Configuration["ApiDefinitions:DEFAULT_SERIE_NFE"];
            string num_nfe = ApiConfig.Configuration["ApiDefinitions:DEFAULT_NUM_INI_NFE"];
            try 
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();
                empresaEntity.stone_code = Int32.Parse(setNfeConfigRequest.stone_code);
                DataTable dt = _dbRepository.FindOneBy<EmpresaEntity>(empresaEntity.GetTableName(), empresaEntity.GetPkName(), empresaEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    if (dt.Rows[0]["serie_nfe"] != null) {
                        empresaEntity.serie_nfe = Int32.Parse(dt.Rows[0]["serie_nfe"].ToString());
                    }
                    if (dt.Rows[0]["num_nfe"] != null) {
                        empresaEntity.num_nfe = Int32.Parse(dt.Rows[0]["num_nfe"].ToString());
                    }
                    if (setNfeConfigRequest.serie_nfe != null) {
                        serie_nfe = setNfeConfigRequest.serie_nfe.ToString();
                    }
                    if (setNfeConfigRequest.num_nfe != null) {
                        num_nfe = setNfeConfigRequest.num_nfe.ToString();
                    }

                    empresaEntity.serie_nfe = Int32.Parse(serie_nfe);
                    empresaEntity.num_nfe = Int32.Parse(num_nfe);
                    empresaEntity.aceite_termos_uso = Int32.Parse(dt.Rows[0]["aceite_termos_uso"].ToString());
                    empresaEntity.cnpj = dt.Rows[0]["cnpj"].ToString();

                    var saved = _dbRepository.Save(empresaEntity).Result;
                    if (saved.error == false) {
                        response = new { error = false, serie_nfe = setNfeConfigRequest.serie_nfe, num_nfe = setNfeConfigRequest.num_nfe  };
                    }
                    else {
                        response = new { error = true, msg = saved.msg };
                    }
                }
                else {
                    response = new { error = true, msg = "SetNfeConfigRequest - PosStone Error: Empresa nao existe na api"};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = "SetNfeConfigRequest - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;

        }

        
    }
}