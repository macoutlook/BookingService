using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Core.Persistence.Contract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Service.Authentication;

internal sealed class BasicAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IUserRepository userRepository)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var auth = Request.Headers["Authorization"];
        if (auth.IsNullOrEmpty())
        {
            Response.StatusCode = 401;
            return AuthenticateResult.Fail("Missing token");
        }

        var authHeader = AuthenticationHeaderValue.Parse(auth!);

        if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase) || !await IsValidToken(authHeader))
        {
            Response.StatusCode = 401;
            return AuthenticateResult.Fail("Invalid token");
        }

        // Create user claims and principal
        var claims = new[] { new Claim(ClaimTypes.Name, "User") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        await Response.WriteAsync("{\"error\": \"Authentication failed. Please provide a valid token.\"}");
    }

    private async Task<bool> IsValidToken(AuthenticationHeaderValue authHeader)
    {
        var (userName, password) = ParseToken(authHeader);
        var user = await userRepository.GetUserByName(userName);

        if (user is null)
            return false;

        return user.Password.Equals(password);
    }

    private (string UserName, string Password) ParseToken(AuthenticationHeaderValue authHeader)
    {
        // Validate and decode Base64
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");

        if (credentialBytes.Length == 0) throw new ArgumentException("Invalid Base64 string", nameof(authHeader));

        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

        if (credentials.Length != 2)
            throw new ArgumentException("Token must be in 'username:password' format", nameof(authHeader));

        return (credentials[0], credentials[1]);
    }
}