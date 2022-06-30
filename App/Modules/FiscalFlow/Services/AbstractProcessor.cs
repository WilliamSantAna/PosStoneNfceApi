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
    public class AbstractProcessor
    {

        public IDBRepository<PosStone> _dbRepository { get; set; }

        public AbstractProcessor(IDBRepository<PosStone> dbRepository) {
            _dbRepository = dbRepository;
        }

        public dynamic GetUfs() 
        {
            object response = new {};
            try {
                object[] ufs = new object[27];
                ufs[0] = new {CodigoIbge = 11, Sigla = "RO", Nome = "Rondônia"};
                ufs[1] = new {CodigoIbge = 12, Sigla = "AC", Nome = "Acre"};
                ufs[2] = new {CodigoIbge = 13, Sigla = "AM", Nome = "Amazonas"};
                ufs[3] = new {CodigoIbge = 14, Sigla = "RR", Nome = "Roraima"};
                ufs[4] = new {CodigoIbge = 15, Sigla = "PA", Nome = "Pará"};
                ufs[5] = new {CodigoIbge = 16, Sigla = "AP", Nome = "Amapá"};
                ufs[6] = new {CodigoIbge = 17, Sigla = "TO", Nome = "Tocantins"};
                ufs[7] = new {CodigoIbge = 21, Sigla = "MA", Nome = "Maranhão"};
                ufs[8] = new {CodigoIbge = 22, Sigla = "PI", Nome = "Piauí"};
                ufs[9] = new {CodigoIbge = 23, Sigla = "CE", Nome = "Ceará"};
                ufs[10] = new {CodigoIbge = 24, Sigla = "RN", Nome = "Rio Grande do Norte"};
                ufs[11] = new {CodigoIbge = 25, Sigla = "PB", Nome = "Paraíba"};
                ufs[12] = new {CodigoIbge = 26, Sigla = "PE", Nome = "Pernambuco"};
                ufs[13] = new {CodigoIbge = 27, Sigla = "AL", Nome = "Alagoas"};
                ufs[14] = new {CodigoIbge = 28, Sigla = "SE", Nome = "Sergipe"};
                ufs[15] = new {CodigoIbge = 29, Sigla = "BA", Nome = "Bahia"};
                ufs[16] = new {CodigoIbge = 31, Sigla = "MG", Nome = "Minas Gerais"};
                ufs[17] = new {CodigoIbge = 32, Sigla = "ES", Nome = "Espírito Santo"};
                ufs[18] = new {CodigoIbge = 33, Sigla = "RJ", Nome = "Rio de Janeiro"};
                ufs[19] = new {CodigoIbge = 35, Sigla = "SP", Nome = "São Paulo"};
                ufs[20] = new {CodigoIbge = 41, Sigla = "PR", Nome = "Paraná"};
                ufs[21] = new {CodigoIbge = 42, Sigla = "SC", Nome = "Santa Catarina"};
                ufs[22] = new {CodigoIbge = 43, Sigla = "RS", Nome = "Rio Grande do Sul"};
                ufs[23] = new {CodigoIbge = 50, Sigla = "MS", Nome = "Mato Grosso do Sul"};
                ufs[24] = new {CodigoIbge = 51, Sigla = "MT", Nome = "Mato Grosso"};
                ufs[25] = new {CodigoIbge = 52, Sigla = "GO", Nome = "Goiás"};
                ufs[26] = new {CodigoIbge = 53, Sigla = "DF", Nome = "Distrito Federal"};

                response = new { error = false, ufs = ufs };
            }
            catch (Exception e) {
                response = new { error = true, msg = " - Exception Error: " + e.ToString()};
            }

            return response;
        }


        public dynamic GetUfBySigla(SiglaRequest siglaRequest) {
            object response = new {};
            try {
                var res = this.GetUfs();
                if (res.error == false) {
                    foreach (var uf in res.ufs) {
                        if (uf.Sigla == siglaRequest.sigla) {
                            response = new { error = false, uf = new {
                                CodigoIbge = uf.CodigoIbge,
                                Sigla = uf.Sigla,
                                Nome = uf.Nome,
                            }};
                            break;
                        }
                    }
                }
                else {
                    response = new { error = true, msg = res.msg};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = " - Exception Error: " + e.ToString()};
            }

            return response;
        }


    }
}