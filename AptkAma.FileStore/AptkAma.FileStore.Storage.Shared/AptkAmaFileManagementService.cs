using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Aptk.Plugins.AptkAma.Data
{
    // Created from MvvmCross File plugin
    public class AptkAmaFileManagementService : AptkAmaBaseFileManagementService
    {
        public AptkAmaFileManagementService() : base(string.Empty)
        {
        }

        public AptkAmaFileManagementService(string baseShortPath) : base(baseShortPath)
        { 
        }

        public override Task CopyFileToStoreAsync(string itemId, string sourceFileFullPath)
        {
            throw new NotImplementedException();
        }

        public override async Task EnsureFolderExistsAsync(string folderShortPath)
        {
            var folderFullPath = GetFullPath(folderShortPath);
            
            try
            {
                await StorageFolder.GetFolderFromPathAsync(folderFullPath);
                return;
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in EnsureFolderExists - folderPath: {folderFullPath} - message: {ex.Message}");
                throw;
            }

            var rootStorageFolder = await StorageFolder.GetFolderFromPathAsync(RootFullPath);
            await CreateFolderAsync(rootStorageFolder, folderShortPath);
        }

        private async Task<StorageFolder> CreateFolderAsync(StorageFolder rootFolder, string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return rootFolder;
            var currentFolder = await CreateFolderAsync(rootFolder, Path.GetDirectoryName(folderPath)).ConfigureAwait(false);
            return await currentFolder.CreateFolderAsync(Path.GetFileName(folderPath), CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
        }

        public override async Task DeleteFileAsync(string fileShortPath)
        {
            try
            {
                var fileFullPath = GetFullPath(fileShortPath);
                var storageFile = await StorageFile.GetFileFromPathAsync(fileFullPath).AsTask().ConfigureAwait(false);
                await storageFile.DeleteAsync().AsTask().ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception)
            {
            }
        }
    }
}
