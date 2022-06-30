using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.App.Common.Controller;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models;
using FiscalFlow.Common.Services.Cryptography;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request;
using PosStoneNfce.API.Portal.App.Common.Utils;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using PosStoneNfce.API.Portal.Configuration;


namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Controller
{
    [ApiController]
    public class EmpresaController : MainController
    {
        private readonly IServiceEmpresa<ServiceEmpresa, EmpresaEntity> _serviceEmpresa;

        public EmpresaController(IServiceEmpresa<ServiceEmpresa, EmpresaEntity> serviceEmpresa, ILogger logger)
        {
            _serviceEmpresa = serviceEmpresa;
        }

        /*
        [HttpGet]
        [Route("Api/[controller]/ListEmpresa")]
        public async Task<IActionResult> ListEmpresa()
        {
            var lstEmpresas = await _serviceEmpresa.ListEmpresaAsync(null);

            return new ObjectResult(lstEmpresas);
        }
        */

        /// <summary>
        /// Insere ou Edita uma empresa (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Empresa/Save
        /// {
        ///     "data": {
        ///         "stone_code": 99999,
        ///         "razao_social": "Everest Inteligencia de Mercado Ltda",
        ///         "cnpj": "02.151.247/0001-85",
        ///         "email": "contato@everestim.com.br",
        ///         "telefone": "(31) 9 8501-3019",
        ///         "endereco": "Rua Passos",
        ///         "numero": "19",
        ///         "bairro": "Canadá",
        ///         "municipio": "Contagem",
        ///         "cod_ibge_municipio": "1234568",
        ///         "uf": "MG",
        ///         "cep": "32015-030",
        ///         "ie": null,
        ///         "id_csc": 2,
        ///         "cod_csc": "a0eadf22-e86b-4b23-a843-a6ecf1267930"
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "Empresa salva com sucesso","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/Save")]
        public IActionResult Save([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
                EmpresaSaveRequest empresaSaveRequest = ObjectExtensions.GetDataObjectFromJsonPost<EmpresaSaveRequest>(post);
                EmpresaProcessor processor = new EmpresaProcessor(_dbRepository);
                var res = processor.EmpresaSave(empresaSaveRequest);
                if (res.error == false) {
                    response = new { };
                }
                else {
                    AddProcessingError(res.msg);
                }
            }
            catch (Exception e) {
                AddProcessingError(e.ToString());
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem os dados da empresa pelo stone_code (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Empresa/GetByStoneCode
        /// {
        ///     "data": {
        ///         "stone_code": 99999
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetByStoneCode")]
        public IActionResult GetByStoneCode([FromBody] JsonElement post) 
        {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
                StoneCodeRequest stoneCodeRequest = ObjectExtensions.GetDataObjectFromJsonPost<StoneCodeRequest>(post);
                EmpresaProcessor processor = new EmpresaProcessor(_dbRepository);
                var res = processor.EmpresaGetByStoneCode(stoneCodeRequest);
                if (res.error == false) {
                    response = new { 
                        IdMidPfj = res.empresa.IdMidPfj,
                        NomeFantasiaApelido = res.empresa.NomeFantasiaApelido,
                        CnpjCpf = res.empresa.CnpjCpf,
                        RazaoSocialNomeCompleto = res.empresa.RazaoSocialNomeCompleto,
                        IdGpecon = res.empresa.IdGpecon,
                    };
                }
                else {
                    AddProcessingError(res.msg);
                }
            }
            catch (Exception e) {
                AddProcessingError(e.ToString());
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Seta o aceite dos termos de uso de uma empresa (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Empresa/SetAceiteTermosUso
        /// {
        ///     "data": {
        ///         "stone_code": 99999
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/SetAceiteTermosUso")]
        public IActionResult SetAceiteTermosUso([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
                SetAceiteTermosUsoRequest setAceiteTermosUsoRequest = ObjectExtensions.GetDataObjectFromJsonPost<SetAceiteTermosUsoRequest>(post);
                EmpresaProcessor processor = new EmpresaProcessor(_dbRepository);
                var res = processor.SetAceiteTermosUso(setAceiteTermosUsoRequest);
                if (res.error == false) {
                    response = new { };
                }
                else {
                    AddProcessingError(res.msg);
                }
            }
            catch (Exception e) {
                AddProcessingError(e.ToString());
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem o aceite dos termos de uso de uma empresa (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Empresa/GetAceiteTermosUso
        /// {
        ///     "data": {
        ///         "stone_code": 99999
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetAceiteTermosUso")]
        public IActionResult GetAceiteTermosUso([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
            StoneCodeRequest stoneCodeRequest = ObjectExtensions.GetDataObjectFromJsonPost<StoneCodeRequest>(post);
            EmpresaProcessor processor = new EmpresaProcessor(_dbRepository);
            var res = processor.GetAceiteTermosUso(stoneCodeRequest);
            if (res.error == false) {
                response = new { aceite_termos_uso = res.aceite_termos_uso };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }


        /// <summary>
        /// Adiciona um produto a uma empresa (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Empresa/AddProduto
        /// {
        ///     "data": {
        ///         "stone_code": 99999
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/AddProduto")]
        public IActionResult AddProduto([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
                ProdutoRequest produtoRequest = ObjectExtensions.GetDataObjectFromJsonPost<ProdutoRequest>(post);
                EmpresaProcessor processor = new EmpresaProcessor(_dbRepository);
                var res = processor.EmpresaAddProduto(produtoRequest);
                if (res.error == false) {
                    response = new { };
                }
                else {
                    AddProcessingError(res.msg);
                }
            }
            catch (Exception e) {
                AddProcessingError(e.ToString());
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Remove um produto de uma empresa (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Empresa/RemoveProduto
        /// {
        ///     "data": {
        ///         "stone_code": 99999
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/RemoveProduto")]
        public IActionResult RemoveProduto([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
                ProdutoRequest produtoRequest = ObjectExtensions.GetDataObjectFromJsonPost<ProdutoRequest>(post);
                EmpresaProcessor processor = new EmpresaProcessor(_dbRepository);
                var res = processor.EmpresaRemoveProduto(produtoRequest);
                if (res.error == false) {
                    response = new { };
                }
                else {
                    AddProcessingError(res.msg);
                }
            }
            catch (Exception e) {
                AddProcessingError(e.ToString());
            }

            return CustomResponse(response);
        }



    }
}