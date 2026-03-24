using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DeliverySystem.Infrastructure.AccessContext;

public sealed class HttpHeaderAccessContext : IAccessContext
{
    private const string RoleHeader = "X-User-Role";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpHeaderAccessContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public UserRole GetCurrentRole()
    {
        var headerValue = _httpContextAccessor.HttpContext?.Request.Headers[RoleHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(headerValue))
            return UserRole.None;

        return Enum.TryParse<UserRole>(headerValue, ignoreCase: true, out var role)
            ? role
            : UserRole.None;
    }
}
