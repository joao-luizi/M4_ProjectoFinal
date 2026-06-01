using System;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Products.Interfaces;
using RepositoryLibrary.Repository;

namespace RepositoryLibrary.Features.Products;

public class LessonTypeService : ILessonTypeService
{
    private readonly ILessonTypeRepository _lessonTypeRepository;
    public LessonTypeService(RideReadyDbContext context)
    {
        _lessonTypeRepository = new LessonTypeRepository(context);
    }
    public async Task<List<LessonType>> GetLessonTypes()
    {
        return await _lessonTypeRepository.GetAllLessonTypesAsync();
    }
}
