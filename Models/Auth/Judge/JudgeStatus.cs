using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.Judge
{
    public enum JudgeStatus
    {
        Accept,
        WrongAnswer,
        CompileError,
        UnknownError,
        Pending
    }
}
