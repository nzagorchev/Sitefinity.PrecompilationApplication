using System;
using System.IO;

namespace PrecompilationApplication
{
    public class PrecompilationCommandExecutor
    {
        public PrecompilationCommandExecutor(string batFilePath, string workingDirectory, string errorLogPath = null, IRunner runner = null)
        {
            if (runner != null)
            {
                this.Runner = runner;
            }
            else
            {
                this.Runner = new Runner();
            }

            this.BatFilePath = batFilePath;
            this.WorkingDirectory = workingDirectory;
            this.ErrorLogPath = errorLogPath;
        }

        public string ReadErrorLogIfExists()
        {
            if (!string.IsNullOrEmpty(this.ErrorLogPath))
            {
                if (File.Exists(this.ErrorLogPath))
                {
                    using (StreamReader sr = new StreamReader(ErrorLogPath))
                    {
                        string line = sr.ReadToEnd();
                        return line;
                    }
                }
            }

            return string.Empty;
        }

        public virtual void Execute()
        {
            // clear error.log before start 
            // needs permissions
            if (!string.IsNullOrEmpty(this.ErrorLogPath))
            {
                File.Delete(this.ErrorLogPath);
            }

            this.Runner.PrecompilationCompleted += Runner_PrecompilationCompleted;
            this.Runner.PrecompilationSucceeded += Runner_PrecompilationSucceeded;
            this.Runner.PrecompilationFailed += Runner_PrecompilationFailed;

           this.Runner.RunExternalExe(BatFilePath, null, WorkingDirectory);
        }

        public virtual void Runner_PrecompilationFailed(Exception ex, string output)
        {
            output += ReadErrorLogIfExists();
            Console.WriteLine("Failed!!!" + output);
        }

        public virtual void Runner_PrecompilationSucceeded(string output)
        {
            Console.WriteLine("Success!!!" + output);
        }

        public virtual void Runner_PrecompilationCompleted(int exitCode, string output)
        {
            output += ReadErrorLogIfExists();
            Console.WriteLine("Completed!!!" + output);
        }

        public IRunner Runner { get; set; }

        public string ErrorLogPath { get; set; }

        public string WorkingDirectory { get; set; }

        public string BatFilePath { get; set; }
    }
}
