using System.Security.Claims;
using Grpc.Core;

namespace Api.Extensions;

public static class ServerCallContextExtensions
{
    public static string GetAuthorizedUserName(this ServerCallContext context)
        => context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier).Value;
}