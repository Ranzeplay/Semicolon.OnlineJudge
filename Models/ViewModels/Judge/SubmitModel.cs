using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Judge
{
    public class SubmitModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public SupportProgrammingLanguage Language { get; set; }
    }
}
