using Semicolon.OnlineJudge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Services
{
    public class BuildRoleService : IBuildRoleService
    {
        private readonly string _storePath = Path.Combine(Directory.GetCurrentDirectory(), "BuildRoles");

        private readonly List<BuildRule> _rules;

        public BuildRoleService()
        {
            _rules = GetAllConfigurations();
        }

        private List<BuildRule> GetAllConfigurations()
        {
            var configFiles = Directory.GetFiles(_storePath, "*.json");

            var configs = new List<BuildRule>();
            foreach (var file in configFiles)
            {
                var content = File.ReadAllText(file);
                configs.Add(JsonSerializer.Deserialize<BuildRule>(content));
            }

            return configs;
        }

        public BuildRule GetConfiguration(string name)
        {
            return _rules.FirstOrDefault(c => c.DisplayName == name);
        }
       

        public List<SupportedProgrammingLanguage> GetSupportedProgrammingLanguages()
        {
            var result = new List<SupportedProgrammingLanguage>();

            foreach (var role in _rules)
            {
                result.Add(new()
                {
                    Id = role.EditorLanguage,
                    DisplayName = role.DisplayName
                });
            }

            return result;
        }

        public BuildRule GetById(string id)
        {
            return _rules.FirstOrDefault(c => c.EditorLanguage == id);
        }
    }
}
