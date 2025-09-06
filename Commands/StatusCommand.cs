using System;
using System.IO;
using System.Linq;

namespace DotnetGit.Commands;

public class StatusCommand(string repoPath) : ICommand
{
    public string Name => "status";

    private readonly string indexPath = Path.Combine(repoPath, ".dotnetgit", "index");

    public void Execute(string[] args)
    {
        var stagedFiles = File.Exists(indexPath)
            ? File.ReadAllLines(indexPath).Select(l => l.Split(' ')[1]).ToHashSet()
            : new HashSet<string>();

        var allFiles = Directory
            .GetFiles(repoPath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(repoPath, f))
            .Where(f => !f.StartsWith(".dotnetgit"))
            .ToList();

        var untracked = allFiles.Where(f => !stagedFiles.Contains(f)).ToList();
        var modified = stagedFiles.Where(f =>
        {
            var filePath = Path.Combine(repoPath, f);

            if (!File.Exists(filePath))
                return false;

            var stagedHash = File
                .ReadAllLines(indexPath)
                .First(l => l.Split(' ')[1] == f)
                .Split(' ')[0];

            var currentContent = File.ReadAllBytes(filePath);

            using var sha1 = System.Security.Cryptography.SHA1.Create();

            var currentHash = string.Concat(sha1.ComputeHash(currentContent).Select(b => b.ToString("x2")));

            return currentHash != stagedHash;
        }).ToList();

        Console.WriteLine("On branch master\n");

        Console.WriteLine("Staged files:");
        foreach (var f in stagedFiles)
            Console.WriteLine($"  {f}");

        Console.WriteLine("\nModified files:");
        foreach (var f in modified)
            Console.WriteLine($"  {f}");

        Console.WriteLine("\nUntracked files:");
        foreach (var f in untracked)
            Console.WriteLine($"  {f}");
    }
}