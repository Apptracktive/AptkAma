namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// Service to manage data
    /// </summary>
    public interface IAptkAmaDataService
    {
        /// <summary>
        /// Service to manage remote Azure data
        /// </summary>
        /// <typeparam name="T">Data table to manage (model class)</typeparam>
        /// <returns></returns>
        IAptkAmaRemoteTableService<T> RemoteTable<T>() where T : ITableData;
    }
}
