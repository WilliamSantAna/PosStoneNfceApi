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
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Exceptions;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Controller
{
    public class AuthController : MainController
    {

        private readonly IServiceToken<ServiceToken, TokenEntity> _serviceToken;

        public AuthController(IServiceToken<ServiceToken, TokenEntity> serviceToken, ILogger logger)
        {
            _serviceToken = serviceToken;
        }

        /// <summary>
        /// Loga um cliente na plataforma (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Auth/Login
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
        [Route("Api/[controller]/Login")]
        public IActionResult Login([FromBody] JsonElement post) {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceToken.GetDBRepository();
            StoneCodeRequest stoneCodeRequest = ObjectExtensions.GetDataObjectFromJsonPost<StoneCodeRequest>(post);
            AuthProcessor processor = new AuthProcessor(_dbRepository);
            var res = processor.Login(stoneCodeRequest);
            if (res.error == false) {
                response = new { bearer = res.bearer };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Verifica se um cliente está logado na plataforma (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Auth/IsLoggedIn
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
        [Route("Api/[controller]/IsLoggedIn")]
        public IActionResult IsLoggedIn([FromBody] JsonElement post) {
            dynamic response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceToken.GetDBRepository();
            StoneCodeRequest stoneCodeRequest = ObjectExtensions.GetDataObjectFromJsonPost<StoneCodeRequest>(post);
            AuthProcessor processor = new AuthProcessor(_dbRepository);
            var res = processor.IsLoggedIn(stoneCodeRequest);
            if (res.error == false) {
                response = new { is_logged_in = res.is_logged_in };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }

        /// <summary>
        /// Encripta uma senha para login na plataforma (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// POST /api/Auth/Encrypt
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
        [Route("Api/[controller]/Encrypt")]
        public IActionResult Encrypt([FromBody] JsonElement post) {
            dynamic response = new object();

            try {
                IDBRepository<PosStone> _dbRepository = _serviceToken.GetDBRepository();
                EncryptRequest encryptRequest = ObjectExtensions.GetDataObjectFromJsonPost<EncryptRequest>(post);
                AuthProcessor processor = new AuthProcessor(_dbRepository);
                var res = processor.Encrypt(encryptRequest);
                if (res.error == false) {
                    response = new { cypher = res.cypher };
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
        /// Loga o usuario master na plataforma (FINALIZADO)
        /// </summary>
        /// <remarks>
        /// GET /api/Auth/AuthMaster
        /// </remarks>
        /// <response code="200">{"error": false,"code": 200, "message": "ok","data": []}</response>
        /// <response code="403">{"error": true,"code": 403, "message": "Erro ao executar metodo %metodo%. Error: %msg_error%", "data": []}</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Route("Api/[controller]/AuthMaster")]
        public IActionResult AuthMaster() {
            object response = new {};

            IDBRepository<PosStone> _dbRepository = _serviceToken.GetDBRepository();
            AuthProcessor processor = new AuthProcessor(_dbRepository);
            var res = processor.AuthMaster();
            if (res.error == false) {
                response = new { bearer = res.bearer, expires = DateExtensions.UnixTimeStampToDateTime(res.expires), user = res.usuario, pass = res.pass };
            }
            else {
                AddProcessingError(res.msg);
            }

            return CustomResponse(response);
        }
       
    }
}