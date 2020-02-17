using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.Problemset;
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

        PointStatus RunTest(TestData data, string compiledProgramPath, Track track, Problem problem);
    }
}
