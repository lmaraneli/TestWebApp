using System;
using System.IO;
using System.Threading.Tasks;

namespace TestWebApp.Interfaces
{
    public interface IFileService
    {
        void DeleteFile(string name);
        Task<string> SaveFileAsync(MemoryStream stream, string ext);
    }
}
