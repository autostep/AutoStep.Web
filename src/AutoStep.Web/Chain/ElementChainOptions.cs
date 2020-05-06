namespace AutoStep.Web.Chain
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementChainOptions
    {
        public int RetryDelayMs { get; set; } = 100;

        public int TotalWaitTimeoutMs { get; set; } = 2000;

        public int PageWaitTimeoutMs { get; set; }
    }
}
