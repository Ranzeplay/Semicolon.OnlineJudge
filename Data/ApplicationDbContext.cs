using Microsoft.EntityFrameworkCore;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.Problemset;
using Semicolon.OnlineJudge.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<OJUser> OJUsers { get; set; }

        public DbSet<Problem> Problems { get; set; }

        public DbSet<Track> Tracks { get; set; }
    }
}
