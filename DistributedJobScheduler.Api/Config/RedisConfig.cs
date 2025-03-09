namespace DistributedJobScheduler.Api.Config
{
    public class RedisConfig
{
    public string? ConnectionString { get; set; }
    public string? ChannelPrefix { get; set; }
}
}