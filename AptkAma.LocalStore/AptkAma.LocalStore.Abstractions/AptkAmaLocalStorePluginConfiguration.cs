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
        /// <param name="databaseFullPath">Database file full device path</param>
        public AptkAmaLocalStorePluginConfiguration(string databaseFullPath)
        {
            DatabaseFullPath = databaseFullPath;
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
        /// <param name="databaseFullPath">Database file full device path</param>
        /// <param name="fileStoreService">An instance of the FileStore extension</param>
        public AptkAmaLocalStorePluginConfiguration(string databaseFullPath, IAptkAmaFileStoreService fileStoreService)
        {
            DatabaseFullPath = databaseFullPath;
            FileStoreService = fileStoreService;
        }

        /// <summary>
        /// Database file full device path
        /// </summary>
        public string DatabaseFullPath { get; set; }

        /// <summary>
        /// Database file name with db extension
        /// </summary>
        /// <value>amslocalstore.db</value>
        public string DatabaseFileName { get; set; } = "amslocalstore.db";

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
