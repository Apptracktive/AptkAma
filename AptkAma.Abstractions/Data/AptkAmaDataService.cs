using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// Service to manage data
    /// </summary>
    public class AptkAmaDataService : IAptkAmaDataService
    {
        public readonly IAptkAmaPluginConfiguration Configuration;
        public readonly IMobileServiceClient Client;
        private Dictionary<Type, object> _remoteTables;

        public AptkAmaDataService(IAptkAmaPluginConfiguration configuration, IMobileServiceClient client)
        {
            Configuration = configuration;
            Client = client;

            // Init tables
            Initialize();
        }

        private void Initialize()
        {
            _remoteTables = new Dictionary<Type, object>();

            // Get the list of tables
            List<Type> tableTypes;
            try
            {
                tableTypes = Configuration.ModelAssembly.DefinedTypes.Where(type => typeof(ITableData).GetTypeInfo().IsAssignableFrom(type)).Select(t => t.AsType()).ToList();
            }
            catch (Exception)
            {
                throw new Exception($"AptkAma: Unable to find any class inheriting ITableData or EntityData into {Configuration.ModelAssembly.FullName}.");
            }

            // Get tables
            foreach (var tableType in tableTypes)
            {
                // Get remote table
                GetType().GetTypeInfo().GetDeclaredMethod("RemoteTable").MakeGenericMethod(tableType).Invoke(this, null);
            }
        }

        /// <summary>
        /// Service to manage remote Azure data
        /// </summary>
        /// <typeparam name="T">Data table to manage (model class)</typeparam>
        /// <returns></returns>
        public IAptkAmaRemoteTableService<T> RemoteTable<T>() where T : ITableData
        {
            object genericRemoteTable;
            _remoteTables.TryGetValue(typeof(T), out genericRemoteTable);
            if (genericRemoteTable == null)
            {
                var table = Client.GetTable<T>();
                var remoteTable = new AptkAmaRemoteTableService<T>(table);
                genericRemoteTable = remoteTable;
                _remoteTables.Add(typeof(T), remoteTable);
            }

            return genericRemoteTable as AptkAmaRemoteTableService<T>;
        }
    }
}
