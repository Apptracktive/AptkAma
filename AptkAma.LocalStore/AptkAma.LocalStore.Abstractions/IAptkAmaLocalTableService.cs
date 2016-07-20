using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// Local specific data request service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAptkAmaLocalTableService<T> : IMobileServiceSyncTable<T>, IAptkAmaTableService<T> where T : ITableData
    {
        Task PurgeAsync<U>(IMobileServiceTableQuery<U> query, CancellationToken cancellationToken);

        Task PurgeAsync(string queryId, CancellationToken cancellationToken);

        Task PurgeAsync(CancellationToken cancellationToken);

        Task PullAsync(string queryId, bool pushOtherTables, CancellationToken cancellationToken);

        Task PullAsync<U>(string queryId, IMobileServiceTableQuery<U> query, CancellationToken cancellationToken);

        Task PullAsync<U>(IMobileServiceTableQuery<U> query, bool pushOtherTables, CancellationToken cancellationToken);

        Task PullAsync(string queryId, CancellationToken cancellationToken);

        Task PullAsync<U>(IMobileServiceTableQuery<U> query, CancellationToken cancellationToken);

        Task PullAsync(bool pushOtherTables, CancellationToken cancellationToken);

        Task PullAsync(CancellationToken cancellationToken);
    }
}
