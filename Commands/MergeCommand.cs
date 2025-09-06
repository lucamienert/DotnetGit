using DotnetGit.Utils;

using System;
using System.IO;
using System.Linq;

namespace DotnetGit.Commands;

public class MergeCommand(string repoPath) : ICommand
{
    public string Name => "merge";

    private readonly ObjectStore objectStore = new ObjectStore(repoPath);
    private readonly string headPath = Path.Combine(repoPath, ".dotnetgit", "HEAD");
    private readonly string refsHeadsPath = Path.Combine(repoPath, ".dotnetgit", "refs", "heads");

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnetgit merge <branch>");
            return;
        }

        var targetBranch = args[0];
        var targetRef = Path.Combine(_refsHeadsPath, targetBranch);

        if (!File.Exists(targetRef))
        {
            Console.WriteLine($"Branch '{targetBranch}' does not exist.");
            return;
        }

        var headContent = File.ReadAllText(_headPath).Trim();

        if (!headContent.StartsWith("ref: "))
        {
            Console.WriteLine("Detached HEAD is not supported for merge yet.");
            return;
        }

        var currentBranch = headContent.Substring("ref: refs/heads/".Length);
        var currentRef = Path.Combine(_refsHeadsPath, currentBranch);

        var currentHash = File.Exists(currentRef) ? File.ReadAllText(currentRef).Trim() : "";
        var targetHash = File.ReadAllText(targetRef).Trim();

        if (currentHash == targetHash)
        {
            Console.WriteLine("Already up to date.");
            return;
        }

        if (IsAncestor(currentHash, targetHash))
        {
            File.WriteAllText(currentRef, targetHash);
            CheckoutBranch(currentBranch);
            Console.WriteLine($"Fast-forward merged '{targetBranch}' into '{currentBranch}'");
        }
        else
        {
            Console.WriteLine("Non-fast-forward merges are not implemented yet.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ancestorHash"></param>
    /// <param name="descendantHash"></param>
    /// <returns></returns>
    private bool IsAncestor(string ancestorHash, string descendantHash)
    {
        var hash = descendantHash;

        while (!string.IsNullOrEmpty(hash))
        {
            if (hash == ancestorHash)
                return true;

            var content = _objectStore.ReadBlob(hash);

            if (content == null)
                break;

            string parent = content
                .Split('\n')
                .FirstOrDefault(l => l.StartsWith("parent:"))?
                .Substring("parent:".Length).Trim() ?? "";

            hash = parent;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="branch"></param>
    private void CheckoutBranch(string branch)
    {
        var branchRef = Path.Combine(_refsHeadsPath, branch);
        var commitHash = File.ReadAllText(branchRef).Trim();

        if (!string.IsNullOrEmpty(commitHash))
        {
            var checkout = new CheckoutCommand(_repoPath);
            
            checkout.Execute(new string[] { branch });
        }
    }
}