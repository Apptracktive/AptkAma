﻿using System;
using System.Diagnostics;
using System.IO;
using Aptk.Plugins.AptkAma.Data;
using Microsoft.WindowsAzure.MobileServices.Files;

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
            string fileName = Path.GetFileName(filePath);

            string targetPath = GetLocalFilePath(itemId, fileName);

            System.IO.File.Copy(filePath, targetPath);

            return targetPath;
#endif
        }

        public virtual string GetLocalFilePath(string itemId, string fileName)
        {
            Debug.WriteLine($"GetLocalFilePath | ItemId: {itemId}, FileName: {fileName}");
#if PORTABLE
            Debug.WriteLine("GetLocalFilePath | PORTABLE");
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            string recordFilesPath = Path.Combine(_filePath, itemId);
            Debug.WriteLine($"GetLocalFilePath | RecordFilePath: {recordFilesPath}");

            if (!Directory.Exists(recordFilesPath))
            {
                Debug.WriteLine($"GetLocalFilePath | RecordFilePath: {recordFilesPath}");
                Directory.CreateDirectory(recordFilesPath);
            }
            Debug.WriteLine($"GetLocalFilePath | Ok");

            return Path.Combine(recordFilesPath, fileName);
#endif
        }

        public virtual void DeleteLocalFile(MobileServiceFile file)
        {
#if PORTABLE
            throw new ArgumentException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#else
            string localPath = GetLocalFilePath(file.ParentId, file.Name);

            if (System.IO.File.Exists(localPath))
            {
                System.IO.File.Delete(localPath);
            }
#endif
        }
    }
}
