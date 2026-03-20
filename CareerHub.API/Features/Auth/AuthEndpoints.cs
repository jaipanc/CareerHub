using CareerHub.API.Infrastructure.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CareerHub.API.Features.Auth;

// ── Request / Response models ─────────────────────────────────────────────────

public sealed record LoginRequestDto(string UserName, string Password);

public sealed record TokenResponseDto(
    string  AccessToken,
    string  RefreshToken,
    DateTime AccessTokenExpiry,
    DateTime RefreshTokenExpiry);

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}

// ── Endpoint registration ─────────────────────────────────────────────────────

public static class AuthEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", LoginAsync)
             .WithName("Login")
             .WithSummary("Authenticate with username and password, receive JWT.")
             .AllowAnonymous()
             .Produces<TokenResponseDto>(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
             .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/refresh", RefreshAsync)
             .WithName("RefreshToken")
             .WithSummary("Exchange a refresh token for a new access token.")
             .AllowAnonymous()
             .Produces<TokenResponseDto>(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────
    private static async Task<IResult> LoginAsync(
        [FromBody]                    LoginRequestDto              dto,
        IValidator<LoginRequestDto>                                validator,
        IAuthService                                               authService,
        CancellationToken                                          ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var result = await authService.LoginAsync(
            new LoginRequest(dto.UserName, dto.Password), ct);

        if (result is null)
            return Results.Problem(
                title:      "Unauthorized",
                detail:     "Invalid credentials or account is locked.",
                statusCode: StatusCodes.Status401Unauthorized);

        return Results.Ok(new TokenResponseDto(
            result.AccessToken,
            result.RefreshToken,
            result.AccessTokenExpiry,
            result.RefreshTokenExpiry));
    }

    // ── POST /api/auth/refresh ────────────────────────────────────────────────
    // Placeholder — full refresh token rotation (stored in DB) to be implemented in Phase 4
    private static IResult RefreshAsync()
        => Results.Problem(
            title:      "Not Implemented",
            detail:     "Refresh token endpoint will be completed in Phase 4.",
            statusCode: StatusCodes.Status501NotImplemented);
}
