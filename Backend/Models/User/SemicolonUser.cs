using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Semicolon.Auth.Models
{
    public class SemicolonUser : IdentityUser
    {
        // public override string Id { get => base.Id; set => base.Id = value; }

        // public override string Email { get => base.Email; set => base.Email = value; }

        // public override string UserName { get => base.UserName; set => base.UserName = value; }

        public DateTime CreateTime { get; set; }

        public string authorizedApps { get; set; }
        public List<string> GetAuthorizedApps()
        {
            if (string.IsNullOrWhiteSpace(authorizedApps))
            {
                return new List<string>();
            }
            return JsonSerializer.Deserialize<List<string>>(authorizedApps);
        }

        public void SetAuthorizedApps(List<string> value)
        {
            authorizedApps = JsonSerializer.Serialize(value);
        }
    }
}
