using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace RepositoryLibrary.Repository
{
    public class LessonTypeRepository : ILessonTypeRepository
    {
        private readonly EM_DbContext _emContext;

        public LessonTypeRepository(EM_DbContext emContext)
        {
            _emContext = emContext;
        }

        public async Task<List<LessonType>> GetAllLessonTypesAsync()
        {
            try
            {
                List<LessonType> lessonTypes = await _emContext.LessonTypes.ToListAsync();

                if (lessonTypes.IsNullOrEmpty()) throw new Exception("There are no types of lesson in the database.");

                return lessonTypes;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<LessonType> AddNewLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                await _emContext.LessonTypes.AddAsync(lessonType);
                await _emContext.SaveChangesAsync();

                return lessonType;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<LessonType> EditLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                LessonType? typeToUpdate = await _emContext.LessonTypes.FirstOrDefaultAsync(lt => lt.LessonTypeId == lessonType.LessonTypeId);

                if (typeToUpdate is null)
                {
                    throw new Exception($"There's no type of lesson with the Id = {lessonType.LessonTypeId}");
                }

                typeToUpdate.Name = lessonType.Name;
                typeToUpdate.DurationInMinutes = lessonType.DurationInMinutes;

                await _emContext.SaveChangesAsync();

                return typeToUpdate;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<LessonType> DeleteLessonTypeAsync(int lessonTypeId)
        {
            try
            {
                LessonType? type = await _emContext.LessonTypes.FirstOrDefaultAsync(lt => lt.LessonTypeId == lessonTypeId);

                if (type is null)
                {
                    throw new Exception($"There's no type of lesson with the Id = {lessonTypeId}");
                }

                _emContext.LessonTypes.Remove(type);
                await _emContext.SaveChangesAsync();

                return type;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
