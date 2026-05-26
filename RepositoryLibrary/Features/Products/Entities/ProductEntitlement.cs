namespace RepositoryLibrary.Features.Products.Entities
{
    public class ProductEntitlement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int LessonTypeId { get; set; }
        public LessonType LessonType { get; set; } = null!;

        public int? WeeklyFrequency { get; set; }
        public int? CreditsGranted { get; set; }
    }
}
