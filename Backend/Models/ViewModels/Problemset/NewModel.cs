using Microsoft.AspNetCore.Http;
using Semicolon.OnlineJudge.Models.Problemset;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Problemset
{
    public class NewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        // [Required]
        public TestData ExampleData { get; set; }

        [Required]
        public double MemoryLimit { get; set; }

        [Required]
        public double TimeLimit { get; set; }

        [Required]
        public IFormFile TestDatas { get; set; }
    }
}
