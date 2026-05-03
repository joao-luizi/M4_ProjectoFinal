using RepositoryLibrary.Models;

namespace RepositoryLibrary.IRepository
{
    public interface ILessonTypeRepository
    {
        public Task<List<LessonType>> GetAllLessonTypesAsync();
        public Task<LessonType> AddNewLessonTypeAsync(LessonType lessonType);

    }
}
