using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace ThunderRoad
{
    public class AdbResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
        public bool Success => ExitCode == 0 && string.IsNullOrEmpty(Error);

        public override string ToString()
        {
            if (Success) return Output;
            return $"ADB failed (exit {ExitCode}): {Error}";
        }
    }

    public class AdbHelper
    {
        private readonly string _adbPath;

        public AdbHelper(string adbPath)
        {
            this._adbPath = adbPath;
        }

        /// <summary>
        /// Runs an adb command asynchronously with streaming output.
        /// onOutputLine is called on the main thread (via EditorApplication.update) for each stdout line as it arrives.
        /// onErrorLine is called on the main thread for each stderr line as it arrives.
        /// onComplete is called on the main thread when the process finishes.
        /// </summary>
        private async void RunAdbCommandAsync(string arguments,
            Action<string> onOutputLine = null,
            Action<string> onErrorLine = null,
            Action<AdbResult> onComplete = null)
        {
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
                    UnityEditor.EditorApplication.update -= PumpQueue;
                }
            }

            if (onOutputLine != null || onErrorLine != null)
            {
                UnityEditor.EditorApplication.update += PumpQueue;
            }

            var result = await Task.Run(() =>
            {
                var stdout = new StringBuilder();
                var stderr = new StringBuilder();
                int exitCode = -1;
                try
                {
                    using var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = _adbPath;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

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

                return new AdbResult
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

            // Log to Unity console
            if (!string.IsNullOrEmpty(result.Output))
            {
                foreach (var line in result.Output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    UnityEngine.Debug.Log($"[adb] {line}");
            }
            if (!string.IsNullOrEmpty(result.Error))
            {
                foreach (var line in result.Error.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    UnityEngine.Debug.LogError($"[adb] {line}");
            }

            onComplete?.Invoke(result);
        }

        /// <summary>
        /// Runs an adb command synchronously. Blocks the calling thread.
        /// Use sparingly (e.g. quick checks like listing devices before export).
        /// </summary>
        public AdbResult RunCommandSync(string arguments)
        {
            try
            {
                using var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = _adbPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return new AdbResult
                {
                    ExitCode = process.ExitCode,
                    Output = output.Trim(),
                    Error = error.Trim()
                };
            }
            catch (Exception ex)
            {
                return new AdbResult
                {
                    ExitCode = -1,
                    Output = string.Empty,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Checks if any Android devices are connected via adb. Synchronous.
        /// Returns true if at least one device is found.
        /// </summary>
        public bool CheckDevicesConnected(out string deviceList)
        {
            var result = RunCommandSync("devices");
            deviceList = result.Output;
            if (!result.Success)
                return false;

            // Parse "adb devices" output: first line is header, subsequent non-empty lines are devices
            var lines = result.Output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int deviceCount = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("List of devices")) continue;
                if (!string.IsNullOrWhiteSpace(line))
                    deviceCount++;
            }
            return deviceCount > 0;
        }

        public void ListDevicesAsync(Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<AdbResult> onComplete = null)
        {
            RunAdbCommandAsync("devices", onOutputLine, onErrorLine, onComplete);
        }

        public void PushAsync(string localPath, string remotePath, Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<AdbResult> onComplete = null)
        {
            RunAdbCommandAsync($"push \"{localPath}\" \"{remotePath}\"", onOutputLine, onErrorLine, onComplete);
        }

        public void ShellAsync(string command, Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<AdbResult> onComplete = null)
        {
            RunAdbCommandAsync($"shell {command}", onOutputLine, onErrorLine, onComplete);
        }

        public void StopAppAsync(string packageName, Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<AdbResult> onComplete = null)
        {
            RunAdbCommandAsync($"shell am force-stop {packageName}", onOutputLine, onErrorLine, onComplete);
        }

        public void StartAppAsync(string packageName, string activity, string args = "", Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<AdbResult> onComplete = null)
        {
            RunAdbCommandAsync($"shell am start -n {packageName}/{activity} --es args '{args}'", onOutputLine, onErrorLine, onComplete);
        }
    }

}
