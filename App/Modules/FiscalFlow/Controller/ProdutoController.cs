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
    public class ProdutoController : MainController
    {

        private readonly IServiceEmpresa<ServiceEmpresa, EmpresaEntity> _serviceEmpresa;

        public ProdutoController(IServiceEmpresa<ServiceEmpresa, EmpresaEntity> serviceEmpresa, ILogger logger)
        {
            _serviceEmpresa = serviceEmpresa;
        }

        /// <summary>
        /// Obtem todos os produtos disponiveis da plataforma (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Produto/GetAll
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
        [Route("Api/[controller]/GetAll")]
        public IActionResult GetAll([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
            ProdutoProcessor processor = new ProdutoProcessor(_dbRepository);
            StoneCodeRequest stoneCodeRequest = ObjectExtensions.GetDataObjectFromJsonPost<StoneCodeRequest>(post);
            var res = processor.ProdutoGetAll(stoneCodeRequest);
            if (res.error == false) {
                response = new { produtos = res.produtos };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem um produto disponivel da plataforma pelo ID (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Produto/GetById
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
        [Route("Api/[controller]/GetById")]
        public IActionResult GetById([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
            ProdutoProcessor processor = new ProdutoProcessor(_dbRepository);
            ProdutoRequest produtoRequest = ObjectExtensions.GetDataObjectFromJsonPost<ProdutoRequest>(post);
            var res = processor.ProdutoGetById(produtoRequest);
            if (res.error == false) {
                response = new { produto = res.produto };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem os produtos da empresa (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Produto/GetByStoneCode
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
        public IActionResult GetByStoneCode([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
            ProdutoProcessor processor = new ProdutoProcessor(_dbRepository);
            StoneCodeRequest stoneCodeRequest = ObjectExtensions.GetDataObjectFromJsonPost<StoneCodeRequest>(post);
            var res = processor.ProdutoGetByStoneCode(stoneCodeRequest);
            if (res.error == false) {
                response = new { produtos = res.produtos };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem um produto da empresa pelo ID do produto (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Produto/GetByStoneCode
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
        [Route("Api/[controller]/GetByStoneCodeAndProdutoId")]
        public IActionResult GetByStoneCodeAndProdutoId([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceEmpresa.GetDBRepository();
            ProdutoProcessor processor = new ProdutoProcessor(_dbRepository);
            ProdutoRequest produtoRequest = ObjectExtensions.GetDataObjectFromJsonPost<ProdutoRequest>(post);
            var res = processor.ProdutoByStoneCodeAndProdutoId(produtoRequest);
            if (res.error == false) {
                response = new { produto = res.produto };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }



    }
}