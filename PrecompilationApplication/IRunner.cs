using System;

namespace PrecompilationApplication
{
    public delegate void OnPrecompilationCompleted(int exitCode, string output);
    public delegate void OnPrecompilationFailed(Exception ex, string output);
    public delegate void OnPrecompilationSucceeded(string output);

    public interface IRunner
    {
        event OnPrecompilationCompleted PrecompilationCompleted;

        event OnPrecompilationFailed PrecompilationFailed;

        event OnPrecompilationSucceeded PrecompilationSucceeded;

        void RunExternalExe(string filename, string arguments = null, string workingDirectory = null);
    }
}
