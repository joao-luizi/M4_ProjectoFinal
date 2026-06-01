namespace RideReadyAPI.Features.Auth.DTOs
{
    public sealed record LoginResponse(
        string Token,
        string Email
    );
}
