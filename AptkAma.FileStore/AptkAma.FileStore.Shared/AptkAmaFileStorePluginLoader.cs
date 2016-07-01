﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public static class AptkAmaFileStorePluginLoader
    {
        private static readonly Lazy<IAptkAmaFileStoreService> LazyInstance = new Lazy<IAptkAmaFileStoreService>(CreateAptkAmaFileService, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static IAptkAmaFileStorePluginConfiguration _configuration;

        public static void Init(IAptkAmaFileStorePluginConfiguration configuration)
        {
            if (configuration == null)
                configuration = new AptkAmaFileStorePluginConfiguration(new AptkAmaFileManagementService());

            else if (configuration.FileManagementService == null)
                configuration.FileManagementService = new AptkAmaFileManagementService();
            
            _configuration = configuration;
        }

        private static IAptkAmaFileStoreService CreateAptkAmaFileService()
        {
#if PORTABLE
            return null;
#else
            if (_configuration == null)
                Init(null);

            return new AptkAmaFileStoreService(_configuration);
#endif
        }

        /// <summary>
        /// Current file plugin instance
        /// </summary>
        public static IAptkAmaFileStoreService Instance
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

        public static async Task CopyFileToStoreAsync(this IAptkAmaDataService dataService, string itemId, string sourceFileFullPath)
        {
            await _configuration.FileManagementService.CopyFileToStoreAsync(itemId, sourceFileFullPath);
        }
    }
}
