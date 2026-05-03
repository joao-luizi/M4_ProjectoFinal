namespace RepositoryLibrary.Models.DTOs
{
    public class UpdateUserDto
    {
        public UpdateUserDto()
        {
            Roles = new List<string>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public bool IsActive { get; set; }
        public int TaxIdentificationNumber { get; set; }
        public long SocialHealthNumber { get; set; }
        public int CitizenNumber { get; set; }
        public IList<string> Roles { get; set; }
        public Photo? Photo { get; set; }
    }
}
