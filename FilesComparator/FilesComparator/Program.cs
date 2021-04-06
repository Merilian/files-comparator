using System;
using System.Threading.Tasks;
using FilesComparator.FileInfo;
using Microsoft.Extensions.DependencyInjection;

namespace FilesComparator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Root folder to find and compare files is required as command line argument");
                return;
            }

            var rootFolder = args[0];

            var serviceProvider = ConfigureServices();
            var fileService = serviceProvider.GetService<IFileService>();

            foreach (var files in await  fileService.GetEqualFilesGroups(rootFolder))
            {
                foreach (var file in files)
                {
                    Console.WriteLine(file.Paths);
                }
                Console.WriteLine();
            }
            
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddTransient<IFileService, FileService>()
                .AddTransient<IFileInfoProvider, FileInfoProvider>()
                .BuildServiceProvider();
        }
    }
}