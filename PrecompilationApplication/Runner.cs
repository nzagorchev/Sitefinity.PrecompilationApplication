using System;
using System.Diagnostics;
using System.Text;

namespace PrecompilationApplication
{
    public class Runner : IRunner
    {
        #region Events

        public event OnPrecompilationCompleted PrecompilationCompleted;

        public event OnPrecompilationFailed PrecompilationFailed;

        public event OnPrecompilationSucceeded PrecompilationSucceeded;

        #endregion

        public void RunExternalExe(string filename, string arguments = null, string workingDirectory = null)
        {
            try
            {
                var process = new Process();

                process.StartInfo.FileName = filename;
                if (!string.IsNullOrEmpty(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }

                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;

                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                this.stdOutput = new StringBuilder();
                process.OutputDataReceived += OnOutputDataReceived;

                string stdError = null;
                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    stdError = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    
                }
                catch (Exception ex)
                {
                    if (stdError == null)
                    {
                        stdError = ex.ToString();
                    }
                    stdOutput.AppendLine(stdError.ToString());
                    throw new Exception("OS error while executing " + Runner.Format(filename, arguments) + ": " + ex.Message, ex);
                }

                this.ExitCode = process.ExitCode;

                if (process.ExitCode == 0)
                {
                    // Succeeded
                    if (PrecompilationSucceeded != null)
                    {
                        PrecompilationSucceeded(stdOutput.ToString());
                    }
                }
                else
                {
                    var message = new StringBuilder();

                    if (!string.IsNullOrEmpty(stdError))
                    {
                        message.AppendLine(stdError);
                    }

                    if (stdOutput.Length != 0)
                    {
                        message.AppendLine("Std output:");
                        message.AppendLine(stdOutput.ToString());
                    }

                    // Failed Event
                    if (PrecompilationFailed != null)
                    {
                        PrecompilationFailed(null, message.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                stdOutput.AppendLine(ex.ToString());
                // Failed Event
                if (PrecompilationFailed != null)
                {
                    PrecompilationFailed(ex, stdOutput.ToString());
                }
            }
            finally
            {
                // Completed Event
                if (PrecompilationCompleted != null)
                {
                    PrecompilationCompleted(this.ExitCode, stdOutput.ToString());
                }
            }
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            this.stdOutput.Append(args.Data);
        }

        private static string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }

        public StringBuilder stdOutput { get; set; }

        public int ExitCode { get; set; }

        public Exception ExceptionCaught { get; set; }
    }
}
