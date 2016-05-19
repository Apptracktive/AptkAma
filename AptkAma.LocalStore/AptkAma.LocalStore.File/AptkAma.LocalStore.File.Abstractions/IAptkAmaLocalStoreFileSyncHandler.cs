using Microsoft.WindowsAzure.MobileServices.Files.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaLocalStoreFileSyncHandler<T> : IFileSyncHandler where T : ITableData
    {
    }
}
