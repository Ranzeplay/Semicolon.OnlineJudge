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
        private readonly IBuildRoleService _buildRoleService = new BuildRoleService();

        public string CreateSourceFile(string code, Track track)
        {
            var path = _storePath;
            var rule = _buildRoleService.GetById(track.LanguageId);

            var programSourceFilePath = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(programSourceFilePath))
            {
                Directory.CreateDirectory(programSourceFilePath);
            }

            programSourceFilePath = Path.Combine(programSourceFilePath, rule.SourceFileName);
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

        public bool CompileProgram(Track trackIn, out Track track)
        {
            track = trackIn;

            // Get judge storage directory
            var path = Path.Combine(_storePath, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                var rule = _buildRoleService.GetById(track.LanguageId);

                using Process compilerProcess = new();
                compilerProcess.StartInfo.UseShellExecute = false;
                compilerProcess.StartInfo.CreateNoWindow = true;
                compilerProcess.StartInfo.RedirectStandardInput = true;
                compilerProcess.StartInfo.RedirectStandardOutput = true;
                compilerProcess.StartInfo.RedirectStandardError = true;
                compilerProcess.StartInfo.WorkingDirectory = path;

                compilerProcess.StartInfo.FileName = rule.CompilationConfiguration.Program;
                if (rule.CompilationConfiguration.Arguments != null)
                {
                    var args = rule.CompilationConfiguration.Arguments.Replace("{sourceFile}", rule.SourceFileName);
                    compilerProcess.StartInfo.Arguments = args;
                    compilerProcess.Start();
                }
                else
                {
                    compilerProcess.Start();
                    foreach (var step in rule.CompilationConfiguration.Steps)
                    {
                        compilerProcess.StandardInput.WriteLine(step.Replace("{sourceFile}", rule.SourceFileName));
                    }
                }

                compilerProcess.WaitForExit();
                string compileOutput = compilerProcess.StandardOutput.ReadToEnd();

                if (string.IsNullOrWhiteSpace(compileOutput))
                {
                    track.CompilerOutput = "None";
                }
                else
                {
                    track.CompilerOutput = compileOutput;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PointStatus RunTest(TestData data, Track track, Problem problem)
        {
            var path = Path.Combine(_storePath, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                var rule = _buildRoleService.GetById(track.LanguageId);

                using Process executableProcess = new();
                executableProcess.StartInfo.UseShellExecute = false;
                executableProcess.StartInfo.CreateNoWindow = true;
                executableProcess.StartInfo.RedirectStandardInput = true;
                executableProcess.StartInfo.RedirectStandardOutput = true;
                executableProcess.StartInfo.WorkingDirectory = path;

                executableProcess.StartInfo.FileName = Path.Combine(path, rule.ExecutionConfiguration.Program);
                if (rule.ExecutionConfiguration.Arguments != null)
                {
                    executableProcess.StartInfo.Arguments = rule.ExecutionConfiguration.Arguments;
                    executableProcess.Start();
                }
                else
                {
                    executableProcess.Start();
                    foreach (var step in rule.ExecutionConfiguration.Steps)
                    {
                        executableProcess.StandardInput.WriteLine(step.Replace("{inputData}", data.Input));
                    }
                }

                string programOutput = executableProcess.StandardOutput.ReadToEnd();
                Thread.Sleep(Convert.ToInt32(problem.GetJudgeProfile().TimeLimit * 1000) + 1000);

                if (!executableProcess.HasExited)
                {
                    executableProcess.Kill();
                }
                executableProcess.WaitForExit();

                var runningTime = ((executableProcess.ExitTime - executableProcess.StartTime).TotalMilliseconds) / 1000;
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
