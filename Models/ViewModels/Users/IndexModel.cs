using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Users
{
    public class IndexModel
    {
        public OJUser User { get; set; }

        public List<Track> Tracks { get; set; }
    }
}
