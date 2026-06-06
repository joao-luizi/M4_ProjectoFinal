using DocumentFormat.OpenXml.Office.PowerPoint.Y2019.Main.Command;
using RepositoryLibrary.Features.Purchases.DTOs;
using RepositoryLibrary.Features.Purchases.Entities;
using RepositoryLibrary.Features.Purchases.Snapshot;

namespace RepositoryLibrary.Features.Purchases.Interfaces
{
    public interface IPurchaseService
    {
        //V2 implemented
        Task<List<PurchaseStudentSummary>> GetStudentPurchaseSummaryAsync();
        //V2 implemented
        Task<List<PurchaseHistoryDto>> GetStudentPurchaseHistoryAsync(string studentId);
        //V2 implemented
        Task CreatePurchaseAsync(CreatePurchaseDto dto);
    }
}
