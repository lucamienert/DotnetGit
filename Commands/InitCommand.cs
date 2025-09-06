using DotnetGit.Utils;

namespace DotnetGit.Commands;

public class InitCommand : ICommand
{
    public string Name => "init";

    private readonly FileUtils fileUtils = new FileUtils();

    public void Execute(string[] args)
    {
        var repoPath = Environment.CurrentDirectory;
        var myGitPath = Path.Combine(repoPath, ".dotnetgit");

        if (System.IO.Directory.Exists(myGitPath))
        {
            Console.WriteLine("Repository already exists.");
            return;
        }

        fileUtils.CreateFolder(myGitPath);

        fileUtils.CreateFolder(Path.Combine(myGitPath, "Objects"));
        fileUtils.CreateFolder(Path.Combine(myGitPath, "refs"));
        fileUtils.CreateFolder(Path.Combine(myGitPath, "refs", "heads"));

        fileUtils.CreateFile(Path.Combine(myGitPath, "HEAD"), "ref: refs/heads/master");

        fileUtils.CreateFile(Path.Combine(myGitPath, "config"), "[dotnetgit]\n\trepositoryformatversion = 0");
    }
}