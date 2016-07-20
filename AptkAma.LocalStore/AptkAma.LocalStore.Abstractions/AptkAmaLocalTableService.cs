using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IMobileServiceLocalStore _store;
        private readonly IMobileServiceSyncTable<T> _syncTable;

        public AptkAmaLocalTableService(IAptkAmaLocalStorePluginConfiguration localStoreConfiguration,
            IMobileServiceLocalStore store,
            IMobileServiceSyncTable<T> syncTable)
        {
            _localStoreConfiguration = localStoreConfiguration;
            _store = store;
            _syncTable = syncTable;
        }

        private async Task<bool> InitializeAsync()
        {
            if (!MobileServiceClient.SyncContext.IsInitialized)
                await MobileServiceClient.SyncContext.InitializeAsync(_store, _localStoreConfiguration.SyncHandler);

            return MobileServiceClient.SyncContext.IsInitialized;
        }

        public async Task<JToken> ReadAsync(string query)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _syncTable.ReadAsync(query);
        }

        public async Task<JObject> InsertAsync(JObject instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            return await _syncTable.InsertAsync(instance);
        }

        public async Task UpdateAsync(JObject instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            await _syncTable.UpdateAsync(instance);
        }

        public async Task DeleteAsync(JObject instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            await _syncTable.DeleteAsync(instance);
        }

        async Task<T> IMobileServiceSyncTable<T>.LookupAsync(string id)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await _syncTable.LookupAsync(id);
        }

        public IMobileServiceTableQuery<T> CreateQuery()
        {
            return _syncTable.CreateQuery();
        }

        public IMobileServiceTableQuery<T> IncludeTotalCount()
        {
            return _syncTable.IncludeTotalCount();
        }

        public IMobileServiceTableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _syncTable.Where(predicate);
        }

        public IMobileServiceTableQuery<U> Select<U>(Expression<Func<T, U>> selector)
        {
            return _syncTable.Select(selector);
        }

        public IMobileServiceTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _syncTable.OrderBy(keySelector);
        }

        public IMobileServiceTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _syncTable.OrderByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _syncTable.ThenBy(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _syncTable.ThenByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> Skip(int count)
        {
            return _syncTable.Skip(count);
        }

        public IMobileServiceTableQuery<T> Take(int count)
        {
            return _syncTable.Take(count);
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync()
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to retrieve data. Initialization failed.", null, null);

            return await _syncTable.ToEnumerableAsync();
        }

        public async Task<List<T>> ToListAsync()
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to retrieve data. Initialization failed.", null, null);

            return await _syncTable.ToListAsync();
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _syncTable.ReadAsync();
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(IMobileServiceTableQuery<U> query)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _syncTable.ReadAsync(query);
        }

        public async Task RefreshAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to refresh data. Initialization failed.", null, null);

            await _syncTable.RefreshAsync(instance);
        }

        public async Task InsertAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            await _syncTable.InsertAsync(instance);
        }

        public async Task UpdateAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            await _syncTable.UpdateAsync(instance);
        }

        public async Task DeleteAsync(T instance)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            await _syncTable.DeleteAsync(instance);
        }

        public async Task PullAsync(string queryId, string query, IDictionary<string, string> parameters, bool pushOtherTables, CancellationToken cancellationToken, PullOptions pullOptions)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(queryId, query, parameters, pushOtherTables, cancellationToken, pullOptions);
        }

        public async Task PullAsync<U>(string queryId, IMobileServiceTableQuery<U> query, bool pushOtherTables, CancellationToken cancellationToken, PullOptions pullOptions)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(queryId, query, pushOtherTables, cancellationToken, pullOptions);
        }

        public async Task PullAsync(string queryId, bool pushOtherTables, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(queryId, _syncTable.CreateQuery(), pushOtherTables, cancellationToken);
        }

        public async Task PullAsync<U>(string queryId, IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(queryId, query, false, cancellationToken);
        }

        public async Task PullAsync<U>(IMobileServiceTableQuery<U> query, bool pushOtherTables, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(typeof(T).Name, query, pushOtherTables, cancellationToken);
        }

        public async Task PullAsync(string queryId, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(queryId, _syncTable.CreateQuery(), false, cancellationToken);
        }

        public async Task PullAsync<U>(IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(typeof(T).Name, query, false, cancellationToken);
        }

        public async Task PullAsync(bool pushOtherTables, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(typeof(T).Name, _syncTable.CreateQuery(), pushOtherTables, cancellationToken);
        }

        public async Task PullAsync(CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to pull data. Initialization failed.", null, null);

            await _syncTable.PullAsync(typeof(T).Name, _syncTable.CreateQuery(), false, cancellationToken);
        }

        public async Task PurgeAsync<U>(string queryId, IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _syncTable.PurgeAsync(queryId, query, cancellationToken);
        }

        public async Task PurgeAsync(string queryId, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _syncTable.PurgeAsync(queryId, _syncTable.CreateQuery(), cancellationToken);
        }

        public async Task PurgeAsync<U>(IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _syncTable.PurgeAsync(typeof(T).Name, query, cancellationToken);
        }

        public async Task PurgeAsync(CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _syncTable.PurgeAsync(typeof(T).Name, _syncTable.CreateQuery(), cancellationToken);
        }

        async Task<JObject> IMobileServiceSyncTable.LookupAsync(string id)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await ((IMobileServiceSyncTable) _syncTable).LookupAsync(id);
        }

        public async Task PurgeAsync(string queryId, string query, bool force, CancellationToken cancellationToken)
        {
            if (!await InitializeAsync())
                throw new MobileServiceInvalidOperationException("Unable to purge data. Initialization failed.", null, null);

            await _syncTable.PurgeAsync(queryId, query, force, cancellationToken);
        }

        public MobileServiceClient MobileServiceClient => _syncTable.MobileServiceClient;

        public string TableName => _syncTable.TableName;

        public MobileServiceRemoteTableOptions SupportedOptions
        {
            get { return _syncTable.SupportedOptions; }
            set { _syncTable.SupportedOptions = value; }
        }
    }
}
