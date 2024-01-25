using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PartsManager.BaseHandlers
{
    public static class JsonBackupHelper
    {
        public static void Backup(string backupName)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory + "jsonBackups";

            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, backupName);
            path = Path.ChangeExtension(path, "json");

            var databaseBackup = new DatabaseBackup();
            string jsonString = JsonSerializer.Serialize(databaseBackup);

            File.WriteAllText(path, jsonString);
        }

        public static void Restore(string path)
        {
            var jsonString = File.ReadAllText(path);
            var databaseBackup = JsonSerializer.Deserialize<DatabaseBackup>(jsonString);

            EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");           
            var tableNames = new List<string>()
            {                
                "PartManufacturerStandards",
                "PartApiStandards",
                "AdditionalInfoes",
                "SaeQualityStandards",
                "ManufacturerStandards",
                "Manufacturers",
                "ApiStandards",
                "Payments",
                "InvoiceParts",
                "Invoices",
                "Parts",
                "PartTypes",
                "Cars",
                "Models",
                "Marks",
            };
            SqlConnection connection = new SqlConnection(unitOfWork.Db.Database.Connection.ConnectionString);
            ServerConnection serverConnection = new ServerConnection(connection);
            try
            {
                var server = new Server(serverConnection);

                serverConnection.BeginTransaction();
                foreach (var tableName in tableNames)
                {
                    server.ConnectionContext.ExecuteNonQuery($"DELETE FROM [{tableName}] DBCC CHECKIDENT ([{tableName}], RESEED, 0)");
                }
                serverConnection.CommitTransaction();
            }
            catch
            {
                serverConnection.RollBackTransaction();
                return;
            }

            unitOfWork.Reload();

            unitOfWork.Db.Marks.AddRange(databaseBackup.Marks);
            unitOfWork.Save();
            unitOfWork.Db.Models.AddRange(databaseBackup.Models);
            unitOfWork.Save();
            unitOfWork.Db.Cars.AddRange(databaseBackup.Cars);
            unitOfWork.Save();
            unitOfWork.Db.PartTypes.AddRange(databaseBackup.PartTypes);
            unitOfWork.Save();
            unitOfWork.Db.Parts.AddRange(databaseBackup.Parts);
            unitOfWork.Save();
            unitOfWork.Db.Invoices.AddRange(databaseBackup.Invoices);
            unitOfWork.Save();
            unitOfWork.Db.InvoiceParts.AddRange(databaseBackup.InvoiceParts);
            unitOfWork.Save();
            unitOfWork.Db.Payments.AddRange(databaseBackup.Payments);
            unitOfWork.Save();
            unitOfWork.Db.ApiStandards.AddRange(databaseBackup.ApiStandards);
            unitOfWork.Save();
            unitOfWork.Db.Manufacturers.AddRange(databaseBackup.Manufacturers);
            unitOfWork.Save();
            unitOfWork.Db.ManufacturerStandards.AddRange(databaseBackup.ManufacturerStandards);
            unitOfWork.Save();
            unitOfWork.Db.SaeQualityStandards.AddRange(databaseBackup.SaeQualityStandards);
            unitOfWork.Save();
            unitOfWork.Db.AdditionalInfos.AddRange(databaseBackup.AdditionalInfos);
            unitOfWork.Save();
            unitOfWork.Db.PartApiStandards.AddRange(databaseBackup.PartApiStandards);
            unitOfWork.Save();
            unitOfWork.Db.PartManufacturerStandards.AddRange(databaseBackup.PartManufacturerStandards);
            unitOfWork.Save();
        }

        public static string ChooseRestore()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            var directory = AppDomain.CurrentDomain.BaseDirectory + "jsonBackups";
            openFileDialog.InitialDirectory = directory;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
