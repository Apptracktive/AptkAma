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
        private readonly IMobileServiceTable<T> _table;

        public AptkAmaRemoteTableService(IMobileServiceTable<T> table)
        {
            _table = table;
        }

        public async Task<JToken> ReadAsync(string query)
        {
            return await _table.ReadAsync(query);
        }

        public async Task<JToken> ReadAsync(string query, IDictionary<string, string> parameters, bool wrapResult)
        {
            return await _table.ReadAsync(query, parameters, wrapResult);
        }

        public async Task<JToken> InsertAsync(JObject instance)
        {
            return await _table.InsertAsync(instance);
        }

        public async Task<JToken> InsertAsync(JObject instance, IDictionary<string, string> parameters)
        {
            return await _table.InsertAsync(instance, parameters);
        }

        public async Task<JToken> UpdateAsync(JObject instance)
        {
            return await _table.UpdateAsync(instance);
        }

        public async Task<JToken> UpdateAsync(JObject instance, IDictionary<string, string> parameters)
        {
            return await _table.UpdateAsync(instance, parameters);
        }

        public async Task<JToken> DeleteAsync(JObject instance)
        {
            return await _table.DeleteAsync(instance);
        }

        public async Task<JToken> DeleteAsync(JObject instance, IDictionary<string, string> parameters)
        {
            return await _table.DeleteAsync(instance, parameters);
        }

        public async Task<JToken> UndeleteAsync(JObject instance)
        {
            return await _table.UndeleteAsync(instance);
        }

        public async Task<JToken> UndeleteAsync(JObject instance, IDictionary<string, string> parameters)
        {
            return await _table.UndeleteAsync(instance, parameters);
        }

        async Task<T> IMobileServiceTable<T>.LookupAsync(object id)
        {
            return await _table.LookupAsync(id);
        }

        async Task<T> IMobileServiceTable<T>.LookupAsync(object id, IDictionary<string, string> parameters)
        {
            return await _table.LookupAsync(id, parameters);
        }

        public async Task RefreshAsync(T instance)
        {
            await _table.RefreshAsync(instance);
        }

        public async Task RefreshAsync(T instance, IDictionary<string, string> parameters)
        {
            await _table.RefreshAsync(instance, parameters);
        }

        public async Task InsertAsync(T instance)
        {
            await _table.InsertAsync(instance);
        }

        public async Task InsertAsync(T instance, IDictionary<string, string> parameters)
        {
            await _table.InsertAsync(instance, parameters);
        }

        public async Task UpdateAsync(T instance)
        {
            await _table.UpdateAsync(instance);
        }

        public async Task UpdateAsync(T instance, IDictionary<string, string> parameters)
        {
            await _table.UpdateAsync(instance, parameters);
        }

        public async Task UndeleteAsync(T instance)
        {
            await _table.UndeleteAsync(instance);
        }

        public async Task UndeleteAsync(T instance, IDictionary<string, string> parameters)
        {
            await _table.UndeleteAsync(instance, parameters);
        }

        public async Task DeleteAsync(T instance)
        {
            await _table.DeleteAsync(instance);
        }

        public async Task DeleteAsync(T instance, IDictionary<string, string> parameters)
        {
            await _table.DeleteAsync(instance, parameters);
        }

        public IMobileServiceTableQuery<T> CreateQuery()
        {
            return _table.CreateQuery();
        }

        public IMobileServiceTableQuery<T> IncludeTotalCount()
        {
            return _table.IncludeTotalCount();
        }

        public IMobileServiceTableQuery<T> IncludeDeleted()
        {
            return _table.IncludeDeleted();
        }

        public IMobileServiceTableQuery<T> WithParameters(IDictionary<string, string> parameters)
        {
            return _table.WithParameters(parameters);
        }

        public IMobileServiceTableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _table.Where(predicate);
        }

        public IMobileServiceTableQuery<U> Select<U>(Expression<Func<T, U>> selector)
        {
            return _table.Select(selector);
        }

        public IMobileServiceTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _table.OrderBy(keySelector);
        }

        public IMobileServiceTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _table.OrderByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _table.ThenBy(keySelector);
        }

        public IMobileServiceTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _table.ThenByDescending(keySelector);
        }

        public IMobileServiceTableQuery<T> Skip(int count)
        {
            return _table.Skip(count);
        }

        public IMobileServiceTableQuery<T> Take(int count)
        {
            return _table.Take(count);
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync()
        {
            return await _table.ToEnumerableAsync();
        }

        public async Task<List<T>> ToListAsync()
        {
            return await _table.ToListAsync();
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(string query)
        {
            return await _table.ReadAsync<U>(query);
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(IMobileServiceTableQuery<U> query)
        {
            return await _table.ReadAsync(query);
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            return await _table.ReadAsync();
        }

        async Task<JToken> IMobileServiceTable.LookupAsync(object id)
        {
            return await ((IMobileServiceTable)_table).LookupAsync(id);
        }

        async Task<JToken> IMobileServiceTable.LookupAsync(object id, IDictionary<string, string> parameters)
        {
            return await ((IMobileServiceTable)_table).LookupAsync(id, parameters);
        }

        public MobileServiceClient MobileServiceClient => _table.MobileServiceClient;

        public string TableName => _table.TableName;
    }
}
