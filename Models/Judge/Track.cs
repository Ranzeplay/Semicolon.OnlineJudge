using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.Judge
{
    public class Track
    {
        public long Id { get; set; }

        public long ProblemId { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreateTime { get; set; }

        public JudgeStatus Status { get; set; }

        public string PointStatus { get; set; }
        
        public string CodeEncoded { get; set; }

        public string CompilerOutput { get; set; }

        public SupportProgrammingLanguage Language { get; set; }

        public void SetPointStatus(List<Point> vs)
        {
            PointStatus = System.Text.Json.JsonSerializer.Serialize(vs);
        }

        public List<Point> GetPointStatus()
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<Point>>(PointStatus);
        }
    }
}
