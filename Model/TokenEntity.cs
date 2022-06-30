using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.Model
{
    public class TokenEntity: IEntity
    {
        public int stone_code { get; set; }
        public string usuario { get; set; }
        public string bearer { get; set; }
        public int expires { get; set; }

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