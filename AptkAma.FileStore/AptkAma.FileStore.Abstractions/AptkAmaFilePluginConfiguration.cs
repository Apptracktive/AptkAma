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
        /// <param name="fileFolderShortPath">Local folder relative path to store files</param>
        public AptkAmaFileStorePluginConfiguration(string fileFolderShortPath)
        {
            FileFolderShortPath = fileFolderShortPath;
        }

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
        /// <param name="fileFolderShortPath">Local folder relative path to store files</param>
        /// <param name="fileManagementService">File management service</param>
        /// <param name="globalFileSyncTriggerFactory">Global file sync trigger</param>
        public AptkAmaFileStorePluginConfiguration(string fileFolderShortPath, IAptkAmaFileManagementService fileManagementService, IFileSyncTriggerFactory globalFileSyncTriggerFactory)
        {
            FileFolderShortPath = fileFolderShortPath;
            FileManagementService = fileManagementService;
            GlobalFileSyncTriggerFactory = globalFileSyncTriggerFactory;
        }

        /// <summary>
        /// Local folder relative path to store files
        /// </summary>
        public string FileFolderShortPath { get; set; }

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