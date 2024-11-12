namespace Utility
{
    class Logger
    {
        private readonly string logFilePath;

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        /// <summary>
        /// Logs any information to log file and console in choosen color.
        /// If no color is specified, prints in default console color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">Color to print to console with.</param>
        public void LogInfo(string message, ConsoleColor color = ConsoleColor.Gray) => Log($"INFO: {message}", color);

        /// <summary>
        /// Logs a warning to log file and console in yellow.
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(string message) => Log($"WARNING: {message}", ConsoleColor.DarkYellow);

        /// <summary>
        /// Logs an error to log file and console in red.
        /// </summary>
        /// <param name="message"></param>
        public void LogError(string message) => Log($"ERROR: {message}", ConsoleColor.Red);

        /// <summary>
        /// Prints a line to both console and log file with current time.
        /// </summary>
        /// <param name="message">Message to print.</param>
        /// <param name="color">Color to print to console with.</param>
        private void Log(string message, ConsoleColor color)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            ColorfulConsole.WriteLineColored(logEntry, color);
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
    }
}