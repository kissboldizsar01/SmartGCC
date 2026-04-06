using System.Diagnostics;
using System.ComponentModel;
using SmartGCC.Models;

namespace SmartGCC.Modules;

public sealed class ProcessRunner
{
    /// <summary>
    /// Runs GCC with the provided process configuration.
    /// </summary>
    /// <param name="config">The process configuration containing GCC arguments and flags.</param>
    /// <param name="cancellationToken">A token used to cancel process execution.</param>
    /// <returns>A process result containing exit code, standard output and standard error.</returns>
    public async Task<ProcessResult> RunAsync(ProcessConfig config, CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "gcc",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in config.GccArgs)
        {
            startInfo.ArgumentList.Add(arg);
        }

        Process? process;
        try
        {
            process = Process.Start(startInfo);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("ERROR: 'gcc' was not found. Please install GCC and make sure it is in your PATH.");
            Environment.Exit(127);
            return new ProcessResult();
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2)
        {
            Console.WriteLine("ERROR: 'gcc' was not found. Please install GCC and make sure it is in your PATH.");
            Environment.Exit(127);
            return new ProcessResult();
        }

        if (process is null)
        {
            return new ProcessResult
            {
                ExitCode = -1,
                Stdout = string.Empty,
                Stderr = "Failed to start gcc process."
            };
        }

        using (process)
        {
            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                await process.WaitForExitAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                    }
                }
                catch
                {
                }

                var timedOutStdout = await stdoutTask;
                var timedOutStderr = await stderrTask;

                return new ProcessResult
                {
                    ExitCode = 124,
                    Stdout = timedOutStdout,
                    Stderr = string.IsNullOrWhiteSpace(timedOutStderr)
                        ? "GCC process timed out after 30 seconds."
                        : timedOutStderr
                };
            }

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            return new ProcessResult
            {
                ExitCode = process.ExitCode,
                Stdout = stdout,
                Stderr = stderr
            };
        }
    }
}
