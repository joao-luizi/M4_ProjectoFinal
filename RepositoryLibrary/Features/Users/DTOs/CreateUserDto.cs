namespace RepositoryLibrary.Features.Users.DTOs
{
    public record CreateUserDto
    (
        string FirstName,
        string LastName,
        string Email,
        string Password,
        bool IsActive,
        bool InformationAuthorized,
        bool ImageAuthorized,
        DateTime RegisterDate,
        int TaxIdentificationNumber,
        long SocialHealthNumber,
        int CitizenNumber,
        DateTime Birthdate,
        string Address,
        int SchoolId
    );
}
