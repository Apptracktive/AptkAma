using Microsoft.WindowsAzure.MobileServices.Files;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaFileManagementService
    {
        string FilePath { get; }
        string CopyFileToAppDirectory(string itemId, string filePath);
        string GetLocalFilePath(string itemId, string fileName);
        void DeleteLocalFile(string itemId, string fileName);
    }
}
