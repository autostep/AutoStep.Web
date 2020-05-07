namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Defines execution options for an element chain.
    /// </summary>
    public class ElementChainOptions
    {
        /// <summary>
        /// Gets or sets the number of milliseconds to wait between element chain execution retries.
        /// </summary>
        public int RetryDelayMs { get; set; } = 100;

        /// <summary>
        /// Gets or sets the total retry timeout.
        /// </summary>
        public int TotalRetryTimeoutMs { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the milliseconds to wait for the page to be ready.
        /// </summary>
        public int PageWaitTimeoutMs { get; set; }
    }
}
