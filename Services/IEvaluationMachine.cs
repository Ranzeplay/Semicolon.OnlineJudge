using Semicolon.OnlineJudge.Models.Judge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Services
{
    public interface IEvaluationMachine
    {
        string CreateSourceFile(string code, long trackId);

        Task<string> CompileProgramAsync(string sourceFilePath, long trackId);

        PointStatus RunTest(string input, string output, string compiledProgramPath, long trackId);
    }
}
