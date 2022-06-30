using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.Model
{
    public class NfeEntity: IEntity
    {
        public int num_nfe { get; set; }
        public int stone_code { get; set; }
        public int client_id_nota { get; set; }
        public string chave_nfe { get; set; }
        public string serie_nfe { get; set; }
        public string status { get; set; }
        public string motivo { get; set; }
        public int contingencia { get; set; }
        public int dh_emissao { get; set; }
        public int dh_retorno { get; set; }
        public int tipo_pagamento { get; set; }
        public double valor_pagamento { get; set; }
        public string cpf_cnpj_destinatario { get; set; }
        public string cnpj_cc { get; set; }
        public string bandeira { get; set; }
        public string cod_pgto { get; set; }
        public string url_danfe { get; set; }

        public string GetPkName() {
            return "num_nfe";
        }

        public string GetPkValue() {
            return this.num_nfe.ToString();
        }

        public string GetTableName() {
            string TableName = this.GetType().Name;
            return TableName.Substring(0, TableName.Length - 6).ToLower();
        }

    }
}