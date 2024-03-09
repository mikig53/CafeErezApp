using BetModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiTelesportApp.Data;
public class FileSystemService : IFileSystemService
{
    public string GetAppDataDirectory()
    {
        var outputDirectory = FileSystem.AppDataDirectory;
        
        return outputDirectory;
    }
    // Implement other file system operations as needed...
}
