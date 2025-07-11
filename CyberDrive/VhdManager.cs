using System.Diagnostics;

namespace CyberDrive
{
    public class VhdManager
    {
        public static void CreateAndFormatVHD(string path, string label = "CyberDriveVault", string size = "500MB")
        {
            string command = $@"
New-VHD -Path '{path}' -SizeBytes {size} -Dynamic | Mount-VHD
$disk = Get-Disk | Where-Object PartitionStyle -Eq 'RAW'
Initialize-Disk -Number $disk.Number -PartitionStyle MBR -PassThru |
    New-Partition -UseMaximumSize -AssignDriveLetter |
    Format-Volume -FileSystem NTFS -NewFileSystemLabel '{label}' -Confirm:$false
Dismount-VHD -Path '{path}'
";

            RunPowerShell(command);
        }

        public static void MountVHD(string path)
        {
            RunPowerShell($"Mount-VHD -Path \"{path}\"");
        }

        public static void UnmountVHD(string path)
        {
            RunPowerShell($"Dismount-VHD -Path \"{path}\"");
        }

        public static void RunPowerShell(string command)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
    }
}
