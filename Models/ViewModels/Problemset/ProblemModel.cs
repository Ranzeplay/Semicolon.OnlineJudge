using Semicolon.OnlineJudge.Models.Problemset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Problemset
{
    public class ProblemModel : Problem
    {
        public string Author { get; set; }

        public string ContentRaw { get; set; }

        public string ContentHtml { get; set; }
    }
}
