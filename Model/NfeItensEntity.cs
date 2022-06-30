using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.Model
{
    public class NfeItensEntity: IEntity
    {
        public int nfe_id { get; set; }
        public string produto_id { get; set; }
        public double qty { get; set; }
        public double vlr { get; set; }
        public string GetPkName() {
            return "id";
        }

        public string GetPkValue() {
            return this.nfe_id.ToString();
        }

        public string GetTableName() {
            string TableName = this.GetType().Name;
            return TableName.Substring(0, TableName.Length - 6).ToLower();
        }
    }
}