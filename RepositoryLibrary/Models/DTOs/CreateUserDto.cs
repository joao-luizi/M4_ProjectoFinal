namespace RepositoryLibrary.Models.DTOs
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
        int SocialHealthName,
        int CitizenNumber,
        DateTime Birthdate,
        string Address,
        int SchoolId
    );
}
