using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DotnetGit.Utils;

public class ObjectStore(string? repoPath)
{
    private readonly string objectsPath = Path.Combine(repoPath, ".dotnetgit", "Objects");

    /// <summary>
    /// Store file content as Blob, then return Hash
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public string WriteBlob(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        var content = File.ReadAllBytes(filePath);
        var hash = ComputeHash(content);

        var dir = Path.Combine(objectsPath, hash.Substring(0, 2));
        var fileName = hash.Substring(2);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, fileName);

        if (!File.Exists(fullPath))
            File.WriteAllBytes(fullPath, content);

        return hash;
    }

    /// <summary>
    /// Hash file Content
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private string ComputeHash(byte[] content)
    {
        using var sha1 = SHA1.Create();

        var hashBytes = sha1.ComputeHash(content);
        var sb = new StringBuilder();

        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }
}