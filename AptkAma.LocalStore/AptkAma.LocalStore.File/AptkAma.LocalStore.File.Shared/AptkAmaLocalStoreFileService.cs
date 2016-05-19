using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaLocalStoreFileService : IAptkAmaLocalStoreFileService
    {
        private readonly IAptkAmaLocalStoreFilePluginConfiguration _configuration;

        public AptkAmaLocalStoreFileService(IAptkAmaLocalStoreFilePluginConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void InitializeFileSyncContext<T>(IMobileServiceClient client, IMobileServiceLocalStore store, IAptkAmaLocalTableService<T> table) where T : ITableData
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            var tInstance = Activator.CreateInstance<T>() as IFileSyncTableData;
            if (tInstance == null)
                return;
            
            var triggerFactory = tInstance.SpecificFileSyncTriggerFactory ?? _configuration.GlobalFileSyncTriggerFactory;
            if(triggerFactory != null)
                client.InitializeFileSyncContext(new AptkAmaLocalStoreFileSyncHandler<T>(_configuration, table), store, triggerFactory);
            else
                client.InitializeFileSyncContext(new AptkAmaLocalStoreFileSyncHandler<T>(_configuration, table), store);
#endif
        }
    }
}