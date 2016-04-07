using System;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public static class AptkAmaLocalStorePluginLoader
    {
        private static readonly Lazy<IAptkAmaLocalStoreService> LazyInstance = new Lazy<IAptkAmaLocalStoreService>(CreateAptkAmaLocalStoreService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        
        private static IAptkAmaLocalStorePluginConfiguration _localStoreConfiguration;
        private static IAptkAmaDataService _dataService;

        public static void Init(IAptkAmaLocalStorePluginConfiguration localStoreConfiguration)
        {
            _localStoreConfiguration = localStoreConfiguration;
        }

        private static IAptkAmaLocalStoreService CreateAptkAmaLocalStoreService()
        {
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
        /// Local pending changes waiting for push to remote Azure tables
        /// </summary>
        public static long PendingChanges(this IAptkAmaDataService dataService)
        {
            _dataService = dataService;
            return Instance.PendingOperations;
        }

    }
}
