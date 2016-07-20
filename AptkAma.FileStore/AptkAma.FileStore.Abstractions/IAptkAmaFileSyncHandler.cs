using Microsoft.WindowsAzure.MobileServices.Files.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaFileSyncHandler<T> : IFileSyncHandler where T : ITableData
    {
    }
}
