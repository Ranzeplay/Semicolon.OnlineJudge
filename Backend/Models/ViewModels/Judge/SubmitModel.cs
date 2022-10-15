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

        // This is only for displaying languages
        public List<SupportedProgrammingLanguage> Languages { get; set; }

        [Required]
        public string LanguageId { get; set; }
    }
}
