using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request;
using System.Collections.Generic;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class NfeGenerateRequest : IRequest
    {
        public string stone_code;
        public string tipo_pagamento;
        public string valor_pagamento;
        public string cpf_cnpj_destinatario;
        public string cnpj_cc;
        public string bandeira;
        public string cod_pgto;
        public List<NfeGenerateItemsRequest> items;
    }
}