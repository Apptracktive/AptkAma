using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaLocalStoreService : IAptkAmaLocalStoreService
    {
        private readonly IAptkAmaLocalStorePluginConfiguration _localStoreConfiguration;
        private readonly IAptkAmaPluginConfiguration _configuration;
        private readonly List<Type> _tableTypes;
        private readonly MobileServiceSQLiteStore _localStore;
        private readonly IMobileServiceClient _client;
        private Dictionary<Type, object> _localTables;

        public AptkAmaLocalStoreService(IAptkAmaLocalStorePluginConfiguration localStoreConfiguration, IAptkAmaDataService dataService)
        {
            _localStoreConfiguration = localStoreConfiguration;
            var aptkAmaDataService = (AptkAmaDataService) dataService;
            _configuration = aptkAmaDataService.Configuration;
            _tableTypes = aptkAmaDataService.TableTypes;
            _client = aptkAmaDataService.Client;
            _localStore = new MobileServiceSQLiteStore(Path.Combine(_localStoreConfiguration.DatabaseFullPath, _localStoreConfiguration.DatabaseFileName));

            InitializationTask = InitializeAsync();
            Task.Run(async () => await InitializationTask);
        }

        public Task<bool> InitializationTask { get; }

        private async Task<bool> InitializeAsync()
        {
            if (!_client.SyncContext.IsInitialized)
            {
                _localTables = new Dictionary<Type, object>();

                // Define local tables
                foreach (var tableType in _tableTypes)
                {
                    GetType().GetTypeInfo().GetDeclaredMethod("DefineTable").MakeGenericMethod(tableType).Invoke(this, null);
                    GetType().GetTypeInfo().GetDeclaredMethod("GetLocalTable").MakeGenericMethod(tableType).Invoke(this, null);
                }

                // Init local store
                await _client.SyncContext.InitializeAsync(_localStore, _localStoreConfiguration.SyncHandler);
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
            _localTables.TryGetValue(typeof(T), out genericLocalTable);
            if (genericLocalTable == null)
            {
                var syncTable = _client.GetSyncTable<T>();
                var localTable = new AptkAmaLocalTableService<T>(_localStoreConfiguration, syncTable, this);
                genericLocalTable = localTable;
                _localTables.Add(typeof(T), localTable);

                _localStoreConfiguration.LocalStoreFileService?.InitializeFileSyncContext(_client, _localStore, localTable);
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
            await Task.WhenAny(InitializationTask, Task.Delay(_localStoreConfiguration.InitTimeout, token));

            if (InitializationTask.IsCompleted && _client.SyncContext.IsInitialized)
            {
                await _client.SyncContext.PushAsync(token);
            }
        }


        public long PendingOperations => _client.SyncContext.PendingOperations;
    }
}
