using StackExchange.Redis;

namespace UptimeMonitor.Services;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" },
        AbortOnConnectFail = false
    });
    private readonly IDatabase _db;

    public RedisService() => _db = _redis.GetDatabase();

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
        var server = _redis.GetServer("localhost", 6379);
        var keys = server.Keys(pattern: "status:*");
        var statuses = new List<object>();
        foreach (var key in keys)
        {
            var url = key.ToString().Replace("status:", "");
            var data = _db.HashGetAll(key);
            statuses.Add(new
            {
                url,
                lastChecked = data.FirstOrDefault(d => d.Name == "lastChecked").Value.ToString(),
                isUp = data.FirstOrDefault(d => d.Name == "isUp").Value.ToString(),
                responseTime = data.FirstOrDefault(d => d.Name == "responseTime").Value.ToString()
            });
        }
        return statuses;
    }

    public object GetFailures()
    {
        var failed = _db.SetMembers("failures").Select(x => x.ToString()).ToList();
        return failed;
    }
}
