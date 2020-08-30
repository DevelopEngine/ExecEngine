using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ExecEngine
{
    public struct CommandOutput
    {
        public int ExitCode {get;set;}
        public string Output {get;set;}
    }
    public class CommandRunner : IDisposable
    {
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _process.Dispose();
            }
        }

        public ProcessStartInfo StartInfo => _process?.StartInfo;

        public CommandRunner(string cmdName)
        {
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                //RedirectStandardError = true,
                FileName = cmdName,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _process = new Process();
            _process.StartInfo = processInfo;
        }

        public CommandRunner SetWorkingDirectory(string directoryPath) {
            _process.StartInfo.WorkingDirectory = directoryPath;
            return this;
        }

        public CommandOutput RunCommand(IEnumerable<string> args) {
            var processArgs = args.Count() == 1
                ? args.First()
                : string.Join(" ", args);
            _process.StartInfo.Arguments = processArgs;
            _process.Start();
            string output = _process.StandardOutput.ReadToEnd().Trim();
            //output = output + Environment.NewLine + _gitProcess.StandardError.ReadToEnd().Trim();
            _process.WaitForExit();
            return new CommandOutput {ExitCode = _process.ExitCode, Output = output};
        }
        public CommandOutput RunCommand(params string[] args) {
            return RunCommand(args.ToList());
        }

        private bool _disposed;
        private readonly Process _process;

    }
}
