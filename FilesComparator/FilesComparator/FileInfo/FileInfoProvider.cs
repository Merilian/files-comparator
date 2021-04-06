using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FilesComparator.FileInfo
{
    public class FileInfoProvider : IFileInfoProvider
    {
        public async Task<ICollection<FileInfo>> GetFilesInfo(string rootFolder)
        {
            var files = Directory.GetFiles(rootFolder, string.Empty, SearchOption.AllDirectories);
            
            var filesDescription = files.Select(file => new FileInfo
            {
                Paths = file,
                Extension = Path.GetExtension(file),
            }).ToList();

            foreach (var fileDescription in filesDescription)
            {
                fileDescription.DataHash = await GetFileDataHash(fileDescription.Paths);
            }

            return filesDescription;
        }
        
        public async Task<bool> IsFilesDataEqual(string firstFilePaths, string secondFilePaths)
        {
            await using var firstFileStream = File.OpenRead(firstFilePaths);
            await using var secondFileStream = File.OpenRead(secondFilePaths);
            
            var isEqual = firstFileStream.Length == secondFileStream.Length;
            if (!isEqual)
            {
                return false;
            }
            
            const int bufferSize = 1024 * sizeof(long);
            var firstBuffer = new byte[bufferSize];
            var secondBuffer = new byte[bufferSize];

            while (isEqual && firstFileStream.Position < firstFileStream.Length)
            {
                await Task.WhenAll(
                    firstFileStream.ReadAsync(firstBuffer, 0, bufferSize),
                    secondFileStream.ReadAsync(secondBuffer, 0, bufferSize));

                isEqual = firstBuffer.SequenceEqual(secondBuffer);
            }

            return isEqual;
        }
        
        private async Task<string> GetFileDataHash(string filePaths)
        {
            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(filePaths);

            return Encoding.Default.GetString(md5.ComputeHash(stream));
        }
    }
}