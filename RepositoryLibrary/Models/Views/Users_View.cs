namespace RepositoryLibrary.Models.Views
{
    public class Users_View
    {
        public string Id { get; set; }
        public string? UserName { get; set; } 
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime Birthdate { get; set; }
        public int CitizenNumber { get; set; }
        public bool ImageAuthorized { get; set; }
        public bool InformationAuthorized { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
        public int SocialHealthNumber { get; set; }
        public int TaxIdentificationNumber { get; set; }
        public string? RoleName { get; set; }
        public string? NormalizedRoleName { get; set; }
    }
}
