using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json.Linq;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
                InsertToTable(databaseBackup.Marks, server, "Marks");
                InsertToTable(databaseBackup.Models, server, "Models");
                InsertToTable(databaseBackup.Cars, server, "Cars");
                InsertToTable(databaseBackup.PartTypes, server, "PartTypes");
                InsertToTable(databaseBackup.Parts, server, "Parts");
                InsertToTable(databaseBackup.Invoices, server, "Invoices");
                InsertToTable(databaseBackup.InvoiceParts, server, "InvoiceParts");
                InsertToTable(databaseBackup.Payments, server, "Payments");
                InsertToTable(databaseBackup.ApiStandards, server, "ApiStandards");
                InsertToTable(databaseBackup.Manufacturers, server, "Manufacturers");
                InsertToTable(databaseBackup.ManufacturerStandards, server, "ManufacturerStandards");
                InsertToTable(databaseBackup.SaeQualityStandards, server, "SaeQualityStandards");
                InsertToTable(databaseBackup.AdditionalInfos, server, "AdditionalInfoes");
                InsertToTable(databaseBackup.PartApiStandards, server, "PartApiStandards");
                InsertToTable(databaseBackup.PartManufacturerStandards, server, "PartManufacturerStandards");
                serverConnection.CommitTransaction();
            }
            catch
            {
                serverConnection.RollBackTransaction();
                return;
            }

            unitOfWork.Reload();

            //InsertToTable(unitOfWork.Db.Marks, databaseBackup.Marks, serverConnection, "Marks", unitOfWork);
            //InsertToTable(unitOfWork.Db.Models, databaseBackup.Models, serverConnection, "Models", unitOfWork);
            //InsertToTable(unitOfWork.Db.Cars, databaseBackup.Cars, serverConnection, "Cars", unitOfWork);
            //InsertToTable(unitOfWork.Db.PartTypes, databaseBackup.PartTypes, serverConnection, "PartTypes", unitOfWork);
            //InsertToTable(unitOfWork.Db.Parts, databaseBackup.Parts, serverConnection, "Parts", unitOfWork);
            //InsertToTable(unitOfWork.Db.Invoices, databaseBackup.Invoices, serverConnection, "Invoices", unitOfWork);
            //InsertToTable(unitOfWork.Db.InvoiceParts, databaseBackup.InvoiceParts, serverConnection, "InvoiceParts", unitOfWork);
            //InsertToTable(unitOfWork.Db.Payments, databaseBackup.Payments, serverConnection, "Payments", unitOfWork);
            //InsertToTable(unitOfWork.Db.ApiStandards, databaseBackup.ApiStandards, serverConnection, "ApiStandards", unitOfWork);
            //InsertToTable(unitOfWork.Db.Manufacturers, databaseBackup.Manufacturers, serverConnection, "Manufacturers", unitOfWork);
            //InsertToTable(unitOfWork.Db.ManufacturerStandards, databaseBackup.ManufacturerStandards, serverConnection, "ManufacturerStandards", unitOfWork);
            //InsertToTable(unitOfWork.Db.SaeQualityStandards, databaseBackup.SaeQualityStandards, serverConnection, "SaeQualityStandards", unitOfWork);
            //InsertToTable(unitOfWork.Db.AdditionalInfos, databaseBackup.AdditionalInfos, serverConnection, "AdditionalInfoes", unitOfWork);
            //InsertToTable(unitOfWork.Db.PartApiStandards, databaseBackup.PartApiStandards, serverConnection, "PartApiStandards", unitOfWork);
            //InsertToTable(unitOfWork.Db.PartManufacturerStandards, databaseBackup.PartManufacturerStandards, serverConnection, "PartManufacturerStandards", unitOfWork);

            //unitOfWork.Db.Marks.AddRange(databaseBackup.Marks);
            //unitOfWork.Save();
            //unitOfWork.Db.Models.AddRange(databaseBackup.Models);
            //unitOfWork.Save();
            //unitOfWork.Db.Cars.AddRange(databaseBackup.Cars);
            //unitOfWork.Save();
            //unitOfWork.Db.PartTypes.AddRange(databaseBackup.PartTypes);
            //unitOfWork.Save();
            //unitOfWork.Db.Parts.AddRange(databaseBackup.Parts);
            //unitOfWork.Save();
            //unitOfWork.Db.Invoices.AddRange(databaseBackup.Invoices);
            //unitOfWork.Save();
            //unitOfWork.Db.InvoiceParts.AddRange(databaseBackup.InvoiceParts);
            //unitOfWork.Save();
            //unitOfWork.Db.Payments.AddRange(databaseBackup.Payments);
            //unitOfWork.Save();
            //unitOfWork.Db.ApiStandards.AddRange(databaseBackup.ApiStandards);
            //unitOfWork.Save();
            //unitOfWork.Db.Manufacturers.AddRange(databaseBackup.Manufacturers);
            //unitOfWork.Save();
            //unitOfWork.Db.ManufacturerStandards.AddRange(databaseBackup.ManufacturerStandards);
            //unitOfWork.Save();
            //unitOfWork.Db.SaeQualityStandards.AddRange(databaseBackup.SaeQualityStandards);
            //unitOfWork.Save();
            //unitOfWork.Db.AdditionalInfos.AddRange(databaseBackup.AdditionalInfos);
            //unitOfWork.Save();
            //unitOfWork.Db.PartApiStandards.AddRange(databaseBackup.PartApiStandards);
            //unitOfWork.Save();
            //unitOfWork.Db.PartManufacturerStandards.AddRange(databaseBackup.PartManufacturerStandards);
            //unitOfWork.Save();
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

        static void InsertToTable<T>(List<T> list, Server server, string tableName) where T : IQuery
        {
            if (list != null && list.Count > 0)
            {
                var query = new StringBuilder($"{list[0].GetTable()}\n");
                query.Append(list[0].GetQuery());
                for (int i = 1; i < list.Count; i++)
                {
                    query.Append(", \n");

                    query.Append(list[i].GetQuery());
                }
                query.Append(";");
                var str = query.ToString();
                server.ConnectionContext.ExecuteNonQuery($"SET IDENTITY_INSERT [{tableName}] ON");
                server.ConnectionContext.ExecuteNonQuery(query.ToString());
                server.ConnectionContext.ExecuteNonQuery($"SET IDENTITY_INSERT [{tableName}] OFF");

                query.Clear();
            }
        }
    }
}

