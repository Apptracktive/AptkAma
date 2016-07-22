using System;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    /// <summary>
    /// AptkAma Plugin's Local Store Extension configuration
    /// </summary>
    public interface IAptkAmaLocalStorePluginConfiguration
    {
        /// <summary>
        /// Database file short device path
        /// </summary>
        string DatabaseShortPath { get; set; }

        /// <summary>
        /// Database file name with db extension
        /// </summary>
        /// <value>amslocalstore.db</value>
        string DatabaseFileName { get; set; }

        /// <summary>
        /// A mobile service sync handler instance
        /// </summary>
        /// <value>MobileServiceSyncHandler</value>
        IMobileServiceSyncHandler SyncHandler { get; set; }

        /// <summary>
        /// An instance of the FileStore extension
        /// </summary>
        IAptkAmaFileStoreService FileStoreService { get; set; }
    }
}
