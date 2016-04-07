using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaRemoteTableService<T> : IMobileServiceTable<T>, IAptkAmaTableService<T> where T : ITableData
    {
    }
}
