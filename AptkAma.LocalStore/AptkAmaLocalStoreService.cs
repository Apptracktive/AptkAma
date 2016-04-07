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
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaLocalStoreService : IAptkAmaLocalStoreService
    {
        private readonly IAptkAmaPluginConfiguration _configuration;
        private readonly IAptkAmaLocalStorePluginConfiguration _localStoreConfiguration;
        private MobileServiceSQLiteStore _localStore;
        private readonly IMobileServiceClient _client;
        private Dictionary<Type, object> _localTableServices;

        public AptkAmaLocalStoreService(IAptkAmaLocalStorePluginConfiguration localStoreConfiguration, IAptkAmaDataService dataService)
        {
            _localStoreConfiguration = localStoreConfiguration;
            _configuration = ((AptkAmaDataService)dataService).Configuration;
            _client = ((AptkAmaDataService)dataService).Client;

            InitializationTask = InitializeAsync();
            Task.Run(async () => await InitializationTask);
        }

        public Task<bool> InitializationTask { get; }

        private async Task<bool> InitializeAsync()
        {
            if (!_client.SyncContext.IsInitialized)
            {
                _localTableServices = new Dictionary<Type, object>();

                // Init local store
                var fullPath = Path.Combine(_localStoreConfiguration.DatabaseFullPath, _localStoreConfiguration.DatabaseFileName);
                try
                {
                    _localStore = new MobileServiceSQLiteStore(fullPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("AptkAma: Unable to create database file {0}. Local store initialization terminated with error: {1}", fullPath, ex.Message);
                    return false;
                }
                
                // Get the list of tables
                List<Type> tableTypes;
                try
                {
                    tableTypes = _configuration.ModelAssembly.DefinedTypes.Where(typeInfo => typeof(ITableData).GetTypeInfo().IsAssignableFrom(typeInfo)).Select(typeInfo => typeInfo.AsType()).ToList();
                }
                catch (Exception)
                {
                    Debug.WriteLine("AptkAma: Unable to find any class inheriting ITableData or EntityData into {0}.", _configuration.ModelAssembly.FullName);
                    return false;
                }

                // Define local tables
                foreach (var tableType in tableTypes)
                {
                    GetType().GetTypeInfo().GetDeclaredMethod("DefineTable").MakeGenericMethod(tableType).Invoke(this, null);
                }

                var syncHandlerTypeInfo = _configuration.ModelAssembly.DefinedTypes.FirstOrDefault(typeInfo => typeof(IMobileServiceSyncHandler).GetTypeInfo().IsAssignableFrom(typeInfo));
                var syncHandler = syncHandlerTypeInfo != null ? (IMobileServiceSyncHandler)Activator.CreateInstance(syncHandlerTypeInfo.AsType()) : new MobileServiceSyncHandler();

                // Init local store
                try
                {
                    await _client.SyncContext.InitializeAsync(_localStore, syncHandler);
                }
                catch (Exception)
                {
                    Debug.WriteLine("AptkAma: Unable to initialize local store. Please refer to online documentation.");
                }
            }
            return _client.SyncContext.IsInitialized;
        }

        private void DefineTable<T>()
        {
            _localStore.DefineTable<T>();
        }

        public IAptkAmaLocalTableService<T> GetLocalTable<T>() where T : ITableData
        {
            object genericLocalTable;
            _localTableServices.TryGetValue(typeof(T), out genericLocalTable);
            if (genericLocalTable == null)
            {
                var localTable = new AptkAmaLocalTableService<T>(_localStoreConfiguration, _client, this);
                genericLocalTable = localTable;
                _localTableServices.Add(typeof(T), localTable);
            }

            return genericLocalTable as AptkAmaLocalTableService<T>;
        }

        public async Task PushAsync()
        {
            var cts = new CancellationTokenSource();
            await Task.WhenAny(InitializationTask, Task.Delay(_localStoreConfiguration.InitTimeout, cts.Token));

            if (InitializationTask.IsCompleted && _client.SyncContext.IsInitialized)
            {
                await _client.SyncContext.PushAsync(cts.Token);
            }
        }


        public long PendingOperations => _client.SyncContext.PendingOperations;
    }
}
