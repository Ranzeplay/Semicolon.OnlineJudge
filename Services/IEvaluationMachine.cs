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
        string CreateSourceFile(string code, Track track);

        PointStatus RunTest(TestData data, string compiledProgramPath, Track track, Problem problem);

        string CompileProgram(Track trackIn, out Track track);
    }
}
