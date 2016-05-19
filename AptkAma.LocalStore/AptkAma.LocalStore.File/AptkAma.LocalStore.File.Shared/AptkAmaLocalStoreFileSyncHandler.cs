using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaLocalStoreFileSyncHandler<T> : IAptkAmaLocalStoreFileSyncHandler<T> where T : ITableData
    {
        private readonly IAptkAmaLocalStoreFilePluginConfiguration _configuration;
        private readonly IAptkAmaLocalTableService<T> _table;

        public AptkAmaLocalStoreFileSyncHandler(IAptkAmaLocalStoreFilePluginConfiguration configuration, IAptkAmaLocalTableService<T> table)
        {
            _configuration = configuration;
            _table = table;
        }

        public Task<IMobileServiceFileDataSource> GetDataSource(MobileServiceFileMetadata metadata)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            var filePath = _configuration.FileManagementService.GetLocalFilePath(metadata.ParentDataItemId, metadata.FileName);
            var fileSource = new PathMobileServiceFileDataSource(filePath) as IMobileServiceFileDataSource;

            return Task.FromResult(fileSource);
#endif
        }

        public async Task ProcessFileSynchronizationAction(MobileServiceFile file, FileSynchronizationAction action)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            if (action == FileSynchronizationAction.Delete)
            {
                _configuration.FileManagementService.DeleteLocalFile(file);
            }
            else
            {
                var filePath = _configuration.FileManagementService.GetLocalFilePath(file.ParentId, file.Name);
                await _table.DownloadFileAsync(file, filePath);
            }
#endif
        }
    }
}