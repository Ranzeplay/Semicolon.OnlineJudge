using Semicolon.OnlineJudge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Services
{
    public interface IBuildRoleService
    {
        List<BuildRole> GetAllConfigurations();

        BuildRole GetConfiguration(string name);
    }
}
