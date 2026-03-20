namespace CareerHub.API.Infrastructure.Auth;

public sealed record LoginRequest(string UserName, string Password);

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    DateTime RefreshTokenExpiry);

public interface IAuthService
{
    /// <summary>
    /// Verifies credentials using the legacy SHA-512 hash stored in the DB.
    /// On success, returns a JWT access token + refresh token.
    /// Returns null if credentials are invalid or the account is locked/inactive.
    /// </summary>
    Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
