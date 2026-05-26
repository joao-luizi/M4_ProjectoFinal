using RepositoryLibrary.Features.Products.Entities;

namespace RepositoryLibrary.Features.Products.Interfaces
{
    public interface ILessonTypeRepository
    {
        public Task<List<LessonType>> GetAllLessonTypesAsync();
        public Task<LessonType> AddNewLessonTypeAsync(LessonType lessonType);

    }
}
