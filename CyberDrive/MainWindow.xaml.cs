using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace CyberDrive
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string vaultEncrypted = "C:\\CyberDrive\\vault_encrypted.vhd";
        private string vaultDecrypted = "C:\\CyberDrive\\vault_decrypted.vhd";

        private void CreateVault_Click(object sender, RoutedEventArgs e)
        {
            var password = PasswordInput.Password;
            Directory.CreateDirectory("C:\\CyberDrive");

            // Use PowerShell to create VHD
            var tempVhd = "C:\\CyberDrive\\temp.vhd";
            VhdManager.RunPowerShell($"New-VHD -Path '{tempVhd}' -SizeBytes 500MB -Dynamic");

            // Encrypt
            CryptoUtils.EncryptFile(tempVhd, vaultEncrypted, password);
            File.Delete(tempVhd);
            MessageBox.Show("Encrypted vault created.");
        }

        private void UnlockVault_Click(object sender, RoutedEventArgs e)
        {
            var password = PasswordInput.Password;
            CryptoUtils.DecryptFile(vaultEncrypted, vaultDecrypted, password);
            VhdManager.MountVHD(vaultDecrypted);
            MessageBox.Show("Vault unlocked and mounted.");
        }

        private void LockVault_Click(object sender, RoutedEventArgs e)
        {
            VhdManager.UnmountVHD(vaultDecrypted);
            SecureDelete.ShredFile(vaultDecrypted);
            MessageBox.Show("Vault locked and securely wiped.");
        }

    }
}