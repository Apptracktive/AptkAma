using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaFileStoreService
    {
        void InitializeFileSyncContext<T>(IMobileServiceClient client, IMobileServiceLocalStore store, IAptkAmaLocalTableService<T> table) where T : ITableData;
    }
}
