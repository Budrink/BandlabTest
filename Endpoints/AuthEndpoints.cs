using ImageCommentApp.Services;
using Microsoft.AspNetCore.Http;

namespace ImageCommentApp.Endpoints
{
    public static class AuthEndpoints
    {
        // ������-�������� ���������� ����� ������ �� �����
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/api/auth/login", (ITokenService tokenService, string username) =>
            {
                var token = tokenService.GenerateToken(username);
                return Results.Ok(new { Token = token });
            });
        }
    }
}
