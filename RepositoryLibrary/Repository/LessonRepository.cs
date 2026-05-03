using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Repository
{
    public class LessonRepository : ILessonRepository
    {
        private readonly EM_DbContext _emContext;

        public LessonRepository(EM_DbContext context)
        {
            _emContext = context;
        }

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            return await _emContext.Lessons
            .Include(l => l.LessonType)
            .Include(l => l.Bookings)
            .Include(l => l.LessonProfs)
            .Include(l => l.LessonHorses).ThenInclude(lh => lh.Horse)
            .Include(l => l.School)
            .ToListAsync();
        }

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            return await _emContext.Lessons.Include(l => l.LessonType).Include(l => l.Bookings).Include(l => l.LessonProfs).FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }

        public async Task CreateLessonAsync(Lesson lesson)
        {
            _emContext.Lessons.Add(lesson);
            await _emContext.SaveChangesAsync();
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _emContext.Lessons.Update(lesson);
            await _emContext.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            var lesson = await _emContext.Lessons.FindAsync(lessonId);
            if (lesson != null)
            {
                _emContext.Lessons.Remove(lesson);
                await _emContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByTeacherAsync(string teacherId)
        {
            return await _emContext.Lessons
                .Where(l => l.LessonProfs.Any(lp => lp.UserId == teacherId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByStudentAsync(string studentId)
        {
            return await _emContext.Lessons
                .Where(l => l.Bookings.Any(b => b.UserId == studentId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Lesson>> GetAvailableLessonsAsync()
        {
            return await _emContext.Lessons
                .Where(l => l.Bookings.Count() < l.MaxSpots)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByHorseIdAsync(int horseId)
        {
            return await _emContext.Lessons
                .Where(l => l.LessonHorses.Any(lh => lh.HorseId == horseId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByDateAsync(DateTime date)
        {
            return await _emContext.Lessons
                .Where(l => l.BeginOfLesson.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date)
        {
            try
            {
                //other possibility
                //var nextDay = date.Date.AddDays(1);
                // return await _context.Lessons
                //     .Include(l => l.LessonType)
                //     .Where(l => l.BeginOfLesson >= date.Date &&
                //                 l.BeginOfLesson < nextDay &&
                //                 l.LessonHorses.Any(lh => lh.HorseId == horseId))
                //     .ToListAsync();
                return await _emContext.Lessons.Include(l => l.LessonType).Where(l => DateTime.Compare(date.Date, l.BeginOfLesson.Date) == 0 && l.LessonHorses.Any(lh => lh.HorseId == horseId)).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }


    }
}
