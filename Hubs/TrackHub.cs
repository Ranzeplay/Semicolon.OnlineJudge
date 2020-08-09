using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.Problemset;
using Semicolon.OnlineJudge.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Hubs
{
    public class TrackHub : Hub
    {
        private readonly ApplicationDbContext _context;

        private readonly IEvaluationMachine _evaluationMachine;
        private readonly ILogger<TrackHub> _logger;

        public TrackHub(ApplicationDbContext context, IEvaluationMachine evaluationMachine, ILogger<TrackHub> logger)
        {
            _context = context;
            _evaluationMachine = evaluationMachine;
            _logger = logger;
        }

        public async Task GetTrack(string trackId)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == long.Parse(trackId));
            await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

            if (track.Status != JudgeStatus.Pending)
            {
                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                await Clients.Caller.SendAsync("finish");
                return;
            }

            var problem = await _context.Problems.FirstOrDefaultAsync(q => q.Id == track.ProblemId);
            if (problem != null)
            {
                _logger.Log(LogLevel.Information, $"[{DateTime.UtcNow}] Starting to check code of track {track.Id}", track.CodeEncoded);

                // Load test data to memory
                var testdata = new List<TestData>();
                var problemDirectory = Path.Combine(Directory.GetCurrentDirectory(), "JudgeDataStorage", problem.Id.ToString(), "data");
                Directory.GetDirectories(problemDirectory).ToList().ForEach(async element =>
                {
                    var currentPath = Path.Combine(problemDirectory, element);

                    TestData data = new TestData
                    {
                        Input = await File.ReadAllTextAsync(Path.Combine(currentPath, "data.in")),
                        Output = await File.ReadAllTextAsync(Path.Combine(currentPath, "data.out"))
                    };
                    testdata.Add(data);
                });

                // Compile source code
                string sourceFilePath = _evaluationMachine.CreateSourceFile(track.CodeEncoded, track);
                string programPath = _evaluationMachine.CompileProgram(track, out Track trackOut);
                track = trackOut;

                // Push compile log to client
                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

                // If compile failed, the executable file will not be exist
                if (!File.Exists(programPath))
                {
                    track.Status = JudgeStatus.CompileError;
                    var pointStatus = track.GetPointStatus();
                    for (int i = 0; i < pointStatus.Count; i++)
                    {
                        pointStatus[i].Status = PointStatus.InternalError;
                    }
                    track.SetPointStatus(pointStatus);
                    _context.Tracks.Update(track);
                    await _context.SaveChangesAsync();
                    await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                    await Clients.Caller.SendAsync("finish");
                    return;
                }

                var status = track.GetPointStatus();
                status.ForEach(element =>
                {
                    element.Status = PointStatus.Judging;
                });

                track.SetPointStatus(status);

                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

                var tasks = new List<Task<PointStatus>>();
                for (int i = 0; i < testdata.Count; i++)
                {
                    var task = new Task<PointStatus>(() =>
                    {
                        var section = i - 1;
                        var data = testdata[section];
                        var result = _evaluationMachine.RunTest(data, programPath, track, problem);

                        return result;

                        // var currentStatus = status;

                        // currentStatus[section].Id = section;
                        // currentStatus[section].Status = result;
                        // status = currentStatus;
                        // Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track))).RunSynchronously();
                    });

                    task.Start();
                    tasks.Add(task);
                }

                var result = await Task.WhenAll(tasks);
                for(int i = 0; i < result.Length; i++)
                {
                    status[i].Status = result[i];
                }

                track.SetPointStatus(status);

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

                _context.Tracks.Update(track);
                await _context.SaveChangesAsync();

                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

                _logger.Log(LogLevel.Information, $"[{DateTime.UtcNow}] Completed the check of code of track {track.Id}", track);
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
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
