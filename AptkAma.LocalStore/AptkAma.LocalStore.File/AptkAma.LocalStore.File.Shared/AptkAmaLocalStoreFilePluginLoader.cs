using System;
using System.IO;
using AptkAma.LocalStore.File.Shared;

namespace Aptk.Plugins.AptkAma.Data
{
    public static class AptkAmaLocalStoreFilePluginLoader
    {
        private static readonly Lazy<IAptkAmaLocalStoreFileService> LazyInstance = new Lazy<IAptkAmaLocalStoreFileService>(CreateAptkAmaLocalStoreFileService, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static IAptkAmaLocalStoreFilePluginConfiguration _configuration;

        public static void Init(IAptkAmaLocalStoreFilePluginConfiguration configuration = null)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#elif __IOS__ || __ANDROID__
            if (configuration == null)
                configuration = new AptkAmaLocalStoreFilePluginConfiguration(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Files"));
            
            if (string.IsNullOrEmpty(configuration.SyncFilesFullPath))
                configuration.SyncFilesFullPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Files");
#else
            if (configuration == null)
                configuration = new AptkAmaLocalStoreFilePluginConfiguration(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Files"));
            
            if (string.IsNullOrEmpty(configuration.SyncFilesFullPath))
                configuration.SyncFilesFullPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Files");
#endif

            if (configuration.FileManagementService == null)
                configuration.FileManagementService = new AptkAmaFileManagementService(configuration.SyncFilesFullPath);

            _configuration = configuration;
        }

        private static IAptkAmaLocalStoreFileService CreateAptkAmaLocalStoreFileService()
        {
            if (_configuration == null)
                Init();

            return new AptkAmaLocalStoreFileService(_configuration);
        }

        /// <summary>
        /// Current file plugin instance
        /// </summary>
        public static IAptkAmaLocalStoreFileService Instance
        {
            get
            {
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
