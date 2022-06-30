using System.Collections.Generic;

namespace PosStoneNfce.Common.Modules.Email
{
    public class EmailMetadata
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> To { get; set; }
    }
}