using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.User
{
    public class OJUser
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string ProblemsPassedId { get; set; }

        public List<long> GetProblemsPassedId()
        {
            if (!string.IsNullOrWhiteSpace(ProblemsPassedId))
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<long>>(ProblemsPassedId);
            }

            return new List<long>();
        }

        public void SetProblemsPassedId(List<long> problemsPassedId)
        {
            ProblemsPassedId = System.Text.Json.JsonSerializer.Serialize(problemsPassedId);
        }
    }
}
