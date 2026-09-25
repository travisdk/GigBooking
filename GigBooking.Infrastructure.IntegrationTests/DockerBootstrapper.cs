using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GigBooking.Infrastructure.IntegrationTests;

public static class DockerBootstrapper
{

    public static async Task EnsureDockerIsRunning(int timeoutSeconds = 60)
    {

        if (IsDockerDaemonRunning())
        {
            return;
        }

        var dockerDesktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
           @"Programs\DockerDesktop\Docker Desktop.exe");

        if (!File.Exists(dockerDesktopPath))
        {
            throw new InvalidOperationException($"Docker desktop not found at location path {dockerDesktopPath}");
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = dockerDesktopPath,
            UseShellExecute = true

        });

        var stopWatch = Stopwatch.StartNew();
        while (stopWatch.Elapsed < TimeSpan.FromSeconds(timeoutSeconds))
        {
            if (IsDockerDaemonRunning())
            {
                return;
            }
            await Task.Delay(2000);
        }
        throw new TimeoutException($"Docker Desktop did not start within {timeoutSeconds} seconds.");
    }
    private static bool IsDockerDaemonRunning()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "info",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            });

            process?.WaitForExit();
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
