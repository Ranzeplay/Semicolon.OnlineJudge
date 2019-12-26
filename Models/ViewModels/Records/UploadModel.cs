using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.Problemset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Records
{
    public class UploadModel : Track
    {
        public UploadModel(Track track, string author)
        {
            Id = track.Id;
            PointStatus = track.PointStatus;
            ProblemId = track.ProblemId;
            Status = track.Status;
            AuthorId = track.AuthorId;
            CompilerOutput = track.CompilerOutput;
            CodeEncoded = track.CodeEncoded;
            CreateTime = track.CreateTime;

            Author = author;
        }

        public string Author { get; set; }

        public long GetPassedCount()
        {
            return GetPointStatus().Where(p => p.Status == Models.Judge.PointStatus.Accepted).LongCount();
        }

        public string GetPassRate()
        {
            return new PassRate { Pass = GetPassedCount(), Submit = GetPointStatus().Count }.ToUIString();
        }
    }
}
