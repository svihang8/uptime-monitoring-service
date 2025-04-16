namespace UptimeMonitor.Services;

public interface IRedisService
{
    Task SaveStatus(string url, bool isUp, double responseTime);
    object GetAllStatuses();
    object GetFailures();
}