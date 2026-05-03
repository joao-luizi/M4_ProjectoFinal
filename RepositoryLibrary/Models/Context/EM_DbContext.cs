using RepositoryLibrary.Models.Views;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Models.Context
{
    public class EM_DbContext : DbContext
    {
        public EM_DbContext(DbContextOptions<EM_DbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>().HasIndex(em => em.Email).IsUnique();
            modelBuilder.Entity<Logo>().HasKey(k => new { k.SchoolId, k.LogoName });
            modelBuilder.Entity<School>()
                .HasOne(lg => lg.Logo)
                .WithOne(scl => scl.School)
                .HasForeignKey<Logo>(lg => lg.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Booking>().HasKey(k => new { k.LessonId, k.UserId });
            modelBuilder.Entity<SchoolUser>().HasKey(k => new { k.SchoolId, k.UserId });
            modelBuilder.Entity<LessonProf>().HasKey(k => new { k.LessonId, k.UserId });
            modelBuilder.Entity<LessonHorse>().HasKey(k => new { k.LessonId, k.HorseId });
            modelBuilder.Entity<LessonHorse>().HasOne(lh => lh.Lesson).WithMany(h => h.LessonHorses).HasForeignKey(lh => lh.LessonId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LessonHorse>().HasOne(lh => lh.Horse).WithMany(lh => lh.LessonHorses).HasForeignKey(lh => lh.HorseId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserHorse>().HasKey(k => new { k.UserId, k.HorseId });
            modelBuilder.Entity<Photo>().HasKey(k => k.UserId);
            modelBuilder.Entity<UserPayment>().HasKey(k => new { k.UserId, k.BuyDate });

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users_View>()
                .ToView("Users_View")
                .HasKey(k => k.Id);
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Horse> Horses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonHorse> LessonHorses { get; set; }
        public DbSet<LessonProf> LessonProfs { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<Logo> Logos { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<SchoolUser> SchoolUsers { get; set; }
        public DbSet<UserHorse> UserHorses { get; set; }
        public DbSet<Users_View> Users_Views { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<UserPayment> UserPayments { get; set; }
    }
}
