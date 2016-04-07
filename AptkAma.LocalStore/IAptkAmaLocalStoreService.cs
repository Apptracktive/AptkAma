using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaLocalStoreService
    {
        Task<bool> InitializationTask { get; }

        IAptkAmaLocalTableService<T> GetLocalTable<T>() where T : ITableData;
        
        Task PushAsync();

        long PendingOperations { get; }
    }
}
