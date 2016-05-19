using Microsoft.WindowsAzure.MobileServices.Files.Sync.Triggers;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// The file plugin extension configuration
    /// </summary>
    public interface IAptkAmaLocalStoreFilePluginConfiguration
    {
        /// <summary>
        /// Sync files full device path
        /// </summary>
        string SyncFilesFullPath { get; set; }

        /// <summary>
        /// Local file management service
        /// </summary>
        IAptkAmaFileManagementService FileManagementService { get; set; }

        /// <summary>
        /// A trigger to manage global file sync
        /// </summary>
        IFileSyncTriggerFactory GlobalFileSyncTriggerFactory { get; set; }
    }
}
