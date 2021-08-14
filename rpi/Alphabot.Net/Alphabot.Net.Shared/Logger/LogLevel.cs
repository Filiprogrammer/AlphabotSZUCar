namespace Alphabot.Net.Shared.Logger
{
    public enum LogLevel
    {
        Debug,
        /// <summary>
        /// Process is every normal log Information that isn't Critical or Warning.
        /// </summary>
        Information,

        /// <summary>
        /// Warning means something failed but it isn't a big deal.
        /// </summary>
        Warning,
        Error,
        /// <summary>
        /// Something happened that the program doesn't work like it should work.
        /// </summary>
        Critical,
    }
}
