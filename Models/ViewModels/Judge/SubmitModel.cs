using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Judge
{
    public class SubmitModel
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public ProgrammingLanguage Language { get; set; }
    }
}
