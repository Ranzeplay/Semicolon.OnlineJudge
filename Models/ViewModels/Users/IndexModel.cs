using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Models.Judge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.ViewModels.Users
{
    public class IndexModel
    {
        public SemicolonUser User { get; set; }

        public List<Track> Tracks { get; set; }
    }
}
