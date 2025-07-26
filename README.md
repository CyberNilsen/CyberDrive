# 🔒 CyberDrive - Encrypted Virtual Disk Manager

A modern, secure virtual disk encryption tool for Windows that creates and manages encrypted virtual hard drives (VHDs) with AES-256 encryption and secure file deletion capabilities.

<img width="634" height="1027" alt="CyberDrive Interface" src="https://github.com/user-attachments/assets/9a5c2b94-d276-467a-8ff6-ab89bfe51c9b" />
## ✨ Features

- **🔐 AES-256 Encryption** - Military-grade encryption using PBKDF2 with 100,000 iterations
- **💾 Virtual Hard Drives** - Create expandable VHD files that mount as regular drives
- **🗑️ Secure File Deletion** - Multi-pass secure deletion with cryptographically random data
- **🎨 Modern UI** - Clean, dark-themed interface with gradient buttons and smooth animations
- **⚡ Fast Operations** - Optimized for quick vault creation and mounting
- **🔒 Password Protection** - Strong password-based vault security

## 🚀 Quick Start

### Prerequisites

- Windows 10/11
- .NET Framework 4.7.2 or higher
- **Administrator privileges** (required for VHD operations)

### Usage

1. **Create a Vault**
   - Enter a unique vault name
   - Set a strong password
   - Click "🔒 Create Encrypted Vault"

2. **Unlock a Vault**
   - Enter the vault name and password
   - Click "🔓 Unlock Vault"
   - The vault will appear as a new drive letter

3. **Lock a Vault**
   - Click "🔐 Lock Vault" to safely unmount
   - All data is automatically encrypted

4. **Check Mounted Vaults**
   - View all currently mounted CyberDrive vaults
   - See available space for each vault

## 🏗️ Building from Source

### Requirements

- Visual Studio 2019 or later
- .NET Framework 4.7.2 SDK
- Windows SDK

### Build Steps

```bash
git clone https://github.com/yourusername/cyberdrive.git
cd cyberdrive
```

Open `CyberDrive.sln` in Visual Studio and build the solution (Ctrl+Shift+B).

## 🔧 Technical Details

### Architecture

- **VHD Management** - Uses Windows `diskpart` utility for virtual disk operations
- **Encryption** - AES-256-CBC with PBKDF2-SHA256 key derivation
- **File System** - NTFS with "CyberDriveVault" volume label
- **Secure Deletion** - 3-pass random data overwrite by default

### File Structure

```
CyberDrive/
├── VhdManager.cs      # Virtual disk creation and mounting
├── CryptoUtils.cs     # AES encryption/decryption utilities  
├── SecureDelete.cs    # Secure file deletion implementation
├── MainWindow.xaml    # Modern WPF user interface
└── MainWindow.xaml.cs # Application logic and event handlers
```

### Security Features

- **PBKDF2** key stretching with 100,000 iterations
- **Random salt** generation for each encryption operation
- **Secure random IV** for each encrypted file
- **Memory protection** for sensitive operations
- **Multi-pass deletion** to prevent data recovery

## ⚠️ Important Security Notes

- **Administrator Rights**: Required for all VHD operations (create, mount, unmount)
- **Password Strength**: Use strong, unique passwords for each vault
- **Backup**: Keep secure backups of important vaults
- **Memory**: Sensitive data may remain in memory - reboot after handling critical data

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Guidelines

- Follow C# coding conventions
- Add XML documentation for public methods
- Include unit tests for new functionality
- Maintain backward compatibility

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

**⚠️ Disclaimer**: This software is provided as-is. Always test thoroughly and maintain backups of important data. The developers are not responsible for any data loss or security breaches.
