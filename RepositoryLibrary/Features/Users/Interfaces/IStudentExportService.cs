namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface IStudentExportService
    {
        Task<byte[]> ExportStudentsToExcelAsync(int schoolId);
    }
}