using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Response;
using PosStoneNfce.API.Portal.Configuration;


namespace PosStoneNfce.API.Portal.App.Common.Controller
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected ActionResult Success(object dataResponse) {
            if (Errors.Any()) {
                return BadRequest(new { errors = Errors.ToArray() });
            }

            SuccessResponse successResponse = new SuccessResponse();
            successResponse.error = false;
            successResponse.code = 200;
            successResponse.message = "ok";
            successResponse.enviroment = ApiConfig.Configuration["ConfigName"];
            successResponse.data = dataResponse;
            return Ok(successResponse);
        }

        protected ActionResult Error(string errorMsg) {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);

            ErrorResponse errorResponse = new ErrorResponse();
            errorResponse.error = true;
            errorResponse.code = 403;
            errorResponse.enviroment = ApiConfig.Configuration["ConfigName"];
            errorResponse.message = "Erro ao executar metodo. Error: " + errorMsg;
            errorResponse.data = new {};
            return Ok(errorResponse);
        }

        protected ICollection<string> Errors = new List<string>();

        protected ActionResult CustomResponse(object result = null)
        {
            if (Errors.Any()) {
                var errors = Errors.ToArray();
                //return BadRequest(new { errors = Errors.ToArray() });
                return Error(string.Join(" | ", errors));
            }

            return Success(result);
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);

            foreach (var error in errors)
            {
                AddProcessingError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected void AddProcessingError(string erro)
        {
            if (Errors.Any())
                Errors.Add($" {erro}");
            else
                Errors.Add(erro);
        }

        protected void ErrorsClear()
        {
            Errors.Clear();
        }
    }
}