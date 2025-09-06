using DotnetGit.Utils;

namespace DotnetGit.Commands;

public class AddCommand(string repoPath) : ICommand
{
    public string Name => "add";

    private readonly ObjectStore objectStore = new ObjectStore(repoPath);
    private readonly FileUtils fileUtils = new FileUtils();
    private readonly string indexPath = Path.Combine(repoPath, ".dotnetgit", "index");

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnetgit add <file1> [file2 ...]");
            return;
        }

        foreach (var path in args)
        {
            var fullPath = Path.Combine(repoPath, path);

            if (File.Exists(fullPath))
            {
                AddFile(fullPath, path);
            }
            else if (Directory.Exists(fullPath))
            {
                AddDirectory(fullPath, path);
            }
            else
            {
                Console.WriteLine($"Path not found: {path}");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="blobHash"></param>
    private void StageFile(string filePath, string blobHash)
    {
        var entry = $"{blobHash} {filePath}";

        if (!File.Exists(indexPath))
            fileUtils.CreateFile(indexPath, entry + Environment.NewLine);
        else
            File.AppendAllText(indexPath, entry + Environment.NewLine);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="relativePath"></param>
    private void AddFile(string fullPath, string relativePath)
    {
        var hash = objectStore.WriteBlob(fullPath);

        StageFile(relativePath, hash);

        Console.WriteLine($"Added {relativePath} ({hash.Substring(0, 7)})");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="relativePath"></param>
    private void AddDirectory(string fullPath, string relativePath)
    {
        foreach (var file in Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories))
        {
            var relPath = Path.GetRelativePath(repoPath, file);
            AddFile(file, relPath);
        }
    }
}