using Microsoft.WindowsAzure.MobileServices.Files.Sync.Triggers;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// The FileStore plugin extension configuration
    /// </summary>
    public class AptkAmaFileStorePluginConfiguration : IAptkAmaFileStorePluginConfiguration
    {
        /// <summary>
        /// FileStore plugin configuration constructor
        /// </summary>
        /// <param name="fileManagementService">File management service</param>
        public AptkAmaFileStorePluginConfiguration(IAptkAmaFileManagementService fileManagementService)
        {
            FileManagementService = fileManagementService;
        }

        /// <summary>
        /// FileStore plugin configuration constructor
        /// </summary>
        /// <param name="globalFileSyncTriggerFactory">Global file sync trigger</param>
        public AptkAmaFileStorePluginConfiguration(IFileSyncTriggerFactory globalFileSyncTriggerFactory)
        {
            GlobalFileSyncTriggerFactory = globalFileSyncTriggerFactory;
        }

        /// <summary>
        /// FileStore plugin configuration constructor
        /// </summary>
        /// <param name="fileManagementService">File management service</param>
        /// <param name="globalFileSyncTriggerFactory">Global file sync trigger</param>
        public AptkAmaFileStorePluginConfiguration(IAptkAmaFileManagementService fileManagementService, IFileSyncTriggerFactory globalFileSyncTriggerFactory)
        {
            GlobalFileSyncTriggerFactory = globalFileSyncTriggerFactory;
        }

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