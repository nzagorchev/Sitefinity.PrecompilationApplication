using System;

namespace PrecompilationApplication
{
    public class Program
    {
        const string BatFilePath = @"D:\Sitefinity 8.0\Telerik.Sitefinity.Compiler\compileWithCommands.bat";
        const string ErrorLogPath = @"D:\Sitefinity 8.0\Telerik.Sitefinity.Compiler\Error.log";
        const string WorkingDirectory = @"D:\Sitefinity 8.0\Telerik.Sitefinity.Compiler\";

        public static void Main(string[] args)
        {
            var executor = new PrecompilationCommandExecutor(BatFilePath, WorkingDirectory, ErrorLogPath);
            executor.Execute();
            Console.ReadLine();
        }
    }
}
