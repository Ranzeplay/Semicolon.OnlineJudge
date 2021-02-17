using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models
{
    public class BuildRole
    {
        public string DisplayName { get; set; }

        public string[] BuildSteps { get; set; }

        public string RunCommand { get; set; }
    }
}
