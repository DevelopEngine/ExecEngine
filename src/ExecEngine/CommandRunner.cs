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
        public string ErrorOutput {get;set;}
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

        public CommandRunner(string cmdName, params string[] defaultArgs)
        {
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = cmdName,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _process = new Process();
            _process.StartInfo = processInfo;
            _baseArgs = defaultArgs?.ToList() ?? new List<string>();
        }

        public CommandRunner SetWorkingDirectory(string directoryPath) {
            _process.StartInfo.WorkingDirectory = directoryPath;
            return this;
        }

        public CommandOutput RunCommand(IEnumerable<string> args) {
            return RunDetached ? RunDetachedCommand(args) : RunAttachedCommand(args);
        }

        private CommandOutput RunAttachedCommand(IEnumerable<string> args) {
            args = _baseArgs.Concat(args);
            var processArgs = args.Count() == 1
                ? args.First()
                : string.Join(" ", args);
            _process.StartInfo.Arguments = processArgs;
            _process.Start();
            string output = _process.StandardOutput.ReadToEnd().Trim();
            string err = _process.StandardError.ReadToEnd().Trim();
            //output = output + Environment.NewLine + _gitProcess.StandardError.ReadToEnd().Trim();
            _process.WaitForExit();
            return new CommandOutput {ExitCode = _process.ExitCode, Output = output, ErrorOutput = err};
        }

        private CommandOutput RunDetachedCommand(IEnumerable<string> args) {
            args = _baseArgs.Concat(args);
            var processArgs = args.Count() == 1
                ? args.First()
                : string.Join(" ", args);
            _process.StartInfo.Arguments = processArgs;
            _process.StartInfo.UseShellExecute = true;
            _process.StartInfo.RedirectStandardOutput = false;
            _process.StartInfo.RedirectStandardError = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.Start();
            _process.WaitForExit();
            return new CommandOutput {ExitCode = _process.ExitCode, Output = null, ErrorOutput = null};
        }

        public CommandOutput RunCommand(params string[] args) {
            return RunCommand(args.ToList());
        }

        /// <summary>
        /// An optional name for this runner.
        /// </summary>
        /// <remarks>Not used or parsed anywhere. Use this for internal purposes.</remarks>
        /// <value>The name of the runner.</value>
        public string Name {get;set;}

        private bool _disposed;
        private readonly Process _process;
        private readonly List<string> _baseArgs;

        public bool RunDetached {get;set;} = false;
    }
}
