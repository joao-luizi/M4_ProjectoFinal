using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;

namespace RepositoryLibrary.Features.Users.Service
{
    // A "cozinha": esta classe faz mesmo o trabalho que o menu (a interface) promete.
    public class StudentExportService : IStudentExportService
    {
        // Em vez de ir a base de dados, pede os alunos ao servico que ja sabe faze-lo.
        private readonly IUserService _userService;
        private readonly ILogger<StudentExportService> _logger;

        public StudentExportService(IUserService userService, ILogger<StudentExportService> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<byte[]> ExportStudentsToExcelAsync(int schoolId)
        {
            _logger.LogInformation("A exportar alunos da escola {SchoolId} para Excel.", schoolId);

            // 1. Buscar os alunos desta escola (papel "Student").
            var students = await _userService.GetUsersBySchoolAndRole(schoolId, StaticRole.Student);

            // 2. Criar o ficheiro Excel em memoria.
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Alunos");

            // 3. Linha de cabecalho (linha 1).
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

            // 4. Uma linha por aluno (a partir da linha 2).
            var row = 2;
            foreach (var s in students)
            {
                ws.Cell(row, 1).Value = s.FirstName + " " + s.LastName ?? string.Empty;
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

            // 5. Ajustar a largura das colunas ao conteudo.
            ws.Columns().AdjustToContents();

            // 6. Gravar para memoria e devolver os bytes do ficheiro.
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();

            _logger.LogInformation("Exportados {Count} alunos da escola {SchoolId} ({Size} bytes).", students.Count, schoolId, bytes.Length);
            return bytes;
        }
    }
}