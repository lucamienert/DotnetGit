namespace DotnetGit.Commands;

public interface ICommand
{
    public void Execute(string[] args);
}