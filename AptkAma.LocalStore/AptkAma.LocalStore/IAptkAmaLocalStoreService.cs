using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaLocalStoreService
    {
        Task<bool> InitializationTask { get; }

        IAptkAmaLocalTableService<T> GetLocalTable<T>() where T : ITableData;
        
        Task PushAsync();

        Task PushAsync(CancellationToken token);

        long PendingOperations { get; }
    }
}
