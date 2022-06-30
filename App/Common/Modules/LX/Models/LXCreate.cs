namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{

    public class LXCreate<T> {
        public T Id { get; set; }
    }

    public class LXCreateInput<T>
    {
        public string Table { get; set; }

        public T Data { get; set; }

        public string[][] Where { get; set; }
    }
}