using Microsoft.EntityFrameworkCore;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.Problemset;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
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

        public string CreateSourceFile(string code, Track track)
        {
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
                tw.WriteLine(Base64Decode(code));
                tw.Close();
            }

            return programSourceFilePath;
        }

        public string CompileProgram(Track trackIn, out Track track)
        {
            track = trackIn;

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

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    compilerProcess.StartInfo.FileName = "cmd.exe";
                }
                else
                {
                    compilerProcess.StartInfo.FileName = "/bin/bash";
                }

                compilerProcess.Start();

                if (track.Language == Models.SupportProgrammingLanguage.C)
                {
                    compilerProcess.StandardInput.WriteLine("gcc source.c");
                }
                else if (track.Language == Models.SupportProgrammingLanguage.Cpp)
                {
                    compilerProcess.StandardInput.WriteLine("g++ source.c");
                }

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(path, "a.exe");
            }
            else
            {
                return Path.Combine(path, "a.out");
            }
        }

        public PointStatus RunTest(TestData data, string compiledProgramPath, Track track, Problem problem)
        {
            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");
            path = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                using Process programProcess = new Process();
                programProcess.StartInfo.UseShellExecute = false;
                programProcess.StartInfo.CreateNoWindow = true;
                programProcess.StartInfo.RedirectStandardInput = true;
                programProcess.StartInfo.RedirectStandardOutput = true;
                programProcess.StartInfo.WorkingDirectory = path;
                programProcess.StartInfo.FileName = compiledProgramPath;
                programProcess.Start();
                programProcess.StandardInput.WriteLine(data.Input);
                string programOutput = programProcess.StandardOutput.ReadToEnd();
                Thread.Sleep(Convert.ToInt32(problem.GetJudgeProfile().TimeLimit * 1000) + 1000);

                if (!programProcess.HasExited)
                {
                    programProcess.Kill();
                }
                programProcess.WaitForExit();

                var runningTime = ((programProcess.ExitTime - programProcess.StartTime).TotalMilliseconds) / 1000;
                if (runningTime > problem.GetJudgeProfile().TimeLimit)
                {
                    return PointStatus.TimeLimitExceeded;
                }

                if (data.Output.Trim().TrimEnd('\n').Trim().Replace("\r", "").Equals(programOutput.Trim().TrimEnd('\n').Trim().Replace("\r", "")))
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
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
