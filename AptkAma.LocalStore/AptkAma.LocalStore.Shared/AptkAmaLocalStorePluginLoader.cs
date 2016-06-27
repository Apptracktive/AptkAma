using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public static class AptkAmaLocalStorePluginLoader
    {
        private static readonly Lazy<IAptkAmaLocalStoreService> LazyInstance = new Lazy<IAptkAmaLocalStoreService>(CreateAptkAmaLocalStoreService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        
        private static IAptkAmaLocalStorePluginConfiguration _localStoreConfiguration;
        private static IAptkAmaDataService _dataService;

        public static void Init(IAptkAmaLocalStorePluginConfiguration localStoreConfiguration = null)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#elif __IOS__
            SQLitePCL.CurrentPlatform.Init();
            if (localStoreConfiguration == null)
                localStoreConfiguration = new AptkAmaLocalStorePluginConfiguration(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
            
            if (string.IsNullOrEmpty(localStoreConfiguration.DatabaseFullPath))
                localStoreConfiguration.DatabaseFullPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#elif __ANDROID__
            if (localStoreConfiguration == null)
                localStoreConfiguration = new AptkAmaLocalStorePluginConfiguration(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
            
            if (string.IsNullOrEmpty(localStoreConfiguration.DatabaseFullPath))
                localStoreConfiguration.DatabaseFullPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#else
            if (localStoreConfiguration == null)
                localStoreConfiguration = new AptkAmaLocalStorePluginConfiguration(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
            
            if (string.IsNullOrEmpty(localStoreConfiguration.DatabaseFullPath))
                localStoreConfiguration.DatabaseFullPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

            _localStoreConfiguration = localStoreConfiguration;
        }

        private static IAptkAmaLocalStoreService CreateAptkAmaLocalStoreService()
        {
            if (_localStoreConfiguration == null)
                Init();

            return new AptkAmaLocalStoreService(_localStoreConfiguration, _dataService);
        }

        /// <summary>
        /// Current plugin instance
        /// </summary>
        private static IAptkAmaLocalStoreService Instance
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
        /// Service to manage local SQLite data
        /// </summary>
        /// <typeparam name="T">Data table to manage (model class)</typeparam>
        /// <returns></returns>
        public static IAptkAmaLocalTableService<T> LocalTable<T>(this IAptkAmaDataService dataService) where T : ITableData
        {
            _dataService = dataService;
            return Instance.GetLocalTable<T>();
        }

        /// <summary>
        /// Push local pending changes to remote Azure tables
        /// </summary>
        /// <returns></returns>
        public static async Task PushAsync(this IAptkAmaDataService dataService)
        {
            _dataService = dataService;
            await Instance.PushAsync();
        }

        /// <summary>
        /// Push local pending changes to remote Azure tables
        /// </summary>
        /// <returns></returns>
        public static async Task PushAsync(this IAptkAmaDataService dataService, CancellationToken token)
        {
            _dataService = dataService;
            await Instance.PushAsync(token);
        }

        /// <summary>
        /// Local pending changes waiting for push to remote Azure tables
        /// </summary>
        public static long PendingChanges(this IAptkAmaDataService dataService)
        {
            _dataService = dataService;
            return Instance.PendingOperations;
        }
    }
}
