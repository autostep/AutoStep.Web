namespace AutoStep.Web.ElementChain
{
    internal class ElementChainOptions
    {
        public int RetryDelayMs { get; set; }

        public int TotalWaitTimeoutMs { get; set; }

        public int PageWaitTimeoutMs { get; set; }
    }
}
