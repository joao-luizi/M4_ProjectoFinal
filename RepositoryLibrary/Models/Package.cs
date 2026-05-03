using System;

namespace RepositoryLibrary.Models;

public class Package
{
    public int Id { get; set; }
    public int LessonTypeId { get; set; }
    public LessonType LessonType { get; set; }
    public string Name { get; set; }
    public int ClassesIncluded { get; set; }
    public bool Weekly { get; set; }
    public int Valor { get; set; }
}
