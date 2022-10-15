namespace Semicolon.OnlineJudge.Models.Judge
{
    public enum PointStatus
    {
        Accepted,
        WrongAnswer,
        RuntimeError,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        OutputLimitExceeded,
        PresentationError,
        InternalError,
        Judging,
        Pending
    }
}