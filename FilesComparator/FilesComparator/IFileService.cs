using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesComparator
{
    public interface IFileService
    {
        Task<ICollection<ICollection<FileInfo.FileInfo>>> GetEqualFilesGroups(string rootFolder);
    }
}