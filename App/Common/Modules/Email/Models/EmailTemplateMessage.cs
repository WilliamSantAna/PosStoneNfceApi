using System.Collections.Generic;

namespace PosStoneNfce.Common.Modules.Email
{
    public class EmailTemplateMessage
    {
        public string Identifier { get; set; }
        public string Template { get; set; }
        public IDictionary<string, dynamic> DataBind { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> To { get; set; }
    }
}