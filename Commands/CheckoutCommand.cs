using DotnetGit.Utils;

using System;
using System.IO;
using System.Linq;

namespace DotnetGit.Commands;

public class CheckoutCommand(string repoPath) : ICommand
{
    public string Name => "checkout";

    private readonly ObjectStore objectStore = new ObjectStore(repoPath);
    private readonly string headPath = Path.Combine(repoPath, ".dotnetgit", "HEAD");
    private readonly string refsHeadsPath = Path.Combine(repoPath, ".dotnetgit", "refs", "heads");

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnetgit checkout <branch>");
            return;
        }

        var branch = args[0];
        var branchRef = Path.Combine(refsHeadsPath, branch);

        if (!File.Exists(branchRef))
        {
            Console.WriteLine($"Branch '{branch}' does not exist.");
            return;
        }

        var commitHash = File.ReadAllText(branchRef).Trim();

        if (string.IsNullOrEmpty(commitHash))
        {
            Console.WriteLine($"Branch '{branch}' has no commits yet.");
            return;
        }

        File.WriteAllText(headPath, $"ref: refs/heads/{branch}");

        foreach (var file in Directory.GetFiles(repoPath, "*", SearchOption.AllDirectories).Where(f => !f.Contains(".dotnetgit")))
            File.Delete(file);

        CheckoutTree(commitHash, "");

        Console.WriteLine($"Switched to branch '{branch}'");
    }

    private void CheckoutTree(string commitHash, string baseDir)
    {
        var commitContent = objectStore.ReadBlob(commitHash);

        if (commitContent == null)
            return;

        string treeHash = commitContent
            .Split('\n')
            .FirstOrDefault(l => l.StartsWith("tree:"))?
            .Substring("tree:".Length).Trim();

        if (string.IsNullOrEmpty(treeHash))
            return;

        var treeContent = objectStore.ReadBlob(treeHash);

        if (treeContent == null)
            return;

        foreach (var line in treeContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split(' ', 2);

            if (parts.Length != 2)
                continue;

            var blobHash = parts[0];
            var fileName = parts[1];
            var filePath = Path.Combine(repoPath, baseDir, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllBytes(filePath, objectStore.ReadBlobBytes(blobHash));
        }
    }
}