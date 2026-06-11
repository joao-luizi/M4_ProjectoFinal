using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Bookings.Enums;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Email.DTOs;
using RepositoryLibrary.Features.Email.Services;
using RepositoryLibrary.Features.Entitlements.DTOs;
using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Entitlements.Interfaces;
using RepositoryLibrary.Features.Lessons.DTOs;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;


namespace RepositoryLibrary.Features.Lessons.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepo;
        private readonly IBookingRepository _bookRepo;
        private readonly IUserService _userService;
        private readonly IEntitlementRepository _entitlementRepo;
        private readonly AppEmailSender _EmailSender;
        private readonly ILogger<LessonService> _logger;

        public LessonService(RideReadyDbContext context, 
            ILessonRepository lessonRepo, 
            IUserService userService, 
            IBookingRepository bookRepo,
            AppEmailSender EmailSender,
            IEntitlementRepository entitlementRepo,
            ILogger<LessonService> logger)

        {
            _lessonRepo = lessonRepo;
            _bookRepo = bookRepo;
            _userService = userService;
            _entitlementRepo = entitlementRepo;
            _EmailSender = EmailSender;
            _logger = logger;
        }


        public async Task<LessonAttendanceDto> GetAttendanceAsync(int lessonId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson is null)
                throw new Exception($"Lesson {lessonId} not found.");

            var students = await _userService.GetSelectUsersBySchoolAndRole(
                lesson.SchoolId,
                StaticRole.Student);

            var dto = new LessonAttendanceDto
            {
                LessonId = lesson.LessonId,

                Students = lesson.Bookings
                    .Select(booking =>
                    {
                        var student = students
                            .FirstOrDefault(s => s.Id == booking.UserId);

                        return new LessonAttendanceUserDto
                        {
                            UserId = booking.UserId,
                            Name = student?.Name ?? "Unknown student",
                            IsBooked = true,
                            WasPresent = booking.WasPresent
                        };
                    })
                    .ToList()
            };

            return dto;
        }
        //V2 implemented
        public async Task<LessonEditDto> GetForEditAsync(int lessonId)
        {
            var fullLesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (fullLesson is null)
                throw new Exception($"Lesson {lessonId} not found");

            var dto = new LessonEditDto
            {
                LessonId = fullLesson.LessonId,
                SchoolId = fullLesson.SchoolId,
                LessonTypeId = fullLesson.LessonTypeId,

                BeginOfLesson = fullLesson.BeginOfLesson,
                EndOfLesson = fullLesson.EndOfLesson,

                MaxSpots = fullLesson.MaxSpots,

                TeacherIds = fullLesson.LessonProfs?
                    .Select(lp => lp.UserId.ToString())
                    .ToList(),

                HorseIds = fullLesson.LessonHorses?
                    .Select(lh => lh.HorseId)
                    .ToList()
            };

            return dto;
        }

        public async Task UpdateAttendanceAsync(LessonAttendanceDto dto)
        {
            var lesson =
                await _lessonRepo.GetByIdWithDetailsAsync(dto.LessonId);

            if (lesson is null)
                throw new Exception("Lesson not found.");

            foreach (var student in dto.Students)
            {
                var booking = lesson.Bookings
                    .FirstOrDefault(b => b.UserId == student.UserId);

                if (booking != null)
                {
                    booking.WasPresent = student.WasPresent;
                }
            }

            await _lessonRepo.SaveChangesAsync();
        }

        public async Task CancelBookingAsync(int lessonId, string userId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson is null)
                throw new Exception("Lesson not found.");

            var booking = lesson.Bookings
                .FirstOrDefault(b => b.UserId == userId);

            if (booking is null)
                return;

            //Se o booking consumiu crédito -> devolver crédito
            if (booking.FundingType == BookingFundingType.Credit)
            {
                await _entitlementRepo.AddAsync(new UserCreditLedgerEntry
                {
                    UserId = userId,
                    LessonTypeId = lesson.LessonTypeId,
                    CreditsDelta = +1,
                    Reason = "Booking cancelled due to lesson deletion"
                });
            }

            await _bookRepo.Delete(booking);
        }

        public async Task<BookingResult> BookLessonAsync(int lessonId, string userId)
        {
            var existing = await _bookRepo.GetByLessonandUserIdsAsync(lessonId, userId);

            if (existing != null)
            {
                return new BookingResult
                {
                    Success = false,
                    Errors = { BookingValidationError.AlreadyBooked }
                };
            }

            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
            {
                return new BookingResult
                {
                    Success = false,
                    Errors = { BookingValidationError.LessonNotFound }
                };
            }

            var errors = await _entitlementRepo.GetSubscriptionErrorsAsync(
                userId,
                lesson.LessonTypeId);

            var creditBalance = await _entitlementRepo.GetCreditBalanceAsync(
                userId,
                lesson.LessonTypeId);

            if (creditBalance <= 0)
            {
                errors.Add(BookingValidationError.NoCreditsAvailable);
            }

            var hasSubscriptionErrors = errors.Any(x =>
                x == BookingValidationError.NoActiveSubscription ||
                x == BookingValidationError.SubscriptionExpired);

            var hasCredits = creditBalance > 0;

            // Weekly usage
            int weeklyBookings = await _bookRepo.GetWeeklyBooking(userId, lesson.LessonTypeId);
            int weeklyLimit = await _entitlementRepo.GetWeeklyLimit(userId, lesson.LessonTypeId);

            bool weeklyLimitExceeded = weeklyBookings >= weeklyLimit;

            var willUseCredit = false;

            // Sem subscrição válida e sem créditos
            if (hasSubscriptionErrors && !hasCredits)
            {
                return new BookingResult
                {
                    Success = false,
                    Errors = errors
                };
            }

            // Excedeu limite semanal
            if (weeklyLimitExceeded)
            {
                if (hasCredits)
                {
                    willUseCredit = true;
                }
                else
                {
                    errors.Add(BookingValidationError.WeeklyLimitExceeded);

                    return new BookingResult
                    {
                        Success = false,
                        Errors = errors
                    };
                }
            }

            await _bookRepo.addAsync(new Booking
            {
                LessonId = lessonId,
                UserId = userId,
                WasPresent = false,
                FundingType = willUseCredit
                    ? BookingFundingType.Credit
                    : BookingFundingType.Subscription
            });

            await _bookRepo.SaveChanges();

            // Ledger se usar crédito
            if (willUseCredit)
            {
                await _entitlementRepo.AddAsync(new UserCreditLedgerEntry
                {
                    UserId = userId,
                    LessonTypeId = lesson.LessonTypeId,
                    CreditsDelta = -1,
                    Reason = "Booking fallback from subscription weekly limit"
                });
            }

            return new BookingResult
            {
                Success = true,
                UsedCredit = willUseCredit,
                Warnings = willUseCredit
                    ? new List<BookingValidationError>
                    {
                BookingValidationError.SubscriptionLimitExceededUsingCredit
                    }
                    : new List<BookingValidationError>()
            };
        }

        private async Task ValidateHorseAvailabilityAsync(
    LessonEditDto dto)
        {
            // TODO:
            // Validate horse workload rules.
            //
            // Rules:
            // - Max 2 lessons per horse/day
            // - If only rides, max 4 rides/day
            // - After 1 ride -> only 1 lesson allowed
            // - After 2 rides -> no lessons allowed
            // - Horses must have 2 rest days/week
        }
        //V2 implemented
        public async Task<int> CreateAsync(LessonEditDto dto)
        {

            //await ValidateHorseAvailabilityAsync(dto);

            var lesson = new Lesson
            {
                SchoolId = dto.SchoolId,
                LessonTypeId = dto.LessonTypeId,
                BeginOfLesson = dto.BeginOfLesson,
                EndOfLesson = dto.EndOfLesson,
                MaxSpots = dto.MaxSpots,

                Bookings = new List<Booking>(),
                LessonProfs = new List<LessonProf>(),
                LessonHorses = new List<LessonHorse>()
            };

            if (dto.TeacherIds != null)
            {
                foreach (var teacherId in dto.TeacherIds)
                {
                    lesson.LessonProfs.Add(new LessonProf
                    {
                        UserId = teacherId,
                        Lesson = lesson
                    });
                }
            }

            if (dto.HorseIds != null)
            {
                foreach (var horseId in dto.HorseIds)
                {
                    lesson.LessonHorses.Add(new LessonHorse
                    {
                        HorseId = horseId,
                        Lesson = lesson
                    });
                }
            }

            await _lessonRepo.AddAsync(lesson);

            return lesson.LessonId;
        }
        //V2 implemented

        public async Task CancelBookingAsync(Booking booking, Lesson lesson)
        {
            if (booking.FundingType == BookingFundingType.Credit)
            {
                await _entitlementRepo.AddAsync(new UserCreditLedgerEntry
                {
                    UserId = booking.UserId,
                    LessonTypeId = lesson.LessonTypeId,
                    CreditsDelta = +1,
                    Reason = "Lesson cancelled"
                });
            }

            _bookRepo.DeleteNoSave(booking);
        }

        public async Task UpdateAsync(LessonEditDto dto)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(dto.LessonId);

            if (lesson == null)
                throw new Exception($"Lesson {dto.LessonId} not found");

            // ─────────────────────────────
            // 1. Update scalar fields
            // ─────────────────────────────
            lesson.BeginOfLesson = dto.BeginOfLesson;
            lesson.EndOfLesson = dto.EndOfLesson;

            var currentBookings = lesson.Bookings?.Count ?? 0;

            if (dto.MaxSpots < currentBookings)
                throw new Exception("Cannot reduce capacity below current bookings");

            lesson.MaxSpots = dto.MaxSpots;

            // ─────────────────────────────
            // 2. Sync Teachers (LessonProfs)
            // ─────────────────────────────
            lesson.LessonProfs?.Clear();

            if (dto.TeacherIds != null)
            {
                lesson.LessonProfs = dto.TeacherIds
                    .Select(id => new LessonProf
                    {
                        UserId = id,
                        LessonId = lesson.LessonId
                    })
                    .ToList();
            }

            // ─────────────────────────────
            // 3. Sync Horses (LessonHorses)
            // ─────────────────────────────
            lesson.LessonHorses?.Clear();

            if (dto.HorseIds != null)
            {
                lesson.LessonHorses = dto.HorseIds
                    .Select(id => new LessonHorse
                    {
                        HorseId = id,
                        LessonId = lesson.LessonId
                    })
                    .ToList();
            }

            // ─────────────────────────────
            // 4. Persist
            // ─────────────────────────────
            await _lessonRepo.UpdateAsync(lesson);
        }
        //V2 implemented
        public async Task<int> DeleteAsync(int lessonId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception($"Lesson {lessonId} not found");

            var userIds = new List<string>();

            // Teachers
            if (lesson.LessonProfs != null)
            {
                userIds.AddRange(
                    lesson.LessonProfs
                        .Select(x => x.UserId)
                );
            }

            // Students (Bookings)
            if (lesson.Bookings != null)
            {
                userIds.AddRange(
                    lesson.Bookings
                        .Select(x => x.UserId)
                );
                foreach (var booking in lesson.Bookings)
                {
                    await CancelBookingAsync(booking, lesson);
                }

            }

            userIds = userIds.Distinct().ToList();

            List<string> userEmails = await _userService.GetUserEmailsAsync(userIds);

            var emailDto = new EmailSenderDto
            {
                UserEmails = userEmails,
                Subject = "Aula cancelada",
                Body = $"A aula de {lesson.BeginOfLesson:dd/MM HH:mm} foi cancelada."
            };

            await _EmailSender.SendLessonCancelledAsync(emailDto);

            await _lessonRepo.DeleteAsync(lesson);

            return userEmails.Count();
        }
        //V2 implemented
        public async Task AssignHorseAsync(int lessonId, int horseId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            var exists = lesson.LessonHorses.Any(h => h.HorseId == horseId);

            if (exists)
                return;

            lesson.LessonHorses.Add(new LessonHorse
            {
                LessonId = lessonId,
                HorseId = horseId
            });

            await _lessonRepo.UpdateAsync(lesson);
        }
        //V2 implemented
        public async Task AssignTeacherAsync(int lessonId, string userId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            var exists = lesson.LessonProfs.Any(p => p.UserId == userId);

            if (exists)
                return;

            lesson.LessonProfs.Add(new LessonProf
            {
                LessonId = lessonId,
                UserId = userId
            });

            await _lessonRepo.UpdateAsync(lesson);
        }
        //V2 implemented
        public async Task ChangeCapacityAsync(int lessonId, int maxSpots)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            var bookings = lesson.Bookings?.Count ?? 0;

            if (maxSpots < bookings)
                throw new Exception("Cannot reduce capacity below current bookings");

            lesson.MaxSpots = maxSpots;

            await _lessonRepo.UpdateAsync(lesson);
        }
    }
}