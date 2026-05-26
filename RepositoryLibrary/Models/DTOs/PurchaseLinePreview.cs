namespace RepositoryLibrary.Models.DTOs
{
    public class PurchaseLinePreview
    {
        public int PackageId { get; set; }

        public string Name { get; set; } = "";

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total => UnitPrice * Quantity;

        public int ClassesIncluded { get; set; }

        public bool Weekly { get; set; }
    }
}