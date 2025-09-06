using DotnetGit.Utils;

using System;
using System.IO;
using System.Text;

namespace DotnetGit.Commands;

public class LogCommand(string repoPath) : ICommand
{
    public string Name => "log";

    private readonly ObjectStore objectStore = new ObjectStore(repoPath);
    private readonly string refsHeadsPath = Path.Combine(repoPath, ".dotnetgit", "refs", "heads", "master");

    public void Execute(string[] args)
    {
        if (!File.Exists(refsHeadsPath))
        {
            Console.WriteLine("No commits yet.");
            return;
        }

        var commitHash = File.ReadAllText(refsHeadsPath).Trim();

        if (string.IsNullOrEmpty(commitHash))
        {
            Console.WriteLine("No commits yet.");
            return;
        }

        Console.WriteLine("Commit history:");

        while (!string.IsNullOrEmpty(commitHash))
        {
            var commitContent = ReadCommitContent(commitHash);

            if (commitContent == null)
                break;

            var message = ParseCommitMessage(commitContent);
            Console.WriteLine($"commit {commitHash.Substring(0, 7)}");
            Console.WriteLine($"    {message}\n");

            commitHash = ParseParentHash(commitContent);
        }
    }

    /// <summary>
    /// Extracts the commit Message
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    private string? ReadCommitContent(string hash)
    {
        var dir = Path.Combine(repoPath, ".dotnetgit", "Objects", hash.Substring(0, 2));
        var file = Path.Combine(dir, hash.Substring(2));

        if (!File.Exists(file))
            return null;

        return File.ReadAllText(file);
    }

    /// <summary>
    /// Helper method to parse message
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private string ParseCommitMessage(string content)
    {
        var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("message:"))
                return line.Substring("message:".Length).Trim();
        }

        return "(no message)";
    }

    /// <summary>
    /// Helper method to parse parent Hash
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private string? ParseParentHash(string content)
    {
        var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("parent:"))
                return line.Substring("parent:".Length).Trim();
        }

        return null;
    }
}