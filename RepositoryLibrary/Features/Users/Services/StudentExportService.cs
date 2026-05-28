using ClosedXML.Excel;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;

namespace RepositoryLibrary.Features.Users.Service
{
    public class StudentExportService : IStudentExportService
    {
        private readonly IUserService _userService;

        public StudentExportService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<byte[]> ExportStudentsToExcelAsync(int schoolId)
        {
            var students = await _userService.GetUsersBySchoolAndRole(schoolId, StaticRole.Student);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Alunos");

            ws.Cell(1, 1).Value = "Nome";
            ws.Cell(1, 2).Value = "Email";
            ws.Cell(1, 3).Value = "Telefone";
            ws.Cell(1, 4).Value = "Data de Nascimento";
            ws.Cell(1, 5).Value = "Morada";
            ws.Cell(1, 6).Value = "NIF";
            ws.Cell(1, 7).Value = "Nº Cartão Cidadão";
            ws.Cell(1, 8).Value = "Nº Segurança Social";
            ws.Cell(1, 9).Value = "Ativo";
            ws.Row(1).Style.Font.Bold = true;

            var row = 2;
            foreach (var s in students)
            {
                ws.Cell(row, 1).Value = s.Name ?? string.Empty;
                ws.Cell(row, 2).Value = s.Email ?? string.Empty;
                ws.Cell(row, 3).Value = s.PhoneNumber ?? string.Empty;
                ws.Cell(row, 4).Value = s.Birthdate.ToString("dd/MM/yyyy");
                ws.Cell(row, 5).Value = s.Address ?? string.Empty;
                ws.Cell(row, 6).Value = s.TaxIdentificationNumber;
                ws.Cell(row, 7).Value = s.CitizenNumber;
                ws.Cell(row, 8).Value = s.SocialHealthNumber;
                ws.Cell(row, 9).Value = s.IsActive ? "Sim" : "Não";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}