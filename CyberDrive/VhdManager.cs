using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberDrive
{
    public class DriveInfo
    {
        public string DriveLetter { get; set; }
        public double TotalSpaceGB { get; set; }
        public double FreeSpaceGB { get; set; }
        public long TotalSpaceBytes { get; set; }
        public long FreeSpaceBytes { get; set; }
    }

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

        public static async Task<DriveInfo> MountVHDAsync(string vhdPath)
        {
            var drivesBefore = System.IO.DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Fixed)
                .Select(d => d.Name.Substring(0, 1))
                .ToList();

            string diskpartScript = $@"
select vdisk file=""{vhdPath}""
attach vdisk
";

            await RunDiskpartAsync(diskpartScript);

            await Task.Delay(3000);

            var drivesAfter = System.IO.DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Fixed)
                .ToList();

            var newDrive = drivesAfter.FirstOrDefault(d =>
                !drivesBefore.Contains(d.Name.Substring(0, 1)) &&
                d.IsReady);

            if (newDrive != null)
            {
                return new DriveInfo
                {
                    DriveLetter = newDrive.Name,
                    TotalSpaceBytes = newDrive.TotalSize,
                    FreeSpaceBytes = newDrive.TotalFreeSpace,
                    TotalSpaceGB = newDrive.TotalSize / (1024.0 * 1024.0 * 1024.0),
                    FreeSpaceGB = newDrive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0)
                };
            }

            foreach (var drive in drivesAfter.Where(d => d.IsReady))
            {
                try
                {
                    if (drive.VolumeLabel == "CyberDriveVault")
                    {
                        return new DriveInfo
                        {
                            DriveLetter = drive.Name,
                            TotalSpaceBytes = drive.TotalSize,
                            FreeSpaceBytes = drive.TotalFreeSpace,
                            TotalSpaceGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0),
                            FreeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0)
                        };
                    }
                }
                catch { }
            }

            return new DriveInfo
            {
                DriveLetter = "Unknown",
                TotalSpaceGB = 0,
                FreeSpaceGB = 0,
                TotalSpaceBytes = 0,
                FreeSpaceBytes = 0
            };
        }

        public static async Task UnmountVHDAsync(string vhdPath)
        {
            string diskpartScript = $@"
select vdisk file=""{vhdPath}""
detach vdisk
";

            await RunDiskpartAsync(diskpartScript);
        }

        public static string[] GetMountedVaults()
        {
            var vaultDrives = new List<string>();

            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.IsReady && drive.VolumeLabel == "CyberDriveVault")
                    {
                        vaultDrives.Add($"{drive.Name} ({drive.TotalFreeSpace / (1024 * 1024 * 1024):F2} GB free)");
                    }
                }
                catch { }
            }

            return vaultDrives.ToArray();
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