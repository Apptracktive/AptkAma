using System;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;
using FileAccess = PCLStorage.FileAccess;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaFileManagementService : IAptkAmaFileManagementService
    {
        private readonly string _rootFullPath;
        private readonly string _baseShortPath;
        private readonly string _baseFullPath;

        public AptkAmaFileManagementService()
        {
            _baseShortPath = string.Empty;

#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#elif __IOS__ || __ANDROID__
            _rootFullPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#else
            _rootFullPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

            _baseFullPath = Path.Combine(_rootFullPath, _baseShortPath);
        }

        public AptkAmaFileManagementService(string baseShortPath)
        {
            _baseShortPath = baseShortPath;

#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#elif __IOS__ || __ANDROID__
            _rootFullPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#else
            _rootFullPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

            _baseFullPath = Path.Combine(_rootFullPath, _baseShortPath);
        }

        public virtual string GetFullPath(string path)
        {
            return path.ToLower().Contains(_rootFullPath.ToLower()) ? path : Path.Combine(_baseFullPath, path);
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
