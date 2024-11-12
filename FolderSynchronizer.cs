using System.Security.Cryptography;
using Utility;

class FolderSynchronizer
{
    private readonly string sourcePath;
    private readonly string replicaPath;
    private readonly int syncInterval;
    private readonly Logger logger;

    public FolderSynchronizer(string sourcePath, string replicaPath, string logFilePath, int syncInterval)
    {
        this.sourcePath = sourcePath;
        this.replicaPath = replicaPath;
        this.logger = new Logger(logFilePath);
        this.syncInterval = syncInterval;
    }

    public void StartSynchronization()
    {
        while (true)
        {
            logger.LogInfo($"Starting synchronization...");

            SyncDirectory(sourcePath, replicaPath);

            Thread.Sleep(syncInterval * 1000);
        }
    }

    /// <summary>
    /// Synchronizes directories and its content (if source directory exists).
    /// </summary>
    /// <param name="source">Sorce directory path.</param>
    /// <param name="replica">Replica directory path.</param>
    private void SyncDirectory(string source, string replica)
    {
        if (!Directory.Exists(source))
        {
            logger.LogError("Source folder does not exist. Exiting synchronization...");
            return;
        }

        CreateDirectoryIfMissing(replica);

        SyncContent(source, replica);
        RemoveExtraContent(replica, source);

        logger.LogInfo("Synchronization completed!");
    }

    /// <summary>
    /// Synchronizes subdirectories and files in given directories (if access is granted).
    /// </summary>
    /// <param name="sourceDirectory">Source directory path.</param>
    /// <param name="replicaDirectory">Replica directory path.</param>
    private void SyncContent(string sourceDirectory, string replicaDirectory)
    {
        if (!HasAccess(sourceDirectory, true))
        {
            logger.LogWarning($"Skipping: {sourceDirectory}");
            return;
        }

        // Synchronizing files in directories
        foreach (var sourceFile in Directory.GetFiles(sourceDirectory))
        {
            var sourceFileName = Path.GetFileName(sourceFile);
            var replicaFile = Path.Combine(replicaDirectory, sourceFileName);

            if (ShouldCopyFile(sourceFile, replicaFile))
            {
                if (!HasAccess(sourceFile))
                {
                    logger.LogWarning($"Skipping: {sourceFile}");
                    continue;
                }

                File.Copy(sourceFile, replicaFile, true);
                logger.LogInfo($"Created/Updated file: {replicaFile}");
            }
        }

        // Synchronizing directories
        foreach (var directory in Directory.GetDirectories(sourceDirectory))
        {
            var subDirectoryName = Path.GetFileName(directory);
            var replicaSubDirectory = Path.Combine(replicaDirectory, subDirectoryName);
            CreateDirectoryIfMissing(replicaSubDirectory);
            SyncContent(directory, replicaSubDirectory);
        }
    }

    private void RemoveExtraContent(string replica, string source)
    {
        // Removing unnecessary files
        foreach (var file in Directory.GetFiles(replica))
        {
            var fileName = Path.GetFileName(file);
            var sourceFile = Path.Combine(source, fileName);
            if (!File.Exists(sourceFile))
            {
                File.Delete(file);
                logger.LogInfo($"Deleted file: {file}");
            }
        }

        // Removing unnecessary directories
        foreach (var directory in Directory.GetDirectories(replica))
        {
            var dirName = Path.GetFileName(directory);
            var sourceDir = Path.Combine(source, dirName);
            if (!Directory.Exists(sourceDir))
            {
                Directory.Delete(directory, true);
                logger.LogInfo($"Deleted directory: {directory}");
            }
        }
    }

    /// <summary>
    /// Checks if source file should be copied (replica file does not exist or is not identical to source).
    /// </summary>
    /// <param name="sourceFile">Source file path.</param>
    /// <param name="replicaFile">Replica file path.</param>
    /// <returns>Returns true if replica file exists and is identical to the source.</returns>
    private bool ShouldCopyFile(string sourceFile, string replicaFile)
    {
        return !File.Exists(replicaFile) || !FilesAreIdentical(sourceFile, replicaFile);
    }

    /// <summary>
    /// Checks if given files are identical using hash values.
    /// </summary>
    /// <param name="file1">First file path.</param>
    /// <param name="file2">Second file path.</param>
    /// <returns>Returns true if files are identical.</returns>
    private bool FilesAreIdentical(string file1, string file2)
    {
        var hashAlgorithm = MD5.Create();

        using var stream1 = File.Open(file1, FileMode.Open, FileAccess.Read);
        using var stream2 = File.Open(file2, FileMode.Open, FileAccess.Read);

        return hashAlgorithm.ComputeHash(stream1).SequenceEqual(hashAlgorithm.ComputeHash(stream2));
    }

    /// <summary>
    /// Checks if given directory exists and creates directory if it is missing, logging it.
    /// </summary>
    /// <param name="path">Path to directory.</param>
    private void CreateDirectoryIfMissing(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            logger.LogInfo($"Created directory: {path}");
        }
    }

    /// <summary>
    /// Checks access permisions to a given item (directory by default or file if specified).
    /// </summary>
    /// <param name="path">Path to item.</param>
    /// <param name="isDirectory">If item is a directory.</param>
    /// <returns>Returns true if the access is granted.</returns>
    private bool HasAccess(string path, bool isDirectory = false)
    {
        try
        {
            if (isDirectory)
            {
                // Checking directory access by trying to get files from it
                var files = Directory.GetFiles(path);
            }
            else
            {
                // Checking file access by immitating opening it
                using var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            }

            return true;
        }
        catch (Exception)
        {
            logger.LogWarning($"Access denied: {path}");
        }

        return false;
    }
}
