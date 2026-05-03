namespace RepositoryLibrary.Models.DTOs
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime Birthdate { get; set; }
        public int CitizenNumber { get; set; }
        public long SocialHealthNumber { get; set; }
        public int TaxIdentificationNumber { get; set; }
        public Photo? Photo { get; set; }
    }
}
