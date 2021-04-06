using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesComparator.FileInfo;

namespace FilesComparator
{
    public class FileService : IFileService
    {
        private readonly IFileInfoProvider _fileInfoProvider;

        public FileService(IFileInfoProvider fileInfoProvider)
        {
            _fileInfoProvider = fileInfoProvider;
        }

        public async Task<ICollection<ICollection<FileInfo.FileInfo>>> GetEqualFilesGroups(string rootFolder)
        {
            var fileGroups = new List<ICollection<FileInfo.FileInfo>>();

            var files = await _fileInfoProvider.GetFilesInfo(rootFolder);
            var filesByHashAndExtension = files.GroupBy(f => (f.Extension, f.DataHash));
            
            foreach (var sameHashGroup in filesByHashAndExtension)
            {
                if (sameHashGroup.Count() == 1)
                {
                    fileGroups.Add(sameHashGroup.ToList());
                    continue;
                }
                
                var equalFiles = await GroupFilesByData(sameHashGroup.ToList());
                fileGroups.AddRange(equalFiles.ToList());
            }

            return fileGroups;
        }

        private async Task<ICollection<ICollection<FileInfo.FileInfo>>> GroupFilesByData(IEnumerable<FileInfo.FileInfo> files)
        {
            var equalFiles = new Dictionary<FileInfo.FileInfo, ICollection<FileInfo.FileInfo>>();

            foreach (var file in files)
            {
                var keyFounded = false;
                foreach (var filesKey in equalFiles.Keys)
                {
                    if (await _fileInfoProvider.IsFilesDataEqual(filesKey.Paths, file.Paths))
                    {
                        equalFiles[filesKey].Add(file);
                        keyFounded = true;
                    }
                }

                if (!keyFounded)
                {
                    equalFiles[file] = new List<FileInfo.FileInfo>{file};
                }
            }

            return equalFiles.Values.ToList();
        }
    }
}