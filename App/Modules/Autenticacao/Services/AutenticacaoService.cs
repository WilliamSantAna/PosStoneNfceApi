using PosStoneNfce.API.Portal.App.Common.Services;
using PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models;
using PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models.Response;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PosStoneNfce.API.Portal.Configuration;


namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Services
{
    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient _httpClient;
        private string _idUsuarioNovoPortalMide;

        public AutenticacaoService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            httpClient.BaseAddress = new Uri(settings.Value.PosStoneNfceApiAutenticacaoUrl);
            _httpClient = httpClient;
            _idUsuarioNovoPortalMide = settings.Value.IdUsuarioMide;
        }

        public async Task<(AuthenticationResponse response, string message)> GetAuthenticationMideAsync(string token)
        {
            var autenticacaoRequest = new AutenticacaoRequest(new Input(_idUsuarioNovoPortalMide));
            var inputContent = GetContent(autenticacaoRequest);
            SetRequestHeaders(token);

            var response = await _httpClient.PostAsync("/Cache/AcessoContaAssociada", inputContent);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await DeserializeResponseObject<APIResponse<AuthenticationResponse>>(response);
                return (apiResponse.Data, string.Empty);
            }                
            else
            {
                var error = await DeserializeResponseObject<ErroResponse>(response);
                return (null, error?.Errors ?? "Falha de Autenticação com o MIDe para salvar a empresa.");
            }
        }

        public async Task<APIResponse<List<CachePermissaoUsuarioEmpresa>>> GetUsuarioPermissaoEmpresaAsync(string token)
        {
            var inputContent = GetContent(new object());
            SetRequestHeaders(token);
            var response = await _httpClient.PostAsync("/Cache/Usuario/PermissaoEmpresa", inputContent);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await DeserializeResponseObject<APIResponse<List<CachePermissaoUsuarioEmpresa>>>(response);
                apiResponse.Sucesso = true;
                return apiResponse;
            }
            else
            {
                var erroResponse = await DeserializeResponseObject<ErroResponse>(response);
                return new APIResponse<List<CachePermissaoUsuarioEmpresa>>(erroResponse?.Errors ?? response.ReasonPhrase);
            }
        }


        public async Task<IEnumerable<CachePermissaoUsuarioGrupoEconomico>> GetUsuarioPermissaoGrupoEconomico(string token)
        {
            var inputContent = GetContent(new object());
            SetRequestHeaders(token);
            var response = await _httpClient.PostAsync("/Cache/Usuario/PermissaoEmpresa", inputContent);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await DeserializeResponseObject<APIResponse<List<CachePermissaoUsuarioGrupoEconomico>>>(response);
                apiResponse.Sucesso = true;
                return apiResponse.Data;
            }
            else
            {
                var erroResponse = await DeserializeResponseObject<ErroResponse>(response);
                throw new Exception(erroResponse.Errors);
            }
        }

        public async Task<APIResponse<PermissoesSelecionadas>> GetUsuarioPermissoesSelecionadasAsync(string token)
        {
            var inputContent = GetContent(new object());
            SetRequestHeaders(token);
            var response = await _httpClient.PostAsync("/Cache/Usuario/GetPermissoesSelecionadas", inputContent);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await DeserializeResponseObject<APIResponse<PermissoesSelecionadas>>(response);
                apiResponse.Sucesso = true;
                return apiResponse;
            }
            else
            {
                var erroResponse = await DeserializeResponseObject<ErroResponse>(response);
                return new APIResponse<PermissoesSelecionadas>(erroResponse?.Errors ?? response.ReasonPhrase);
            }
        }

        private void SetRequestHeaders(string token)
        {
            if (_httpClient.DefaultRequestHeaders.Any())
                _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");            
        }
    }
}