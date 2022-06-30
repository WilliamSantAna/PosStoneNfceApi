namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{

    public class LXDelete 
    {
        public int AffectedRows { get; set; }
    }

    public class LXDeleteInput
    {
        public string Table { get; set; }

        public string[][] Where { get; set; }
    }
}