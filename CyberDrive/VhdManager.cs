using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberDrive
{
    public static class VhdManager
    {
        public static async Task CreateVHDAsync(string vhdPath, int sizeInMB)
        {
            string diskpartScript = $@"
create vdisk file=""{vhdPath}"" maximum={sizeInMB} type=expandable
select vdisk file=""{vhdPath}""
attach vdisk
create partition primary
active
format fs=ntfs label=""CyberDriveVault"" quick
assign
detach vdisk
";

            await RunDiskpartAsync(diskpartScript);
        }

        public static async Task<char> MountVHDAsync(string vhdPath)
        {
            string diskpartScript = $@"
select vdisk file=""{vhdPath}""
attach vdisk
";

            await RunDiskpartAsync(diskpartScript);

            await Task.Delay(2000);

            return await GetVHDDriveLetterAsync(vhdPath);
        }

        public static async Task UnmountVHDAsync(string vhdPath)
        {
            string diskpartScript = $@"
select vdisk file=""{vhdPath}""
detach vdisk
";

            await RunDiskpartAsync(diskpartScript);
        }

        private static async Task<char> GetVHDDriveLetterAsync(string vhdPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "logicaldisk get size,freespace,caption",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Contains(":") && !line.Contains("Caption"))
                {
                    var parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0 && parts[0].Length >= 2)
                    {
                        return parts[0][0];
                    }
                }
            }

            return 'Z'; 
        }

        private static async Task RunDiskpartAsync(string script)
        {
            var tempScriptFile = Path.GetTempFileName();

            try
            {
                await File.WriteAllTextAsync(tempScriptFile, script);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "diskpart",
                    Arguments = $"/s \"{tempScriptFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Diskpart failed with exit code {process.ExitCode}. Error: {error}. Output: {output}");
                }
            }
            finally
            {
                try
                {
                    File.Delete(tempScriptFile);
                }
                catch { }
            }
        }
    }
}