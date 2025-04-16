namespace UptimeMonitoringAPI.Models;

public class MonitorRequest
{
    public string Url { get; set; } = string.Empty;
    public int IntervalSeconds { get; set; } = 60;
}
