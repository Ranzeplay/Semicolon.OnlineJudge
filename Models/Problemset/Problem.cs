using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.Problemset
{
    public class Problem
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string AuthorId { get; set; }

        public string Description { get; set; }

        public DateTime PublishTime { get; set; }

        public TestData ExampleData { get; set; }

        public JudgeProfile JudgeProfile { get; set; }

        public PassRate PassRate { get; set; }
    }
}
