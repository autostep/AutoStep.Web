namespace AutoStep.Web.Chain
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementChainOptions
    {
        public int RetryDelayMs { get; set; }

        public int TotalWaitTimeoutMs { get; set; }

        public int PageWaitTimeoutMs { get; set; }
    }
}
