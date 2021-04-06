using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesComparator.FileInfo
{
    public interface IFileInfoProvider
    {
        Task<ICollection<FileInfo>> GetFilesInfo(string rootFolder);
        Task<bool> IsFilesDataEqual(string firstFilePaths, string secondFilePaths);
    }
}