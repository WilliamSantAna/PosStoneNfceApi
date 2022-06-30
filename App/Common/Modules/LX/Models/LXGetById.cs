namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{
    
    public class LXGetByIdInput<T>
    {
        public T Id { get; set; }

        public string[] Attributes { get; set; }

    }

}