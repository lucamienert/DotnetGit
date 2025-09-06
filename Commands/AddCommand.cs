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

        foreach (var filePath in args)
        {
            var fullPath = Path.Combine(repoPath, filePath);

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"File not found: {filePath}");
                continue;
            }

            var blobHash = objectStore.WriteBlob(fullPath);

            StageFile(filePath, blobHash);

            Console.WriteLine($"Added {filePath} ({blobHash})");
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
}