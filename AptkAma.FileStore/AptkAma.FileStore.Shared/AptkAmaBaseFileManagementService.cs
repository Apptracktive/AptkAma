using System.IO;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Data
{
    public abstract class AptkAmaBaseFileManagementService : IAptkAmaFileManagementService
    {
        protected readonly string RootFullPath;
        protected readonly string BaseShortPath;
        protected readonly string BaseFullPath;

        protected AptkAmaBaseFileManagementService(string baseShortPath)
        {
            BaseShortPath = baseShortPath;

#if __IOS__ || __ANDROID__
            RootFullPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#else
            RootFullPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

            BaseFullPath = Path.Combine(RootFullPath, BaseShortPath);
        }

        public virtual string GetFullPath(string path)
        {
            return path.ToLower().Contains(RootFullPath.ToLower()) ? path : Path.Combine(BaseFullPath, path);
        }

        public abstract Task CopyFileToStoreAsync(string itemId, string sourceFileFullPath);

        public abstract Task EnsureFolderExistsAsync(string folderShortPath);

        public abstract Task DeleteFileAsync(string fileShortPath);
    }
}
