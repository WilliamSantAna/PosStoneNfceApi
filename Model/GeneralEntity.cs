using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.Model
{
    public class GeneralEntity: IEntity
    {
        public string tipo { get; set; }

        public string texto { get; set; }

        public string GetPkName() {
            return "tipo";
        }

        public string GetPkValue() {
            return this.tipo.ToString();
        }

        public string GetTableName() {
            string TableName = this.GetType().Name;
            return TableName.Substring(0, TableName.Length - 6).ToLower();
        }
    }
}