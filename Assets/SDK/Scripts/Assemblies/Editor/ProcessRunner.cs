using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ThunderRoad
{
    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
        public bool Success => ExitCode == 0 && string.IsNullOrEmpty(Error);

        public override string ToString()
        {
            if (Success) return Output;
            return $"Process failed (exit {ExitCode}): {Error}";
        }
    }

    public static class ProcessRunner
    {
        public static async void RunProcessAsync(string processName, string arguments,
            Action<string> onOutputLine = null,
            Action<string> onErrorLine = null,
            Action<ProcessResult> onComplete = null,
            bool hidden = true, bool shellExecute = false, bool logToConsole = true)
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = processName,
                Arguments = arguments,
                WindowStyle = hidden ? System.Diagnostics.ProcessWindowStyle.Hidden : System.Diagnostics.ProcessWindowStyle.Normal,
                CreateNoWindow = hidden,
                UseShellExecute = shellExecute,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            // Thread-safe queues for streaming lines back to main thread
            var outputLines = new ConcurrentQueue<string>();
            var errorLines = new ConcurrentQueue<string>();
            bool finished = false;

            // Pump queues on main thread via EditorApplication.update
            void PumpQueue()
            {
                while (outputLines.TryDequeue(out string line))
                {
                    onOutputLine?.Invoke(line);
                }
                while (errorLines.TryDequeue(out string errLine))
                {
                    onErrorLine?.Invoke(errLine);
                }
                if (finished)
                {
                    EditorApplication.update -= PumpQueue;
                }
            }

            if (onOutputLine != null || onErrorLine != null)
            {
                EditorApplication.update += PumpQueue;
            }

            var result = await Task.Run(() =>
            {
                var stdout = new StringBuilder();
                var stderr = new StringBuilder();
                int exitCode = -1;
                try
                {
                    using var process = new System.Diagnostics.Process()
                    {
                        StartInfo = processInfo
                    };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null) return;
                        stdout.AppendLine(e.Data);
                        outputLines.Enqueue(e.Data);
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null) return;
                        stderr.AppendLine(e.Data);
                        errorLines.Enqueue(e.Data);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                }
                catch (Exception ex)
                {
                    stderr.AppendLine(ex.Message);
                }

                return new ProcessResult
                {
                    ExitCode = exitCode,
                    Output = stdout.ToString().Trim(),
                    Error = stderr.ToString().Trim()
                };
            });

            finished = true;

            // Final pump to catch any remaining lines
            while (outputLines.TryDequeue(out string remaining))
                onOutputLine?.Invoke(remaining);
            while (errorLines.TryDequeue(out string remaining))
                onErrorLine?.Invoke(remaining);

            if (logToConsole)
            {
                // Log to Unity console
                if (!string.IsNullOrEmpty(result.Output))
                {
                    foreach (var line in result.Output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                        Debug.Log($"[{processName}] {line}");
                }
                if (!string.IsNullOrEmpty(result.Error))
                {
                    foreach (var line in result.Error.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                        Debug.LogError($"[{processName}] {line}");
                }
            }

            onComplete?.Invoke(result);
        }

        public static ProcessResult RunProcessSync(string processName, string arguments,
            bool hidden = true, bool shellExecute = false, bool logToConsole = true)
        {
            try
            {
                using var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = processName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return new ProcessResult
                {
                    ExitCode = process.ExitCode,
                    Output = output.Trim(),
                    Error = error.Trim()
                };
            }
            catch (Exception ex)
            {
                return new ProcessResult
                {
                    ExitCode = -1,
                    Output = string.Empty,
                    Error = ex.Message
                };
            }
        }
    }
}
