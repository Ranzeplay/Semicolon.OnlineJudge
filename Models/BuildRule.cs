using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models
{
    public class BuildRule
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// The language settings of monaco-editor
        /// </summary>
        public string EditorLanguage { get; set; }

        public string SourceFileName { get; set; }

        public Commands CompilationConfiguration { get; set; }

        public Commands ExecutionConfiguration { get; set; }
    }

    public class Commands
    {
        public string Program { get; set; }

        public string Arguments { get; set; }

        public string[] Steps { get; set; }
    }
}
