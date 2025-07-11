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

                char driveLetter = await VhdManager.MountVHDAsync(decryptedVhd);

                MessageBox.Show($"Vault unlocked and mounted to drive {driveLetter}:");
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

                MessageBox.Show("Vault locked and securely wiped.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}