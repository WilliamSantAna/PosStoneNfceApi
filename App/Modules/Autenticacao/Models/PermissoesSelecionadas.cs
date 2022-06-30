using System.Collections.Generic;

namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models
{
    public class PermissoesSelecionadas
    {
        public List<int> GruposEconomicos { get; set; }
        public List<string> Empresas { get; set; }
    }
}