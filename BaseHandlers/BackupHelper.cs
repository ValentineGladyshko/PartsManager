using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartsManager.BaseHandlers
{
    public static class BackupHelper
    {
        public static Backup CreateBackup(string dbName, string backupName) 
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory + "backups";

            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, backupName);
            path = Path.ChangeExtension(path, "bak");

            var backup = new Backup();
            backup.Action = BackupActionType.Database;
            backup.Database = dbName;
            backup.Devices.AddDevice(path, DeviceType.File);
            backup.Initialize = true;
            backup.PercentCompleteNotification = 5;

            return backup;
        }

        public static Restore CreateRestore(string dbName, string path)
        {
            var restore = new Restore();
            restore.Database = dbName;
            restore.Action = RestoreActionType.Database;
            restore.Devices.AddDevice(path, DeviceType.File);
            restore.ReplaceDatabase = true;
            restore.PercentCompleteNotification = 5;

            return restore;
        }

        public static string ChooseRestore()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Backup files (*.bak)|*.bak";
            var directory = AppDomain.CurrentDomain.BaseDirectory + "backups";
            openFileDialog.InitialDirectory = directory;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public static bool? AskRestore(string path)
        {
            var message = $"Ви впевнені що хочете завантажити резервну копію:\n\"{path}\"?";
            var dialogWindow = new DialogWindow(message);
            return dialogWindow.ShowDialog();
        }

        public static bool CheckSameName(string name, string path)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory + "backups";

            var path2 = Path.Combine(directory, name);
            path2 = Path.ChangeExtension(path2, "bak");
            return path2 == path;
        }
    }
}
