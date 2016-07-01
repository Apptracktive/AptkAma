using Microsoft.WindowsAzure.MobileServices.Files.Sync.Triggers;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// The file plugin extension configuration
    /// </summary>
    public interface IAptkAmaFileStorePluginConfiguration
    {
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
