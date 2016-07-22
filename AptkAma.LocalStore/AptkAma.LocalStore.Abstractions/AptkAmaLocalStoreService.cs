using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaLocalStoreService : IAptkAmaLocalStoreService
    {
        private readonly string _rootPath;
        private readonly IAptkAmaLocalStorePluginConfiguration _localStoreConfiguration;
        private readonly IAptkAmaPluginConfiguration _configuration;
        private MobileServiceSQLiteStore _localStore;
        private readonly IMobileServiceClient _client;
        private Dictionary<Type, object> _localTables;

        public AptkAmaLocalStoreService(string rootPath, IAptkAmaLocalStorePluginConfiguration localStoreConfiguration, IAptkAmaDataService dataService)
        {
            _rootPath = rootPath;
            _localStoreConfiguration = localStoreConfiguration;
            _configuration = ((AptkAmaDataService)dataService).Configuration;
            _client = ((AptkAmaDataService)dataService).Client;

            Initialize();
        }

        private void Initialize()
        {
            _localTables = new Dictionary<Type, object>();

            // Init local store
            var basePath = string.IsNullOrEmpty(_localStoreConfiguration.DatabaseShortPath) ? _rootPath : Path.Combine(_rootPath, _localStoreConfiguration.DatabaseShortPath);
            var fullPath = Path.Combine(basePath, _localStoreConfiguration.DatabaseFileName);
            try
            {
                _localStore = new MobileServiceSQLiteStore(fullPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"AptkAma: Unable to create database file {fullPath}. Initialization terminated with error: {ex.Message}");
            }

            // Get the list of tables
            List<Type> tableTypes;
            try
            {
                tableTypes = _configuration.ModelAssembly.DefinedTypes.Where(typeInfo => typeof(ITableData).GetTypeInfo().IsAssignableFrom(typeInfo)).Select(typeInfo => typeInfo.AsType()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"AptkAma: Unable to find any class inheriting ITableData or EntityData into {_configuration.ModelAssembly.FullName}. Initialization terminated with error: {ex.Message}");
            }

            // Define local tables
            foreach (var tableType in tableTypes)
            {
                GetType().GetTypeInfo().GetDeclaredMethod("GetLocalTable").MakeGenericMethod(tableType).Invoke(this, null);
            }
        }

        public IAptkAmaLocalTableService<T> GetLocalTable<T>() where T : ITableData
        {
            object genericLocalTable;
            _localTables.TryGetValue(typeof(T), out genericLocalTable);
            if (genericLocalTable == null)
            {
                _localStore.DefineTable<T>();
                var syncTable = _client.GetSyncTable<T>();
                var localTable = new AptkAmaLocalTableService<T>(_localStoreConfiguration, _localStore, syncTable);
                genericLocalTable = localTable;
                _localTables.Add(typeof(T), localTable);

                _localStoreConfiguration.FileStoreService?.InitializeFileSyncContext(_client, _localStore, localTable);
            }

            return genericLocalTable as AptkAmaLocalTableService<T>;
        }

        public async Task PushAsync()
        {
            var cts = new CancellationTokenSource();
            await PushAsync(cts.Token);
        }

        public async Task PushAsync(CancellationToken token)
        {
            if (!_client.SyncContext.IsInitialized)
                await _client.SyncContext.InitializeAsync(_localStore, _localStoreConfiguration.SyncHandler);

            await _client.SyncContext.PushAsync(token);
        }
        
        public long PendingOperations => _client.SyncContext.PendingOperations;
    }
}
