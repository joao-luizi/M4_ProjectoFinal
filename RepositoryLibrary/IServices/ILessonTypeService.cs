using System;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.IServices;

public interface ILessonTypeService
{
    Task<List<LessonType>> GetLessonTypes();
}
