using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Hubs
{
    public class TrackHub : Hub
    {
        private readonly ApplicationDbContext _context;

        private readonly IEvaluationMachine _evaluationMachine;

        public TrackHub(ApplicationDbContext context, IEvaluationMachine evaluationMachine)
        {
            _context = context;
            _evaluationMachine = evaluationMachine;
        }

        public async Task GetTrack(string trackId)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == long.Parse(trackId));
            if(track.Status != JudgeStatus.Pending)
            {
                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                await Clients.Caller.SendAsync("finish");
                return;
            }

            var problem = await _context.Problems.FirstOrDefaultAsync(q => q.Id == track.ProblemId);
            if (problem != null)
            {
                var testdata = problem.GetJudgeProfile().GetJudgeDatas();

                string sourceFilePath = _evaluationMachine.CreateSourceFile(track.CodeEncoded, track.Id);
                string programPath = await _evaluationMachine.CompileProgramAsync(sourceFilePath, track.Id);
                for (int i = 0; i < testdata.Count; i++)
                {
                    var data = testdata[i];
                    var result = _evaluationMachine.RunTest(data.Input, data.Output, programPath, track.Id);

                    var status = track.GetPointStatus();
                    status[i].Id = i;
                    status[i].Status = result;
                    track.SetPointStatus(status);

                    await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                }

                if (track.GetPointStatus().FirstOrDefault(x => x.Status != PointStatus.Accepted) != null)
                {
                    track.Status = JudgeStatus.WrongAnswer;
                }
                else
                {
                    track.Status = JudgeStatus.Accept;
                    var passRate = problem.GetPassRate();
                    passRate.Pass += 1;
                    problem.SetPassRate(passRate);
                    _context.Update(problem);
                }

                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                _context.Tracks.Update(track);
                await _context.SaveChangesAsync();
            }

            await Clients.Caller.SendAsync("finish");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
