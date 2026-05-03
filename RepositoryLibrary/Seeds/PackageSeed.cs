using System;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;

namespace RepositoryLibrary.Seeds;

public class PackageSeed
{
    public static async Task SeedPackages(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            List<LessonType> lessonTypes = [.. em_context.LessonTypes];
            var Volteio = lessonTypes.FirstOrDefault(lt => lt.Name == "Volteio");
            var Sela1 = lessonTypes.FirstOrDefault(lt => lt.Name == "Sela 1");
            var Sela2 = lessonTypes.FirstOrDefault(lt => lt.Name == "Sela 2");
            var Individual = lessonTypes.FirstOrDefault(lt => lt.Name == "Individual");
            var Passeio = lessonTypes.FirstOrDefault(lt => lt.Name == "Passeio");
            if (!em_context.Packages.Any())
            {
                List<Package> packages =
                [
                    new Package{ ClassesIncluded = 1, LessonType = Volteio , Name = "Volteio1xSemana", Valor = 75, Weekly = true},
                    new Package{ ClassesIncluded = 2, LessonType = Volteio , Name = "Volteio2xSemana", Valor = 135, Weekly = true},
                    new Package{ ClassesIncluded = 3, LessonType = Volteio , Name = "Volteio3xSemana", Valor = 200, Weekly = true},
                    new Package{ ClassesIncluded = 8, LessonType = Volteio , Name = "Volteio", Valor = 157, Weekly = false},
                    new Package{ ClassesIncluded = 1, LessonType = Sela1 , Name = "Sela11xSemana", Valor = 100, Weekly = true},
                    new Package{ ClassesIncluded = 2, LessonType = Sela1 , Name = "Sela12xSemana", Valor = 175, Weekly = true},
                    new Package{ ClassesIncluded = 3, LessonType = Sela1 , Name = "Sela13xSemana", Valor = 245, Weekly = true},
                    new Package{ ClassesIncluded = 8, LessonType = Sela1 , Name = "Sela1", Valor = 197, Weekly = false},
                    new Package{ ClassesIncluded = 1, LessonType = Sela2 , Name = "Sela21xSemana", Valor = 125, Weekly = true},
                    new Package{ ClassesIncluded = 2, LessonType = Sela2 , Name = "Sela22xSemana", Valor = 209, Weekly = true},
                    new Package{ ClassesIncluded = 3, LessonType = Sela2 , Name = "Sela23xSemana", Valor = 290, Weekly = true},
                    new Package{ ClassesIncluded = 8, LessonType = Sela2 , Name = "Sela2", Valor = 226, Weekly = false},
                    new Package{ ClassesIncluded = 1, LessonType = Individual , Name = "Individual1xSemana", Valor = 178, Weekly = true},
                    new Package{ ClassesIncluded = 2, LessonType = Individual , Name = "Individual2xSemana", Valor = 295, Weekly = true},
                    new Package{ ClassesIncluded = 3, LessonType = Individual , Name = "Individual3xSemana", Valor = 412, Weekly = true},
                    new Package{ ClassesIncluded = 8, LessonType = Individual , Name = "Individual", Valor = 310, Weekly = false}
                ];
                await em_context.Packages.AddRangeAsync(packages);
                await em_context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding payment", e);
        }
    }
}
