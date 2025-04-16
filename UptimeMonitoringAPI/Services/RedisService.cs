using StackExchange.Redis;
using UptimeMonitoringAPI.Models;

namespace UptimeMonitoringAPI.Services;

public class RedisService : IRedisService
{

    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { "redis:6379" },
        AbortOnConnectFail = false
    });
    private readonly IDatabase _db;

    public RedisService() {
        Console.WriteLine($"Redis connection test : {_redis.IsConnected}");
        _db = _redis.GetDatabase();
    }

    public async Task SaveStatus(string url, bool isUp, double responseTime)
    {
        await _db.HashSetAsync($"status:{url}", new HashEntry[] {
            new("lastChecked", DateTime.UtcNow.ToString("o")),
            new("isUp", isUp),
            new("responseTime", responseTime)
        });
        if (!isUp)
            await _db.SetAddAsync("failures", url);
        else
            await _db.SetRemoveAsync("failures", url);
    }

    public object GetAllStatuses()
    {
        var server = _redis.GetServer("redis", 6379);
        var keys = server.Keys(pattern: "status:*");
        var statuses = new List<ServiceStatus>();
        foreach (var key in keys)
        {
            var url = key.ToString().Replace("status:", "");
            var data = _db.HashGetAll(key);
            statuses.Add(new ServiceStatus(
                url,
                data.FirstOrDefault(d => d.Name == "isUp").Value.ToString(),
                data.FirstOrDefault(d => d.Name == "lastChecked").Value.ToString(),
                data.FirstOrDefault(d => d.Name == "responseTime").Value.ToString()
            ));
        }
        return System.Text.Json.JsonSerializer.Serialize(statuses);
    }

    public object GetFailures()
    {
        var failed = _db.SetMembers("failures").Select(x => x.ToString()).ToList();
        return failed;
    }
}
