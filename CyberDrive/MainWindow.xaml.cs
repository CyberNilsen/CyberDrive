using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CyberDrive
{
    public partial class MainWindow : Window
    {
        private string baseDir = @"C:\CyberDrive\";

        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(baseDir);
        }

        private async void CreateVault_Click(object sender, RoutedEventArgs e)
        {
            string vaultName = VaultNameInput.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(vaultName))
            {
                MessageBox.Show("Please enter a vault name.");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password.");
                return;
            }

            string tempVhd = Path.Combine(baseDir, "temp.vhd");
            string encryptedVhd = Path.Combine(baseDir, vaultName + "_encrypted.vhd");

            try
            {
                await VhdManager.CreateVHDAsync(tempVhd, 500);

                CryptoUtils.EncryptFile(tempVhd, encryptedVhd, password);

                File.Delete(tempVhd);

                MessageBox.Show($"Encrypted vault '{vaultName}' created successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);

                if (File.Exists(tempVhd))
                {
                    try { File.Delete(tempVhd); } catch { }
                }
            }
        }

        private async void UnlockVault_Click(object sender, RoutedEventArgs e)
        {
            string vaultName = VaultNameInput.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(vaultName))
            {
                MessageBox.Show("Please enter a vault name.");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password.");
                return;
            }

            string encryptedVhd = Path.Combine(baseDir, vaultName + "_encrypted.vhd");
            string decryptedVhd = Path.Combine(baseDir, vaultName + "_decrypted.vhd");

            try
            {
                if (!File.Exists(encryptedVhd))
                {
                    MessageBox.Show("Encrypted vault not found.");
                    return;
                }

                CryptoUtils.DecryptFile(encryptedVhd, decryptedVhd, password);

                var driveInfo = await VhdManager.MountVHDAsync(decryptedVhd);

                string message = $"✅ Vault '{vaultName}' unlocked successfully!\n\n" +
                               $"📁 Drive Letter: {driveInfo.DriveLetter}\n" +
                               $"💾 Available Space: {driveInfo.FreeSpaceGB:F2} GB\n" +
                               $"📊 Total Space: {driveInfo.TotalSpaceGB:F2} GB\n\n" +
                               $"You can now access your encrypted files at {driveInfo.DriveLetter}";

                MessageBox.Show(message, "Vault Unlocked", MessageBoxButton.OK, MessageBoxImage.Information);

                if (MessageBox.Show($"Would you like to open {driveInfo.DriveLetter} in Windows Explorer?",
                    "Open Drive?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", driveInfo.DriveLetter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);

                if (File.Exists(decryptedVhd))
                {
                    try { File.Delete(decryptedVhd); } catch { }
                }
            }
        }

        private async void LockVault_Click(object sender, RoutedEventArgs e)
        {
            string vaultName = VaultNameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(vaultName))
            {
                MessageBox.Show("Please enter a vault name.");
                return;
            }

            string decryptedVhd = Path.Combine(baseDir, vaultName + "_decrypted.vhd");

            try
            {
                if (!File.Exists(decryptedVhd))
                {
                    MessageBox.Show("No decrypted vault is currently mounted.");
                    return;
                }

                await VhdManager.UnmountVHDAsync(decryptedVhd);

                SecureDelete.ShredFile(decryptedVhd);

                MessageBox.Show($"✅ Vault '{vaultName}' locked and securely wiped.\n\n" +
                               "The encrypted drive has been unmounted and the temporary decrypted file has been securely deleted.",
                               "Vault Locked", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void CheckMountedVaults_Click(object sender, RoutedEventArgs e)
        {
            var mountedVaults = VhdManager.GetMountedVaults();

            if (mountedVaults.Length == 0)
            {
                MessageBox.Show("No CyberDrive vaults are currently mounted.",
                               "No Mounted Vaults", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string message = "📁 Currently mounted CyberDrive vaults:\n\n";
                foreach (var vault in mountedVaults)
                {
                    message += $"• {vault}\n";
                }

                MessageBox.Show(message, "Mounted Vaults", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}