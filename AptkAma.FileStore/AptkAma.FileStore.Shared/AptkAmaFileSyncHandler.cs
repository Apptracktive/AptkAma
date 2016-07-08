using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaFileSyncHandler<T> : IAptkAmaFileSyncHandler<T> where T : ITableData
    {
        private readonly IAptkAmaFileStorePluginConfiguration _configuration;
        private readonly IAptkAmaLocalTableService<T> _table;

        public AptkAmaFileSyncHandler(IAptkAmaFileStorePluginConfiguration configuration, IAptkAmaLocalTableService<T> table)
        {
            _configuration = configuration;
            _table = table;
        }

        public Task<IMobileServiceFileDataSource> GetDataSource(MobileServiceFileMetadata metadata)
        {
#if PORTABLE || WINDOWS_PHONE
            throw new ArgumentException("This functionality is not implemented in this version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            var source = new PathMobileServiceFileDataSource(GetFileFullPath(metadata.ParentDataItemId, metadata.FileName)) as IMobileServiceFileDataSource;

            return Task.FromResult(source);
#endif
        }

        public async Task ProcessFileSynchronizationAction(MobileServiceFile file, FileSynchronizationAction action)
        {
#if PORTABLE || WINDOWS_PHONE
            throw new ArgumentException("This functionality is not implemented in this version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            if (action == FileSynchronizationAction.Delete)
            {
                await _configuration.FileManagementService.DeleteFileAsync(GetFileFullPath(file.ParentId, file.Name));
            }
            else
            {
                try
                {
                    await _configuration.FileManagementService.EnsureFolderExistsAsync(file.ParentId);
                    await _table.DownloadFileAsync(file, GetFileFullPath(file.ParentId, file.Name));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Download error: {ex.Message} | {ex.StackTrace}");
                }
            }
#endif
        }

        private string GetFileFullPath(string itemId, string fileName)
        {
            return _configuration.FileManagementService.GetFullPath(Path.Combine(itemId, fileName));
        }
    }
}