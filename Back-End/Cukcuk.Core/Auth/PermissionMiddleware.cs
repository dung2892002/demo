using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Cukcuk.Core.Auth
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var permissionAttribute = endpoint.Metadata.GetMetadata<PermissionAttribute>();
                if (permissionAttribute != null)
                {
                    
                    var userRoles = context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                    if (!userRoles.Contains("Admin"))
                    {
                        var username = context.User?.FindFirst(ClaimTypes.Name)?.Value;
                        var hasPermission = await permissionService.CheckUserPermission(username, permissionAttribute.Permission);

                        if (!hasPermission)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync("Bạn không có quyền thực hiện chức năng này, liên hệ admin để được trợ giúp!");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
