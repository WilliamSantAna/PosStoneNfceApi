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
    public class GeneralController : MainController
    {

        private readonly IServiceGeneral<ServiceGeneral, GeneralEntity> _serviceGeneral;

        public GeneralController(IServiceGeneral<ServiceGeneral, GeneralEntity> serviceGeneral, ILogger logger)
        {
            _serviceGeneral = serviceGeneral;
        }

        /// <summary>
        /// Obtem a versao da api (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// GET /api/General/GetApiVersion
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetApiVersion")]
        public IActionResult GetApiVersion() {
            string version = ApiConfig.Configuration["API_VERSION"];
            object response = new object();

            try {
                response = new { version };
            }
            catch (Exception e) {
                AddProcessingError(e.ToString());
            }

            return CustomResponse(response);
        }


        /// <summary>
        /// Obtem o texto da politica de privacidade (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// GET /api/General/GetPoliticaPrivacidade
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetPoliticaPrivacidade")]
        public IActionResult GetPoliticaPrivacidade() {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
            GeneralProcessor processor = new GeneralProcessor(_dbRepository);
            var res = processor.GetPoliticaPrivacidade();
            if (res.error == false) {
                response = new { politicaPrivacidade = res.politica_privacidade };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem o texto dos termos de uso (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// GET /api/General/GetTermosUso
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetTermosUso")]
        public IActionResult GetTermosUso() {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
            GeneralProcessor processor = new GeneralProcessor(_dbRepository);
            var res = processor.GetTermosUso();
            if (res.error == false) {
                response = new { termosUso = res.termos_uso };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }


        /// <summary>
        /// Obtem as UFs disponiveis no FiscalFlow (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// GET /api/General/GetUfs
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetUfs")]
        public IActionResult GetUfs() {

            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
            GeneralProcessor processor = new GeneralProcessor(_dbRepository);
            var res = processor.GetUfs();
            if (res.error == false) {
                response = new { ufs = res.ufs };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem uma UF pela Sigla (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/General/GetUfBySigla
        /// {
        ///     "data": {
        ///         "sigla": "RS"
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetUfBySigla")]
        public IActionResult GetUfBySigla([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
                SiglaRequest siglaRequest = ObjectExtensions.GetDataObjectFromJsonPost<SiglaRequest>(post);
                GeneralProcessor processor = new GeneralProcessor(_dbRepository);
                var res = processor.GetUfBySigla(siglaRequest);
                if (res.error == false) {
                    response = new { 
                        CodigoIbge = res.uf.CodigoIbge, 
                        Sigla = res.uf.Sigla, 
                        Nome = res.uf.Nome 
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
        /// Obtem uma UF pela Sigla (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/General/GetCidadesByUf
        /// {
        ///     "data": {
        ///         "sigla": "RS"
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetCidadesByUf")]
        public IActionResult GetCidadesByUf([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
                SiglaRequest siglaRequest = ObjectExtensions.GetDataObjectFromJsonPost<SiglaRequest>(post);
                GeneralProcessor processor = new GeneralProcessor(_dbRepository);
                var res = processor.GetCidadesByUf(siglaRequest);
                if (res.error == false) {
                    response = new { cidades = res.cids };
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
        /// Obtem o raw data (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/General/GetRawData
        /// {
        ///     "data": {
        ///         "raw_instruction": ""
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/GetRawData")]
        public IActionResult GetRawData([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
                SqlRequest sqlRequest = ObjectExtensions.GetDataObjectFromJsonPost<SqlRequest>(post);
                GeneralProcessor processor = new GeneralProcessor(_dbRepository);
                var res = processor.GetRawData(sqlRequest);
                if (res.error == false) {
                    response = new { result = res.result };
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
        /// Obtem o raw data (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/General/ExecuteRawData
        /// {
        ///     "data": {
        ///         "raw_instruction": ""
        ///     }
        /// }        
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/ExecuteRawData")]
        public IActionResult ExecuteRawData([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceGeneral.GetDBRepository();
                SqlRequest sqlRequest = ObjectExtensions.GetDataObjectFromJsonPost<SqlRequest>(post);
                GeneralProcessor processor = new GeneralProcessor(_dbRepository);
                var res = processor.ExecuteRawData(sqlRequest);
                if (res.error == false) {
                    response = new { result = res.result };
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