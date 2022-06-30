using PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models;
using PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Services
{
    public interface IAutenticacaoService
    {
        Task<(AuthenticationResponse response, string message)> GetAuthenticationMideAsync(string token);        
        Task<APIResponse<List<CachePermissaoUsuarioEmpresa>>> GetUsuarioPermissaoEmpresaAsync(string token);
        Task<IEnumerable<CachePermissaoUsuarioGrupoEconomico>> GetUsuarioPermissaoGrupoEconomico(string token);
        Task<APIResponse<PermissoesSelecionadas>> GetUsuarioPermissoesSelecionadasAsync(string token);
    }
}