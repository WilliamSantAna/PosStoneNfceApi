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
using PosStoneNfce.API.Portal.App.Modules.Autenticacao.Services;




namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Controller
{
    [ApiController]
    public class NfeController : MainController
    {

        private readonly IServiceNfe<ServiceNfe, NfeEntity> _serviceNfe;

        public NfeController(IServiceNfe<ServiceNfe, NfeEntity> serviceNfe, ILogger logger)
        {
            _serviceNfe = serviceNfe;
        }
        /// <summary>
        /// Gera uma NFe (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Nfe/Generate
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
        [Route("Api/[controller]/Generate")]
        public IActionResult Generate([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceNfe.GetDBRepository();
            NfeProcessor processor = new NfeProcessor(_dbRepository);
            NfeGenerateRequest nfeGenerateRequest = ObjectExtensions.GetDataObjectFromJsonPost<NfeGenerateRequest>(post);
            var res = processor.NfeGenerate(nfeGenerateRequest);
            if (res.error == false) {
                response = new { nfe = res.nfe };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Obtem uma danfe (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Nfe/GetDanfe
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
        [Route("Api/[controller]/GetDanfe")]
        public IActionResult GetDanfe([FromBody] JsonElement post) {
            return Success(post);
        }

        /// <summary>
        /// Seta a configuracao de serie e numero da nfe (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Nfe/SetConfig
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
        [Route("Api/[controller]/SetConfig")]
        public IActionResult SetConfig([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceNfe.GetDBRepository();
                SetNfeConfigRequest setNfeConfigRequest = ObjectExtensions.GetDataObjectFromJsonPost<SetNfeConfigRequest>(post);
                NfeProcessor processor = new NfeProcessor(_dbRepository);
                var res = processor.SetNfeConfigRequest(setNfeConfigRequest);
                if (res.error == false) {
                    response = new { serie_nfe = res.serie_nfe, num_nfe = res.num_nfe };
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