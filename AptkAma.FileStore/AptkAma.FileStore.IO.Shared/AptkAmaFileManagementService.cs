using System;
using System.IO;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public class AptkAmaFileManagementService : AptkAmaBaseFileManagementService
    {
        public AptkAmaFileManagementService() : base(string.Empty)
        {
        }

        public AptkAmaFileManagementService(string baseShortPath) : base(baseShortPath)
        {
        }

        public override async Task CopyFileToStoreAsync(string itemId, string sourceFileFullPath)
        {
            await EnsureFolderExistsAsync(itemId).ConfigureAwait(false);

            var fileName = Path.GetFileName(sourceFileFullPath);

            if(string.IsNullOrEmpty(fileName))
                throw new Exception($"Unable to get the filename from path: {sourceFileFullPath}");

            var targetFileFullPath = GetFullPath(Path.Combine(itemId, fileName));

            File.Copy(sourceFileFullPath, targetFileFullPath);
        }

        public override Task EnsureFolderExistsAsync(string folderShortPath)
        {
            var folderFullPath = GetFullPath(folderShortPath);

            if (!Directory.Exists(folderFullPath))
                Directory.CreateDirectory(folderFullPath);

            return Task.FromResult(true);
        }

        public override Task DeleteFileAsync(string fileShortPath)
        {
            var fileFullPath = GetFullPath(fileShortPath);

            if (File.Exists(fileFullPath))
                File.Delete(fileFullPath);

            return Task.FromResult(true);
        }
    }
}
