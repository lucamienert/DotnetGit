using DotnetGit.Commands;

var commands = new Dictionary<string, ICommand>
{
    { "init", new InitCommand() },
};

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnetgit <command> [options]");
    return;
}

var cmd = args[0];

if (commands.TryGetValue(cmd, out var command))
{
    command.Execute(args);
}
else
{
    Console.WriteLine($"Unknown command: {cmd}");
}
