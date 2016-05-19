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
        public readonly List<Type> TableTypes;
        private Dictionary<Type, object> _remoteTables;
        private bool _isInitilized;

        public AptkAmaDataService(IAptkAmaPluginConfiguration configuration, IMobileServiceClient client)
        {
            Configuration = configuration;
            Client = client;

            // Get the list of tables
            try
            {
                TableTypes = Configuration.ModelAssembly.DefinedTypes.Where(type => typeof(ITableData).GetTypeInfo().IsAssignableFrom(type)).Select(t => t.AsType()).ToList();
            }
            catch (Exception)
            {
                throw new Exception($"AptkAma: Unable to find any class inheriting ITableData or EntityData into {Configuration.ModelAssembly.FullName}.");
            }

            // Init tables
            Initialize();
        }

        private bool Initialize()
        {
            if (!_isInitilized)
            {
                _remoteTables = new Dictionary<Type, object>();

                // Get tables
                foreach (var tableType in TableTypes)
                {
                    // Get remote table
                    GetType().GetTypeInfo().GetDeclaredMethod("RemoteTable").MakeGenericMethod(tableType).Invoke(this, null);
                }

                _isInitilized = true;
            }
            return _isInitilized;
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
