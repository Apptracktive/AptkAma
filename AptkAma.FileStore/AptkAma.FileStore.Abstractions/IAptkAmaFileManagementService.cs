using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public interface IAptkAmaFileManagementService
    {
        string GetFullPath(string path);
        Task CopyFileToStoreAsync(string itemId, string sourceFileFullPath);
        Task EnsureFolderExistsAsync(string folderShortPath);
        Task DeleteFileAsync(string fileShortPath);
    }
}
