using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SqlKata;

namespace PosStoneNfce.API.Portal.App.Common.Modules.LX
{
    public class LXFindAll<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Rows { get; set; }
    }
    

    public class LXFindAllInput
    {
        public string Table { get; set; }

        public string[][] Filter { get; set; }

        [Required]
        public string[] Attributes { get; set; }

        public string SortField { get; set; }

        [RegularExpression(@"^(ascend|descend)$", 
         ErrorMessage = "ascend ou descend")]
        public string SortOrder { get; set; }

        public int PageSize { get; set; } = 5;

        public int PageIndex { get; set; } = 1;

        public LXJoin[] Joins { get; set; }

        public string As { get; set; }

        public Func<Query, Query> BeforeExecute { get; set; }

    }


    public class LXUpdateInput<T1, T2>
    {
        public string Table { get; set; }

        public T1 Id { get; set; }

        public T2 Data { get; set; }

        public Func<Query, Query> BeforeExecute { get; set; }

    }


    public class LXJoin {
        public string Table { get; set; }

        public string As { get; set; }

        public string[][] Where { get; set; }

        public string SourceKey { get; set; }

        public string TargetKey { get; set; }

        public string Type { get; set; }
    }


    public class LXAttribute 
    {
        public string Name { get; set; }
        
        public string From { get; set; }

        public string As { get; set; }
    }


    public class EntityID<T> 
    {
        public T Id { get; set; }
    }

}