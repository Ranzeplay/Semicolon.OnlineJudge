using Microsoft.AspNetCore.SignalR;
using Semicolon.OnlineJudge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Hubs
{
    public class TrackHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public TrackHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task getTrack(long id)
        {
            var status = _context.Tracks.FirstOrDefault(t => t.Id == id);
        }
    }
}
