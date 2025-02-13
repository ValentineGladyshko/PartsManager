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
using System.Windows.Input;
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

        public static bool Restore(string path)
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
                "PartnerPayments",
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
                InsertToTable(databaseBackup.PartnerPayments, server, "PartnerPayments");
                serverConnection.CommitTransaction();
            }
            catch
            {
                serverConnection.RollBackTransaction();
                return false;
            }
            return true;
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
                var chunks = list.SplitIntoSets(950);
                var queries = new List<StringBuilder>();
                foreach (var chunk in chunks)
                {
                    var localList = chunk.ToList();
                    var query = new StringBuilder($"{localList[0].GetTable()}\n");
                    query.Append(localList[0].GetQuery());
                    for (int i = 1; i < localList.Count; i++)
                    {
                        query.Append(", \n");
                        query.Append(localList[i].GetQuery());
                    }
                    query.Append(";");
                    queries.Add(query);
                }
                server.ConnectionContext.ExecuteNonQuery($"SET IDENTITY_INSERT [{tableName}] ON");
                foreach (var query in queries)
                {
                    var t = query.ToString();

                    server.ConnectionContext.ExecuteNonQuery(query.ToString());
                }
                server.ConnectionContext.ExecuteNonQuery($"SET IDENTITY_INSERT [{tableName}] OFF");

                foreach (var query in queries)
                {
                    query.Clear();
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> SplitIntoSets<T> (this IEnumerable<T> source, int itemsPerSet)
        {
            var sourceList = source as List<T> ?? source.ToList();
            for (var index = 0; index < sourceList.Count; index += itemsPerSet)
            {
                yield return sourceList.Skip(index).Take(itemsPerSet);
            }
        }
    }
}

