using Microsoft.WindowsAzure.MobileServices.Files.Sync.Triggers;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// The file plugin extension configuration
    /// </summary>
    public class AptkAmaLocalStoreFilePluginConfiguration : IAptkAmaLocalStoreFilePluginConfiguration
    {
        /// <summary>
        /// File plugin configuration constructor
        /// </summary>
        /// <param name="syncFilesFullPath">Sync files full device path</param>
        public AptkAmaLocalStoreFilePluginConfiguration(string syncFilesFullPath)
        {
            SyncFilesFullPath = syncFilesFullPath;
        }

        /// <summary>
        /// Sync files full device path
        /// </summary>
        public string SyncFilesFullPath { get; set; }

        /// <summary>
        /// Local file management service
        /// </summary>
        public IAptkAmaFileManagementService FileManagementService { get; set; }

        /// <summary>
        /// A trigger to manage the file sync
        /// </summary>
        public IFileSyncTriggerFactory GlobalFileSyncTriggerFactory { get; set; }
    }
}