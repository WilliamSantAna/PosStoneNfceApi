namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models
{
    public class CachePermissaoUsuarioEmpresa
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public int GrupoEconomicoId { get; set; }
    }
}