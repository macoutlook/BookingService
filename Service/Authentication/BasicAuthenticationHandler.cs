using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core.Persistence.Contract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Service.Authentication;

internal sealed class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IUserRepository userRepository)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private readonly ILogger<BasicAuthenticationHandler> _logger =
        loggerFactory.CreateLogger<BasicAuthenticationHandler>();
    private const string? ResponseContentType = "application/problem+json";
    private static JsonSerializerOptions SerializerOptions => new(JsonSerializerDefaults.Web);

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var auth = Request.Headers["Authorization"];
        if (auth.IsNullOrEmpty())
        {
            Response.StatusCode = 401;
            return AuthenticateResult.Fail("Missing token");
        }

        var authHeader = AuthenticationHeaderValue.Parse(auth!);

        if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase) || !await IsTokenValid(authHeader))
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
        Response.ContentType = ResponseContentType;
        
        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Authentication failed. Please provide a valid token.",
            Status = 401
        };

        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails, SerializerOptions));
    }

    private async Task<bool> IsTokenValid(AuthenticationHeaderValue authHeader)
    {
        if (authHeader.Parameter is null)
        {
            _logger.LogWarning("No token provided.");
            return false;
        }
        if(!TryParseToken(authHeader.Parameter, out var credentials))
            return false;
        var user = await userRepository.GetUserByName(credentials.userName);

        return user is not null && user.Password.Equals(credentials.password);
    }

    private bool TryParseToken(string token, out (string userName, string password) credentials)
    {
        // Validate and decode Base64
        credentials = (string.Empty, string.Empty);
        Span<byte> credentialBytes = new byte[token.Length];
        var wasTokenParsed = Convert.TryFromBase64String(token, credentialBytes, out int bytesWritten);


        if (wasTokenParsed is false || credentialBytes.Length == 0)
        {
            _logger.LogWarning("Invalid base64 token string.");
            return false;
        }

        var credentialsArray = System.Text.Encoding.UTF8.GetString(credentialBytes.Slice(0, bytesWritten)).Split(':', 2);

        if (credentialsArray.Length != 2)
        {
            _logger.LogWarning("Token must be in 'username:password' format");
            return false;
        }
        
        credentials = (credentialsArray[0], credentialsArray[1]);
        
        return true;
    }
}