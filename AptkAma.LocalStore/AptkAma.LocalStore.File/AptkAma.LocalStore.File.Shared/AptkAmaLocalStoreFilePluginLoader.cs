using System;
using AptkAma.LocalStore.File.Shared;

namespace Aptk.Plugins.AptkAma.Data
{
    public static class AptkAmaLocalStoreFilePluginLoader
    {
        private static readonly Lazy<IAptkAmaLocalStoreFileService> LazyInstance = new Lazy<IAptkAmaLocalStoreFileService>(CreateAptkAmaLocalStoreFileService, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static IAptkAmaLocalStoreFilePluginConfiguration _configuration;

        public static void Init(IAptkAmaLocalStoreFilePluginConfiguration configuration)
        {
            _configuration = configuration;
            if(_configuration.FileManagementService == null)
                _configuration.FileManagementService = new AptkAmaFileManagementService(_configuration.SyncFilesFullPath);
        }

        private static IAptkAmaLocalStoreFileService CreateAptkAmaLocalStoreFileService()
        {
            return new AptkAmaLocalStoreFileService(_configuration);
        }

        /// <summary>
        /// Current file plugin instance
        /// </summary>
        public static IAptkAmaLocalStoreFileService Instance
        {
            get
            {
                if (_configuration == null)
                {
                    throw new Exception($"You must call {nameof(AptkAmaLocalStoreFilePluginLoader)}'s Init method before working with files.");
                }
                var instance = LazyInstance.Value;
                if (instance == null)
                {
                    throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return instance;
            }
        }

        /// <summary>
        /// Current file management service
        /// </summary>
        /// <param name="dataService">The data service</param>
        /// <returns>A file management service implementation instance</returns>
        public static IAptkAmaFileManagementService FileManagementService(this IAptkAmaDataService dataService)
        {
            return _configuration.FileManagementService;
        }
    }
}
