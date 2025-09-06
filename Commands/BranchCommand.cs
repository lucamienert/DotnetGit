using System;
using System.IO;
using System.Linq;

namespace DotnetGit.Commands;

public class BranchCommand(string repoPath) : ICommand
{
    public string Name => "branch";

    private readonly string refsHeadsPath = Path.Combine(repoPath, ".dotnetgit", "refs", "heads");

    public void Execute(string[] args)
    {
        if (!Directory.Exists(refsHeadsPath))
            Directory.CreateDirectory(refsHeadsPath);

        var currentBranch = "master";
        
        if (args.Length == 0)
        {
            var branches = Directory
                .GetFiles(refsHeadsPath)
                .Select(f => Path.GetFileName(f))
                .ToList();

            foreach (var branch in branches)
            {
                if (branch == currentBranch)
                    Console.WriteLine($"* {branch}");
                else
                    Console.WriteLine($"  {branch}");
            }
        }
        else
        {
            var newBranch = args[0];
            var masterRef = Path.Combine(refsHeadsPath, "master");
            var newBranchRef = Path.Combine(refsHeadsPath, newBranch);

            if (File.Exists(newBranchRef))
            {
                Console.WriteLine($"Branch {newBranch} already exists.");
                return;
            }

            var hash = File.Exists(masterRef) ? File.ReadAllText(masterRef) : "";

            File.WriteAllText(newBranchRef, hash);
            Console.WriteLine($"Branch {newBranch} created at {hash.Substring(0, 7)}");
        }
    }
}