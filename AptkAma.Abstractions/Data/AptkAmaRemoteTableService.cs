using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaRemoteTableService<T> : IAptkAmaRemoteTableService<T> where T : ITableData
    {
        private readonly IMobileServiceClient _client;
        private IMobileServiceTable<T> _remoteTable;
        private bool _isInitialized;

        public AptkAmaRemoteTableService(IMobileServiceClient client)
        {
            _client = client;
            Initialize();
        }

        private bool Initialize()
        {
            if (!_isInitialized)
            {
                try
                {
                    _remoteTable = _client.GetTable<T>();
                    _isInitialized = true;
                }
                catch (Exception)
                {
                    Debug.WriteLine("Impossible d'initialiser {0}", typeof(T).Name);
                }
            }

            return _isInitialized;
        }

        public async Task<JToken> ReadAsync(string query)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _remoteTable.ReadAsync(query);
        }

        public async Task<JToken> ReadAsync(string query, IDictionary<string, string> parameters, bool wrapResult)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _remoteTable.ReadAsync(query, parameters, wrapResult);
        }

        public async Task<JToken> InsertAsync(JObject instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            return await _remoteTable.InsertAsync(instance);
        }

        public async Task<JToken> InsertAsync(JObject instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            return await _remoteTable.InsertAsync(instance, parameters);
        }

        public async Task<JToken> UpdateAsync(JObject instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            return await _remoteTable.UpdateAsync(instance);
        }

        public async Task<JToken> UpdateAsync(JObject instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            return await _remoteTable.UpdateAsync(instance, parameters);
        }

        public async Task<JToken> DeleteAsync(JObject instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            return await _remoteTable.DeleteAsync(instance);
        }

        public async Task<JToken> DeleteAsync(JObject instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            return await _remoteTable.DeleteAsync(instance, parameters);
        }

        public async Task<JToken> UndeleteAsync(JObject instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to undelete data. Initialization failed.", null, null);

            return await _remoteTable.UndeleteAsync(instance);
        }

        public async Task<JToken> UndeleteAsync(JObject instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to undelete data. Initialization failed.", null, null);

            return await _remoteTable.UndeleteAsync(instance, parameters);
        }

        async Task<T> IMobileServiceTable<T>.LookupAsync(object id)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await _remoteTable.LookupAsync(id);
        }

        async Task<T> IMobileServiceTable<T>.LookupAsync(object id, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await _remoteTable.LookupAsync(id, parameters);
        }

        public async Task RefreshAsync(T instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to refresh data. Initialization failed.", null, null);

            await _remoteTable.RefreshAsync(instance);
        }

        public async Task RefreshAsync(T instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to refresh data. Initialization failed.", null, null);

            await _remoteTable.RefreshAsync(instance, parameters);
        }

        public async Task InsertAsync(T instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            await _remoteTable.InsertAsync(instance);
        }

        public async Task InsertAsync(T instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to insert data. Initialization failed.", null, null);

            await _remoteTable.InsertAsync(instance, parameters);
        }

        public async Task UpdateAsync(T instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            await _remoteTable.UpdateAsync(instance);
        }

        public async Task UpdateAsync(T instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to update data. Initialization failed.", null, null);

            await _remoteTable.UpdateAsync(instance, parameters);
        }

        public async Task UndeleteAsync(T instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to undelete data. Initialization failed.", null, null);

            await _remoteTable.UndeleteAsync(instance);
        }

        public async Task UndeleteAsync(T instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to undelete data. Initialization failed.", null, null);

            await _remoteTable.UndeleteAsync(instance, parameters);
        }

        public async Task DeleteAsync(T instance)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            await _remoteTable.DeleteAsync(instance);
        }

        public async Task DeleteAsync(T instance, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to delete data. Initialization failed.", null, null);

            await _remoteTable.DeleteAsync(instance, parameters);
        }

        public IMobileServiceTableQuery<T> CreateQuery()
        {
            return _remoteTable.CreateQuery();
        }

        public IMobileServiceTableQuery<T> IncludeTotalCount()
        {
            return _remoteTable.IncludeTotalCount();
        }

        public IMobileServiceTableQuery<T> IncludeDeleted()
        {
            return _remoteTable.IncludeDeleted();
        }

        public IMobileServiceTableQuery<T> WithParameters(IDictionary<string, string> parameters)
        {
            return _remoteTable.WithParameters(parameters);
        }

        public IMobileServiceTableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _remoteTable.Where(predicate);
        }

        public IMobileServiceTableQuery<U> Select<U>(Expression<Func<T, U>> selector)
        {
            return _remoteTable.Select(selector);
        }

        public IMobileServiceTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _remoteTable.OrderBy(keySelector);
        }

        public IMobileServiceTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _remoteTable.OrderByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _remoteTable.ThenBy(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _remoteTable.ThenByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> Skip(int count)
        {
            return _remoteTable.Skip(count);
        }

        public IMobileServiceTableQuery<T> Take(int count)
        {
            return _remoteTable.Take(count);
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync()
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to retrieve data. Initialization failed.", null, null);

            return await _remoteTable.ToEnumerableAsync();
        }

        public async Task<List<T>> ToListAsync()
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to retrieve data. Initialization failed.", null, null);

            return await _remoteTable.ToListAsync();
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(string query)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _remoteTable.ReadAsync<U>(query);
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(IMobileServiceTableQuery<U> query)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _remoteTable.ReadAsync(query);
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to read data. Initialization failed.", null, null);

            return await _remoteTable.ReadAsync();
        }

        async Task<JToken> IMobileServiceTable.LookupAsync(object id)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await ((IMobileServiceTable)_remoteTable).LookupAsync(id);
        }

        async Task<JToken> IMobileServiceTable.LookupAsync(object id, IDictionary<string, string> parameters)
        {
            if (!Initialize())
                throw new MobileServiceInvalidOperationException("Unable to lookup data. Initialization failed.", null, null);

            return await ((IMobileServiceTable)_remoteTable).LookupAsync(id, parameters);
        }

        public MobileServiceClient MobileServiceClient => _remoteTable.MobileServiceClient;

        public string TableName => _remoteTable.TableName;

        public MobileServiceSystemProperties SystemProperties
        {
            get { return _remoteTable.SystemProperties; }
            set { _remoteTable.SystemProperties = value; }
        }
    }
}
