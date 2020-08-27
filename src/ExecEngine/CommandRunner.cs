using System;
using System.Diagnostics;
using System.IO;

namespace ExecEngine
{
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

        public (int, string) RunCommand(string args)
        {
            //args = args.StartsWith("git ") ? args : $"git {args}";
            _process.StartInfo.Arguments = args;
            _process.Start();
            string output = _process.StandardOutput.ReadToEnd().Trim();
            //output = output + Environment.NewLine + _gitProcess.StandardError.ReadToEnd().Trim();
            _process.WaitForExit();
            return (_process.ExitCode, output);
        }

        private bool _disposed;
        private readonly Process _process;
        
    }
}
