using DotnetGit.Utils;

using System;
using System.IO;
using System.Text;

namespace DotnetGit.Commands;

public class CommitCommand(string repoPath) : ICommand
{
    public string Name => "commit";

    private readonly ObjectStore objectStore = new ObjectStore(repoPath);
    private readonly string indexPath = Path.Combine(repoPath, ".dotnetgit", "index");
    private readonly string refsHeadsPath = Path.Combine(repoPath, ".dotnetgit", "refs", "heads", "master");

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnetgit commit <message>");
            return;
        }

        var message = args[0];

        if (!File.Exists(indexPath) || new FileInfo(indexPath).Length == 0)
        {
            Console.WriteLine("Nothing to commit. The index is empty.");
            return;
        }

        var stagedLines = File.ReadAllLines(indexPath);
        var treeBuilder = new StringBuilder();

        foreach (var line in stagedLines)
        {
            treeBuilder.AppendLine(line);
        }

        var parentHash = "";

        if (File.Exists(refsHeadsPath))
        {
            parentHash = File.ReadAllText(refsHeadsPath).Trim();
        }

        var commitBuilder = new StringBuilder();
        commitBuilder.AppendLine("tree:");
        commitBuilder.Append(treeBuilder);

        if (!string.IsNullOrEmpty(parentHash))
            commitBuilder.AppendLine($"parent: {parentHash}");

        commitBuilder.AppendLine($"message: {message}");

        var commitHash = objectStore.WriteBlob(CreateTempFile(commitBuilder.ToString()));

        File.WriteAllText(refsHeadsPath, commitHash);

        Console.WriteLine($"[master {commitHash.Substring(0, 7)}] {message}");

        File.WriteAllText(indexPath, string.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private string CreateTempFile(string content)
    {
        string tempFile = Path.Combine(Path.GetTempPath(), $"commit{Guid.NewGuid()}.txt");
        File.WriteAllText(tempFile, content);

        return tempFile;
    }
}