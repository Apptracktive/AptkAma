using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaLocalTableService<T> : IAptkAmaLocalTableService<T> where T : ITableData
    {
        private readonly IAptkAmaLocalStorePluginConfiguration _localStoreConfiguration;
        private readonly IMobileServiceClient _client;
        private readonly IAptkAmaLocalStoreService _localStoreService;
        private IMobileServiceSyncTable<T> _localTable;

        public AptkAmaLocalTableService(IAptkAmaLocalStorePluginConfiguration localStoreConfiguration, 
            IMobileServiceClient client, 
            IAptkAmaLocalStoreService localStoreService)
        {
            _localStoreConfiguration = localStoreConfiguration;
            _client = client;
            _localStoreService = localStoreService;
            Task.Run(async () => await InitializeAsync());
        }

        private async Task<bool> InitializeAsync()
        {
            var cts = new CancellationTokenSource();
            try
            {
                await Task.WhenAny(_localStoreService.InitializationTask, Task.Delay(_localStoreConfiguration.InitTimeout, cts.Token));
            }
            catch (TaskCanceledException)
            {
                throw new MobileServiceInvalidOperationException($"Initialization timed out after {_localStoreConfiguration.InitTimeout.TotalSeconds} seconds.", null, null);
            }

            if (_localStoreService.InitializationTask.IsCompleted && _client.SyncContext.IsInitialized)
            {
                _localTable = _client.GetSyncTable<T>();
            }

            return _localStoreService.InitializationTask.IsCompleted;
        }

        public async Task<JToken> ReadAsync(string query)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _localTable.ReadAsync(query);
        }

        public async Task<JObject> InsertAsync(JObject instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            return await _localTable.InsertAsync(instance);
        }

        public async Task UpdateAsync(JObject instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            await _localTable.UpdateAsync(instance);
        }

        public async Task DeleteAsync(JObject instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            await _localTable.DeleteAsync(instance);
        }

        async Task<T> IMobileServiceSyncTable<T>.LookupAsync(string id)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await _localTable.LookupAsync(id);
        }

        public IMobileServiceTableQuery<T> CreateQuery()
        {
            return _localTable.CreateQuery();
        }

        public IMobileServiceTableQuery<T> IncludeTotalCount()
        {
            return _localTable.IncludeTotalCount();
        }

        public IMobileServiceTableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _localTable.Where(predicate);
        }

        public IMobileServiceTableQuery<U> Select<U>(Expression<Func<T, U>> selector)
        {
            return _localTable.Select(selector);
        }

        public IMobileServiceTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _localTable.OrderBy(keySelector);
        }

        public IMobileServiceTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _localTable.OrderByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _localTable.ThenBy(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _localTable.ThenByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> Skip(int count)
        {
            return _localTable.Skip(count);
        }

        public IMobileServiceTableQuery<T> Take(int count)
        {
            return _localTable.Take(count);
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync()
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to retrieve data. Initialization failed.", null, null);

            return await _localTable.ToEnumerableAsync();
        }

        public async Task<List<T>> ToListAsync()
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to retrieve data. Initialization failed.", null, null);

            return await _localTable.ToListAsync();
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _localTable.ReadAsync();
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(IMobileServiceTableQuery<U> query)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _localTable.ReadAsync(query);
        }

        public async Task RefreshAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to refresh data. Initialization failed.", null, null);

            await _localTable.RefreshAsync(instance);
        }

        public async Task InsertAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            await _localTable.InsertAsync(instance);
        }

        public async Task UpdateAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            await _localTable.UpdateAsync(instance);
        }

        public async Task DeleteAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            await _localTable.DeleteAsync(instance);
        }

        public async Task PullAsync(string queryId, string query, IDictionary<string, string> parameters, bool pushOtherTables, CancellationToken cancellationToken, PullOptions pullOptions)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(queryId, query, parameters, pushOtherTables, cancellationToken, pullOptions);
        }

        public async Task PullAsync<U>(string queryId, IMobileServiceTableQuery<U> query, bool pushOtherTables, CancellationToken cancellationToken, PullOptions pullOptions)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(queryId, query, pushOtherTables, cancellationToken, pullOptions);
        }

        public async Task PullAsync(string queryId, bool pushOtherTables, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(queryId, _localTable.CreateQuery(), pushOtherTables, cancellationToken);
        }

        public async Task PullAsync<U>(string queryId, IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(queryId, query, false, cancellationToken);
        }

        public async Task PullAsync<U>(IMobileServiceTableQuery<U> query, bool pushOtherTables, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(typeof(T).Name, query, pushOtherTables, cancellationToken);
        }

        public async Task PullAsync(string queryId, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(queryId, _localTable.CreateQuery(), false, cancellationToken);
        }

        public async Task PullAsync<U>(IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(typeof(T).Name, query, false, cancellationToken);
        }

        public async Task PullAsync(bool pushOtherTables, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(typeof(T).Name, _localTable.CreateQuery(), pushOtherTables, cancellationToken);
        }

        public async Task PullAsync(CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _localTable.PullAsync(typeof(T).Name, _localTable.CreateQuery(), false, cancellationToken);
        }

        public async Task PurgeAsync<U>(string queryId, IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _localTable.PurgeAsync(queryId, query, cancellationToken);
        }

        public async Task PurgeAsync(string queryId, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _localTable.PurgeAsync(queryId, _localTable.CreateQuery(), cancellationToken);
        }

        public async Task PurgeAsync<U>(IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _localTable.PurgeAsync(typeof(T).Name, query, cancellationToken);
        }

        public async Task PurgeAsync(CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _localTable.PurgeAsync(typeof(T).Name, _localTable.CreateQuery(), cancellationToken);
        }

        async Task<JObject> IMobileServiceSyncTable.LookupAsync(string id)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await ((IMobileServiceSyncTable) _localTable).LookupAsync(id);
        }

        public async Task PurgeAsync(string queryId, string query, bool force, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _localTable.PurgeAsync(queryId, query, force, cancellationToken);
        }

        public MobileServiceClient MobileServiceClient => _localTable.MobileServiceClient;

        public string TableName => _localTable.TableName;

        public MobileServiceRemoteTableOptions SupportedOptions
        {
            get { return _localTable.SupportedOptions; }
            set { _localTable.SupportedOptions = value; }
        }
    }
}
