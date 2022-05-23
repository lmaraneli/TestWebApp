using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using TestWebApp.Application.Configurations;
using TestWebApp.Interfaces;

namespace TestWebApp.Application
{
    public class FileService : IFileService
    {
        FileServiceOptions _options;

        public FileService(IOptions<FileServiceOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> SaveFileAsync(MemoryStream stream, string ext)
        {
            var name = Guid.NewGuid().ToString();
            var location = Path.GetFullPath($"{_options.SaveLocation}/{name}{ext}");

            await File.WriteAllBytesAsync(location, stream.ToArray());

            return $"{_options.RelativePath}/{name}.{ext}";
        }

        public void DeleteFile(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    File.Delete(name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Could not delete file: ");
                }
            }
        }
    }
}
