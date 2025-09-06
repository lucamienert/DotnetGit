namespace DotnetGit.Commands;

public interface ICommand
{
    public string Name { get; }

    public void Execute(string[] args);
}