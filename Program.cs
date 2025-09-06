using DotnetGit.Commands;

if (args.Length == 0)
{
    ShowUsage();

    return;
}

var commands = new List<ICommand>
{
    new InitCommand()
};

var repoPath = FindRepoPath(Environment.CurrentDirectory);

if (repoPath != null)
{
    commands.Add(new AddCommand(repoPath));
    commands.Add(new CommitCommand(repoPath));
    commands.Add(new LogCommand(repoPath));
}

var cmdName = args[0].ToLower();
var command = commands.Find(c => c.Name == cmdName);

if (command == null)
{
    Console.WriteLine($"Unknown command: {cmdName}");
    ShowUsage();

    return;
}

var commandArgs = args.Length > 1 ? args[1..] : Array.Empty<string>();
command.Execute(commandArgs);

void ShowUsage()
{
    Console.WriteLine("Usage: dotnetgit <command> [options]");
    Console.WriteLine("Commands:");
    Console.WriteLine("  init                Initialize repository");
    Console.WriteLine("  add <file>          Stage file for commit");
    Console.WriteLine("  commit <message>    Commit staged files");
}

string? FindRepoPath(string startDir)
{
    var dir = startDir;

    while (dir != null)
    {
        if (Directory.Exists(Path.Combine(dir, ".dotnetgit")))
            return dir;

        dir = Directory.GetParent(dir)?.FullName;
    }

    return null;
}