using System;
using System.ComponentModel.DataAnnotations;
using SqlKata;

namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{
    
    public class LXFindOneInput
    {
        public string Table { get; set; }

        [Required]
        public string[][] Filter { get; set; }

        [Required]
        public string[] Attributes { get; set; }

        public string As { get; set; }

        public Func<Query, Query> BeforeExecute { get; set; }

    }

}