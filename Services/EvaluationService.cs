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
    public class EvaluationService : IEvaluationService
    {
        private readonly string _storePath = Path.Combine(Directory.GetCurrentDirectory(), "EvaluationMachine");

        public string CreateSourceFile(string code, Track track)
        {
            var path = _storePath;

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

            var path = Path.Combine(_storePath, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

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
                string compileOutput = compilerProcess.StandardOutput.ReadToEnd();
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
            catch (Exception)
            {
                throw;
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
            var path = Path.Combine(_storePath, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                var programProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = path,
                        FileName = compiledProgramPath
                    },
                };
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

                if (data.Output.Trim().TrimEnd('\n').Trim().Replace("\r", "") == programOutput.Trim().TrimEnd('\n').Trim().Replace("\r", ""))
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
