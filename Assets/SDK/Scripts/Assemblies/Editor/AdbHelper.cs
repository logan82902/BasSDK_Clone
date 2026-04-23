using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace ThunderRoad
{
    public class AdbHelper
    {
        private readonly string _adbPath;

        public AdbHelper(string adbPath)
        {
            this._adbPath = adbPath;
        }

        /// <summary>
        /// Runs an adb command synchronously. Blocks the calling thread.
        /// Use sparingly (e.g. quick checks like listing devices before export).
        /// </summary>
        public ProcessResult RunCommandSync(string arguments) => ProcessRunner.RunProcessSync(_adbPath, arguments);

        /// <summary>
        /// Checks if any Android devices are connected via adb. Synchronous.
        /// Returns true if at least one device is found.
        /// </summary>
        public bool CheckDevicesConnected(out string deviceList)
        {
            var result = ProcessRunner.RunProcessSync(_adbPath, "devices");
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

        public void ListDevicesAsync(Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<ProcessResult> onComplete = null)
        {
            ProcessRunner.RunProcessAsync(_adbPath, "devices", onOutputLine, onErrorLine, onComplete);
        }

        public void PushAsync(string localPath, string remotePath, Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<ProcessResult> onComplete = null)
        {
            ProcessRunner.RunProcessAsync(_adbPath, $"push \"{localPath}\" \"{remotePath}\"", onOutputLine, onErrorLine, onComplete);
        }

        public void ShellAsync(string command, Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<ProcessResult> onComplete = null)
        {
            ProcessRunner.RunProcessAsync(_adbPath, $"shell {command}", onOutputLine, onErrorLine, onComplete);
        }

        public void StopAppAsync(string packageName, Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<ProcessResult> onComplete = null)
        {
            ProcessRunner.RunProcessAsync(_adbPath, $"shell am force-stop {packageName}", onOutputLine, onErrorLine, onComplete);
        }

        public void StartAppAsync(string packageName, string activity, string args = "", Action<string> onOutputLine = null, Action<string> onErrorLine = null, Action<ProcessResult> onComplete = null)
        {
            ProcessRunner.RunProcessAsync(_adbPath, $"shell am start -n {packageName}/{activity} --es args '{args}'", onOutputLine, onErrorLine, onComplete);
        }
    }

}
