namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{

    public class LXUpdate {
        public int AffectedRows { get; set; }
    }

    public class LXUpdateInput<T>
    {
        public string Table { get; set; }

        public T Data { get; set; }

        public string[][] Where { get; set; }
    }
}