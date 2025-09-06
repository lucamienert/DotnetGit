using System;
using System.IO;

namespace DotnetGit.Utils;

public class FileUtils
{
    public void CreateFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public void CreateFile(string path, string? content = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        if (!File.Exists(path))
            File.WriteAllText(path, content ?? string.Empty);
    }
}