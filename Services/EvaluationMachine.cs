using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Services
{
    public class EvaluationMachine : IEvaluationMachine
    {
        private readonly ApplicationDbContext _context;

        public EvaluationMachine(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CompileProgramAsync(string sourceFilePath, long trackId)
        {
            var track = _context.Tracks.FirstOrDefault(x => x.Id == trackId);

            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");
            path = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var programSourceFilePath = Path.Combine(path, "source.c");

            string compileOutput = string.Empty;
            try
            {
                using Process compilerProcess = new Process();
                compilerProcess.StartInfo.UseShellExecute = false;
                compilerProcess.StartInfo.CreateNoWindow = true;
                compilerProcess.StartInfo.RedirectStandardInput = true;
                compilerProcess.StartInfo.RedirectStandardOutput = true;
                compilerProcess.StartInfo.WorkingDirectory = path;
                compilerProcess.StartInfo.FileName = "cmd.exe";
                compilerProcess.Start();
                compilerProcess.StandardInput.WriteLine("gcc source.c");
                compilerProcess.StandardInput.WriteLine("exit");
                compileOutput = compilerProcess.StandardOutput.ReadToEnd();
                compilerProcess.WaitForExit();

                if (string.IsNullOrWhiteSpace(compileOutput))
                {
                    track.CompilerOutput = "None";
                }
                else
                {
                    track.CompilerOutput = compileOutput;
                }

                _context.Tracks.Update(track);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Path.Combine(path, "a.exe");
        }

        public string CreateSourceFile(string code, long trackId)
        {
            var track = _context.Tracks.FirstOrDefault(x => x.Id == trackId);

            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");

            var programSourceFilePath = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(programSourceFilePath))
            {
                Directory.CreateDirectory(programSourceFilePath);
            }

            programSourceFilePath = Path.Combine(programSourceFilePath, "source.c");
            if (!File.Exists(programSourceFilePath))
            {
                File.Create(programSourceFilePath).Close();
            }
            else
            {
                File.Delete(programSourceFilePath);
                File.Create(programSourceFilePath).Close();
            }

            using (var tw = new StreamWriter(programSourceFilePath, true))
            {
                tw.WriteLine(Base64Decode(track.CodeEncoded));
                tw.Close();
            }

            return programSourceFilePath;
        }

        public PointStatus RunTest(string input, string expectedOutput, string compiledProgramPath, long trackId)
        {
            var track = _context.Tracks.FirstOrDefault(x => x.Id == trackId);
            var problem = _context.Problems.FirstOrDefault(p => p.Id == track.ProblemId);

            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");
            path = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var programSourceFilePath = Path.Combine(path, "a.exe");

            string programOutput = string.Empty;
            try
            {
                using Process programProcess = new Process();
                programProcess.StartInfo.UseShellExecute = false;
                programProcess.StartInfo.CreateNoWindow = true;
                programProcess.StartInfo.RedirectStandardInput = true;
                programProcess.StartInfo.RedirectStandardOutput = true;
                programProcess.StartInfo.WorkingDirectory = path;
                programProcess.StartInfo.FileName = programSourceFilePath;
                programProcess.Start();
                programProcess.StandardInput.WriteLine(input);
                programOutput = programProcess.StandardOutput.ReadToEnd();
                programProcess.WaitForExit();

                var runningTime = (DateTime.Now - programProcess.StartTime).Seconds;
                if(runningTime > problem.GetJudgeProfile().TimeLimit)
                {
                    return PointStatus.TimeLimitExceeded;
                }

                if (expectedOutput == programOutput)
                {
                    return PointStatus.Accepted;
                }
                else
                {
                    return PointStatus.WrongAnswer;
                }
            }
            catch
            {
                return PointStatus.InternalError;
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
