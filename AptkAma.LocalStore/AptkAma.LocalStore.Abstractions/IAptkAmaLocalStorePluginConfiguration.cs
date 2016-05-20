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
        /// Database file full device path
        /// </summary>
        string DatabaseFullPath { get; set; }

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
        /// An instance of the LocalStore File extension
        /// </summary>
        IAptkAmaLocalStoreFileService LocalStoreFileService { get; set; }
    }
}
