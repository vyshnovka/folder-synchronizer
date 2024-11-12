namespace Utility
{
    public static class ColorfulConsole
    {
        /// <summary>
        /// Prints a line to console with the specified text color.
        /// </summary>
        /// <param name="message">Message to print.</param>
        /// <param name="color">Text color.</param>
        public static void WriteLineColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}