using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    //public class AptkAmaFileManagementService : AptkAmaBaseFileManagementService
    //{
    //    public AptkAmaFileManagementService() : base(string.Empty)
    //    {
    //    }

    //    public AptkAmaFileManagementService(string baseShortPath) : base(baseShortPath)
    //    {
    //    }

    //    public override Task EnsureFolderExistsAsync(string folderShortPath)
    //    {
    //        var shortPath = GetShortPath(folderShortPath);

    //        using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
    //        {
    //            if (!isf.DirectoryExists(shortPath))
    //                isf.CreateDirectory(shortPath);
    //        }

    //        return Task.FromResult(true);
    //    }

    //    public override Task DeleteFileAsync(string fileShortPath)
    //    {
    //        var shortPath = GetShortPath(fileShortPath);

    //        using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
    //        {
    //            if(isf.FileExists(shortPath))
    //                isf.DeleteFile(shortPath);
    //        }

    //        return Task.FromResult(true);
    //    }

    //    private string GetShortPath(string path)
    //    {
    //        return path.ToLower().Contains(RootFullPath.ToLower()) ? path.Substring(RootFullPath.Length + 1) : path;
    //    }
    //}
}
