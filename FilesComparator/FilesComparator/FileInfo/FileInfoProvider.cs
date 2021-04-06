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
        private const int ReadBufferSize = 1024 * sizeof(long);

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
            
            var firstBuffer = new byte[ReadBufferSize];
            var secondBuffer = new byte[ReadBufferSize];

            while (isEqual && firstFileStream.Position < firstFileStream.Length)
            {
                await Task.WhenAll(
                    firstFileStream.ReadAsync(firstBuffer, 0, ReadBufferSize),
                    secondFileStream.ReadAsync(secondBuffer, 0, ReadBufferSize));

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