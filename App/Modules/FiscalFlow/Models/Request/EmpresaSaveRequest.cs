using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class EmpresaSaveRequest : IRequest
    {

        public string stone_code;
        public string razao_social;
        public string cnpj;
        public string email;
        public string telefone;
        public string endereco;
        public string numero;
        public string bairro;
        public string municipio;
        public string cod_ibge_municipio;
        public string uf;
        public string cep;
        public string ie;
        public string id_csc;
        public string cod_csc;
    }
}