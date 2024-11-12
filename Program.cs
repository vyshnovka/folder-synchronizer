using Utility;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            ColorfulConsole.WriteLineColored("Too few or too many arguments!", ConsoleColor.Red);
            Console.WriteLine("Usage: FolderSynchronizer.exe <sourcePath> <replicaPath> <logFilePath> <syncInterval>");
            return;
        }

        var sourcePath = args[0];
        var replicaPath = args[1];
        var logFilePath = args[2];

        if (!int.TryParse(args[3], out var syncInterval) || syncInterval <= 0)
        {
            ColorfulConsole.WriteLineColored("Invalid synchronization interval. Provide a positive integer.", ConsoleColor.Red);
            return;
        }

        var synchronizer = new FolderSynchronizer(sourcePath, replicaPath, logFilePath, syncInterval);
        synchronizer.StartSynchronization();
    }
}