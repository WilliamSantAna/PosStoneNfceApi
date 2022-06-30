using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.Model
{
    public class EmpresaEntity: IEntity
    {
        public int stone_code { get; set; }

        public string cnpj { get; set; }

        public int aceite_termos_uso { get; set; }

        public int num_nfe { get; set; }

        public int serie_nfe { get; set; }

        public string GetPkName() {
            return "stone_code";
        }

        public string GetPkValue() {
            return this.stone_code.ToString();
        }
        public string GetTableName() {
            string TableName = this.GetType().Name;
            return TableName.Substring(0, TableName.Length - 6).ToLower();
        }

    }
}