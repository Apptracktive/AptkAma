using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;
using FileAccess = PCLStorage.FileAccess;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaFileManagementService : IAptkAmaFileManagementService
    {
        private string _rootFullPath;
        private string _fileFolderFullPath;

        public void Init(string rootFullPath, IAptkAmaFileStorePluginConfiguration configuration)
        {
            _rootFullPath = rootFullPath;
            _fileFolderFullPath = string.IsNullOrEmpty(configuration.FileFolderShortPath) ? _rootFullPath : Path.Combine(_rootFullPath, configuration.FileFolderShortPath);
            Debug.WriteLine($"File folder:{_fileFolderFullPath}");
        }

        public virtual string GetFullPath(string path)
        {
            return path.ToLower().Contains(_rootFullPath.ToLower()) ? path : Path.Combine(_fileFolderFullPath, path);
        }

        public virtual async Task CopyFileToStoreAsync(string itemId, string sourceFileFullPath)
        {
            await EnsureFolderExistsAsync(itemId).ConfigureAwait(false);

            var fileName = Path.GetFileName(sourceFileFullPath);

            if (string.IsNullOrEmpty(fileName))
                throw new Exception($"Unable to get the filename from path: {sourceFileFullPath}");

            var targetFileFullPath = GetFullPath(Path.Combine(itemId, fileName));

            var sourceFile = await FileSystem.Current.LocalStorage.GetFileAsync(sourceFileFullPath).ConfigureAwait(false);
            var sourceStream = await sourceFile.OpenAsync(FileAccess.Read).ConfigureAwait(false);

            var targetFile = await FileSystem.Current.LocalStorage.CreateFileAsync(targetFileFullPath, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);

            using (var targetStream = await targetFile.OpenAsync(FileAccess.ReadAndWrite).ConfigureAwait(false))
            {
                await sourceStream.CopyToAsync(targetStream);
            }
        }

        public virtual async Task EnsureFolderExistsAsync(string folderShortPath)
        {
            var folderFullPath = GetFullPath(folderShortPath);

            var checkExists = await FileSystem.Current.LocalStorage.CheckExistsAsync(folderFullPath).ConfigureAwait(false);
            if (checkExists == ExistenceCheckResult.NotFound)
            {
                await FileSystem.Current.LocalStorage.CreateFolderAsync(folderFullPath, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);
            }
        }

        public virtual async Task DeleteFileAsync(string fileShortPath)
        {
            var fileFullPath = GetFullPath(fileShortPath);

            var checkExists = await FileSystem.Current.LocalStorage.CheckExistsAsync(fileFullPath).ConfigureAwait(false);

            if (checkExists == ExistenceCheckResult.FileExists)
            {
                var file = await FileSystem.Current.LocalStorage.GetFileAsync(fileFullPath).ConfigureAwait(false);
                await file.DeleteAsync().ConfigureAwait(false);
            }
        }
    }
}
