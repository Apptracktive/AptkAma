using Microsoft.WindowsAzure.MobileServices.Files.Sync.Triggers;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IFileSyncTableData : ITableData
    {
        /// <summary>
        /// A trigger to manage this specific table file sync
        /// </summary>
        IFileSyncTriggerFactory SpecificFileSyncTriggerFactory { get; }
    }
}
