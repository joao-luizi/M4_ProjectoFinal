using System;
using RepositoryLibrary.Features.Products.Entities;

namespace RepositoryLibrary.Features.Products.Interfaces;

public interface ILessonTypeService
{
    Task<List<LessonType>> GetLessonTypes();
}
