/// <summary>
/// a simple regex commit linter example
/// https://www.conventionalcommits.org/en/v1.0.0/
/// https://github.com/angular/angular/blob/22b96b9/CONTRIBUTING.md#type
/// </summary>

using System.Text.RegularExpressions;

try
{
    string pattern = @"^(?=.{1,90}$)(?:build|feat|ci|chore|docs|fix|perf|refactor|revert|style|test)(?:\(.+\))*(?::).{4,}(?:#\d+)*(?<![\.\s])$";
    string commitMessage = File.ReadAllLines(Args[0])[0];

    if (Regex.IsMatch(commitMessage, pattern))
        return 0;

    DisplayErrorMessage(commitMessage);
    return 1;
}
catch (Exception ex)
{
    Console.WriteLine($"Error occurred: {ex.Message}");
    return 1;
}

private static void DisplayErrorMessage(string commitMessage)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nInvalid commit message detected.");
    Console.ResetColor();

    if (!commitMessage.Contains(":"))
    {
        Console.WriteLine("\nError: Message lacks a ':' separator between type and subject.");
        Console.WriteLine("Example: 'feat: add new feature'");
    }
    else if (!Regex.IsMatch(commitMessage, @"^(build|feat|ci|chore|docs|fix|perf|refactor|revert|style|test)"))
    {
        Console.WriteLine("\nError: Message type is invalid or missing.");
        Console.WriteLine("Valid types: build, feat, ci, chore, docs, fix, perf, refactor, revert, style, test.");
    }
    else if (commitMessage.Length > 90)
    {
        Console.WriteLine("\nError: Message is too long. It should be less than 90 characters.\n");
    }

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("For a detailed guide on commit message format, visit:");
    Console.WriteLine("https://www.conventionalcommits.org/en/v1.0.0/");
}