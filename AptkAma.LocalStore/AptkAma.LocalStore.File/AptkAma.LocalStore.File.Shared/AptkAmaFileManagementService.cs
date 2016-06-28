using System;
using System.Diagnostics;
using System.IO;
using Aptk.Plugins.AptkAma.Data;

namespace AptkAma.LocalStore.File.Shared
{
    public class AptkAmaFileManagementService : IAptkAmaFileManagementService
    {
        private readonly string _filePath;

        public AptkAmaFileManagementService(string filePath)
        {
            _filePath = filePath;
        }

        public virtual string FilePath => _filePath;

        public virtual string CopyFileToAppDirectory(string itemId, string filePath)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            var fileName = Path.GetFileName(filePath);

            var targetPath = GetLocalFilePath(itemId, fileName);

            System.IO.File.Copy(filePath, targetPath);

            return targetPath;
#endif
        }

        public virtual string GetLocalFilePath(string itemId, string fileName)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            var recordFilesPath = Path.Combine(_filePath, itemId);

            if (!Directory.Exists(recordFilesPath))
                Directory.CreateDirectory(recordFilesPath);
            
            return Path.Combine(recordFilesPath, fileName);
#endif
        }

        public virtual void DeleteLocalFile(string itemId, string fileName)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            var localPath = GetLocalFilePath(itemId, fileName);

            if (System.IO.File.Exists(localPath))
            {
                System.IO.File.Delete(localPath);
            }
#endif
        }
    }
}
