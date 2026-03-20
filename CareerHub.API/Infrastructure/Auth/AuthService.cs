using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CareerHub.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CareerHub.API.Infrastructure.Auth;

public sealed class AuthService(
    CareerHubContext db,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var login = await db.SecurityLogins
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Login == request.UserName, ct);

        if (login is null || login.IsInactive || login.IsLocked)
            return null;

        if (!VerifyHash(request.Password, login.Password))
            return null;

        var roles = await db.SecurityLoginsRoles
            .AsNoTracking()
            .Where(r => r.LoginId == login.Id)
            .Select(r => r.Role!.Role)
            .ToListAsync(ct);

        return BuildTokenResponse(login.Id, login.Login, login.EmailAddress, roles);
    }

    // -------------------------------------------------------------------------
    // SHA-512 legacy hash verification
    //
    // Original format (must match exactly — existing DB passwords use this):
    //   stored = Base64( SHA512(plaintext + salt) ++ salt )
    //   where salt = last (storedBytes.Length - 64) bytes of the decoded value
    //   and   64   = SHA-512 output size in bytes
    // -------------------------------------------------------------------------

    private static bool VerifyHash(string plainText, string storedHash)
    {
        const int hashSizeInBytes = 64; // SHA-512 = 512 bits = 64 bytes

        byte[] hashWithSalt = Convert.FromBase64String(storedHash);
        if (hashWithSalt.Length < hashSizeInBytes)
            return false;

        // Extract salt appended after the hash bytes
        byte[] salt = hashWithSalt[hashSizeInBytes..];

        string recomputed = ComputeHash(plainText, salt);
        return storedHash == recomputed;
    }

    private static string ComputeHash(string plainText, byte[] salt)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        // Concatenate: plaintext bytes + salt bytes
        byte[] combined = new byte[plainBytes.Length + salt.Length];
        plainBytes.CopyTo(combined, 0);
        salt.CopyTo(combined, plainBytes.Length);

        // SHA-512 hash  (.NET 5+ static method — replaces deprecated SHA512Managed)
        byte[] hash = SHA512.HashData(combined);

        // Store format: hash bytes ++ salt bytes → Base64
        byte[] result = new byte[hash.Length + salt.Length];
        hash.CopyTo(result, 0);
        salt.CopyTo(result, hash.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Generates a new random salt using the modern API.
    /// Replaces: new RNGCryptoServiceProvider().GetNonZeroBytes(salt)
    /// </summary>
    public static byte[] GenerateSalt(int length = 10)
        => RandomNumberGenerator.GetBytes(length);

    /// <summary>
    /// Hashes a new plain-text password (use when creating/resetting passwords).
    /// </summary>
    public static string HashNewPassword(string plainText)
        => ComputeHash(plainText, GenerateSalt());

    // -------------------------------------------------------------------------
    // JWT generation
    // -------------------------------------------------------------------------

    private TokenResponse BuildTokenResponse(
        Guid userId, string login, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Name,  login),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key      = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds    = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry   = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer:   _jwt.Issuer,
            audience: _jwt.Audience,
            claims:   claims,
            expires:  expiry,
            signingCredentials: creds);

        var accessToken  = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExpiry = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays);

        return new TokenResponse(accessToken, refreshToken, expiry, refreshExpiry);
    }
}
