# .NET GIT

A simple Git-clone version control system written in C# for learning purposes. 
It has basic Git functionality listed below.
It initializes a repository in `.dotnetgit`.

## Supported commands

- init
- add
- commit
- branch
- log
- status
- checkout
- merge

## Examples

### Initialize a repository

```bash
dotnet run -- init
```

### Add files

Note: Folders and Files are supported

```bash
dotnet run -- src/ README.md
```

### Commit changes

```bash
dotnet run -- commit "Initial commit"
```

### Status

```bash
dotnet run -- status
```

### View commit history

```bash
dotnet run -- log
```

### Branching

```bash
dotnet run -- branch test
dotnet run -- checkout test
```

### Merge

```bash
dotnet run -- checkout master
dotnet run -- merge test
```

## Sources

https://github.com/codecrafters-io/build-your-own-x
https://wyag.thb.lt/
https://kushagra.dev/blog/build-git-learn-git/
https://www.leshenko.net/p/ugit/#