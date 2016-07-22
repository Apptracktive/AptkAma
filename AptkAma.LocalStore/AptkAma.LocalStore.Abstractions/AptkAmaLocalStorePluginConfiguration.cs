using System;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// AptkAma Plugin's Local Store Extension configuration
    /// </summary>
    public class AptkAmaLocalStorePluginConfiguration : IAptkAmaLocalStorePluginConfiguration
    {
        /// <summary>
        /// AptkAma Plugin's Local Store Extension constructor
        /// </summary>
        /// <param name="databaseShortPath">Database file short device path</param>
        public AptkAmaLocalStorePluginConfiguration(string databaseShortPath)
        {
            DatabaseShortPath = databaseShortPath;
        }

        /// <summary>
        /// AptkAma Plugin's Local Store Extension constructor
        /// </summary>
        /// <param name="fileStoreService">An instance of the FileStore extension</param>
        public AptkAmaLocalStorePluginConfiguration(IAptkAmaFileStoreService fileStoreService)
        {
            FileStoreService = fileStoreService;
        }

        /// <summary>
        /// AptkAma Plugin's Local Store Extension constructor
        /// </summary>
        /// <param name="databaseShortPath">Database file short device path</param>
        /// <param name="fileStoreService">An instance of the FileStore extension</param>
        public AptkAmaLocalStorePluginConfiguration(string databaseShortPath, IAptkAmaFileStoreService fileStoreService)
        {
            DatabaseShortPath = databaseShortPath;
            FileStoreService = fileStoreService;
        }

        /// <summary>
        /// Database file short device path
        /// </summary>
        public string DatabaseShortPath { get; set; }

        /// <summary>
        /// Database file name with db extension
        /// </summary>
        /// <value>amalocalstore.db</value>
        public string DatabaseFileName { get; set; } = "amalocalstore.db";

        /// <summary>
        /// A mobile service sync handler instance
        /// </summary>
        /// <value>MobileServiceSyncHandler</value>
        public IMobileServiceSyncHandler SyncHandler { get; set; } = new MobileServiceSyncHandler();

        /// <summary>
        /// An instance of the LocalStore File extension
        /// </summary>
        public IAptkAmaFileStoreService FileStoreService { get; set; }
    }
}
