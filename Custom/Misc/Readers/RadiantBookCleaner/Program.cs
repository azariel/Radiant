// See https://aka.ms/new-console-template for more information
using RadiantBookCleaner.Cleaners.LightNovelPub;

Console.WriteLine("Source depot:");
Console.WriteLine("[1]: lightnovelpub.com");
ConsoleKeyInfo _ProviderKey = Console.ReadKey();

string _DirectoryPath = "";
while (!Directory.Exists(_DirectoryPath))
{
    Console.WriteLine("-------------------------------------------------");
    Console.WriteLine("Directory path containing source files:");
    _DirectoryPath = Console.ReadLine();
}

switch (_ProviderKey.Key)
{
    case ConsoleKey.NumPad1:
    case ConsoleKey.D1:
        var _LightNovelPubCleaner = new LightNovelPubCleaner();
        _LightNovelPubCleaner.CleanFilesInDirectory(_DirectoryPath, "*.*", SearchOption.AllDirectories);
        break;
}

Console.WriteLine("Done.");
Console.ReadKey();
Environment.Exit(0);
