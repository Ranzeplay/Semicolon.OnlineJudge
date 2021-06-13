using Semicolon.OnlineJudge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Services
{
    public interface IBuildRuleService
    {
        // List<BuildRule> GetAllConfigurations();

        BuildRule GetConfiguration(string name);

        List<SupportedProgrammingLanguage> GetSupportedProgrammingLanguages();

        BuildRule GetById(string id);
    }
}
