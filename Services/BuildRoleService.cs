using Semicolon.OnlineJudge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Semicolon.OnlineJudge.Services
{
    public class BuildRoleService : IBuildRoleService
    {
        private readonly string _storePath = Path.Combine(Directory.GetCurrentDirectory(), "BuildRoles");

        public BuildRole GetConfiguration(string name)
        {
            var configs = GetAllConfigurations();
            return configs.FirstOrDefault(c => c.DisplayName == name);
        }

        public List<BuildRole> GetAllConfigurations()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var configFiles = Directory.GetFiles(_storePath, "*.yml");

            var configs = new List<BuildRole>();
            foreach (var file in configFiles)
            {
                var content = File.ReadAllText(file);
                configs.Add(deserializer.Deserialize<BuildRole>(content));
            }

            return configs;
        }
    }
}
