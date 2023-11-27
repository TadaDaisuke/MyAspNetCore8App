using Serilog.Core;
using Serilog.Events;

namespace MyAspNetCore8App.Logging;

/// <summary>
/// HttpContext情報出力用のSerilogカスタムエンリッチャー
/// </summary>
/// <param name="httpContextAccessor">HttpContextアクセス用クラス</param>
public class HttpContextEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    /// <inheritdoc/>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        if (!(httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false))
        {
            return;
        }
        var userName = httpContextAccessor.HttpContext.User.Identities.First().Claims.FirstOrDefault(x => x.Type.EndsWith("/name"))?.Value.ToString();
        var userNameProperty = factory.CreateProperty("UserName", userName);
        logEvent.AddPropertyIfAbsent(userNameProperty);
        var ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        var ipAddressProperty = factory.CreateProperty("IpAddress", string.IsNullOrWhiteSpace(ipAddress) ? "UnknownAddress" : ipAddress);
        logEvent.AddPropertyIfAbsent(ipAddressProperty);
    }
}
