using System;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Repository;

namespace RepositoryLibrary.Services;

public class LessonTypeService : ILessonTypeService
{
    private readonly ILessonTypeRepository _lessonTypeRepository;
    public LessonTypeService(EM_DbContext context)
    {
        _lessonTypeRepository = new LessonTypeRepository(context);
    }
    public async Task<List<LessonType>> GetLessonTypes()
    {
        return await _lessonTypeRepository.GetAllLessonTypesAsync();
    }
}
