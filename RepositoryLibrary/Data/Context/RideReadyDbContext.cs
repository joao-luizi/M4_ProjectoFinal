
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Purchases.Entities;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Schools.Entities;
using RepositoryLibrary.Features.Lessons.Entities;

namespace RepositoryLibrary.Data.Context
{
    public class RideReadyDbContext : DbContext
    {
        public RideReadyDbContext(DbContextOptions<RideReadyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>().HasIndex(em => em.Email).IsUnique();
            modelBuilder.Entity<School>()
                .HasOne(lg => lg.SchoolPhoto)
                .WithOne(scl => scl.School)
                .HasForeignKey<SchoolPhoto>(lg => lg.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SchoolUser>()
                .HasKey(x => new { x.UserId, x.SchoolId });

            modelBuilder.Entity<UserHorse>()
                .HasKey(x => new { x.UserId, x.HorseId });
            modelBuilder.Entity<Booking>().HasKey(k => new { k.LessonId, k.UserId });
            modelBuilder.Entity<SchoolUser>().HasKey(k => new { k.SchoolId, k.UserId });
            modelBuilder.Entity<LessonProf>().HasKey(k => new { k.LessonId, k.UserId });
            modelBuilder.Entity<LessonHorse>().HasKey(k => new { k.LessonId, k.HorseId });
            modelBuilder.Entity<LessonHorse>().HasOne(lh => lh.Lesson).WithMany(h => h.LessonHorses).HasForeignKey(lh => lh.LessonId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LessonHorse>().HasOne(lh => lh.Horse).WithMany(lh => lh.LessonHorses).HasForeignKey(lh => lh.HorseId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Horse>()
                .HasOne(h => h.HorseFoto)
                .WithOne(p => p.Horse)
                .HasForeignKey<HorseFoto>(p => p.HorseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
           



            modelBuilder.Entity<Product>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18,2)")
               .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.MonthlyRecurringAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.OneOffAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.Lines)
                .WithOne(l => l.Purchase)
                .HasForeignKey(l => l.PurchaseId);

            modelBuilder.Entity<Purchase>()
                .HasIndex(p => new { p.UserId, p.PurchasedAtUtc });

            modelBuilder.Entity<PurchaseLine>()
                .Property(l => l.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseLine>()
                .Property(l => l.LineTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseLine>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId);

            modelBuilder.Entity<UserSubscription>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<UserSubscription>()
                .HasOne(s => s.PurchaseLine)
                .WithMany()
                .HasForeignKey(s => s.PurchaseLineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSubscription>()
                .HasMany(s => s.Entitlements)
                .WithOne(e => e.UserSubscription)
                .HasForeignKey(e => e.UserSubscriptionId);

            modelBuilder.Entity<UserSubscription>()
                .HasIndex(s => new { s.UserId, s.PeriodStart, s.PeriodEnd, s.Status });

            modelBuilder.Entity<UserSubscriptionEntitlement>()
                .HasOne(e => e.LessonType)
                .WithMany()
                .HasForeignKey(e => e.LessonTypeId);

            modelBuilder.Entity<UserCreditLedgerEntry>()
                .HasOne(e => e.LessonType)
                .WithMany()
                .HasForeignKey(e => e.LessonTypeId);

            modelBuilder.Entity<UserCreditLedgerEntry>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCreditLedgerEntry>()
                .HasOne(e => e.PurchaseLine)
                .WithMany()
                .HasForeignKey(e => e.PurchaseLineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCreditLedgerEntry>()
                .HasIndex(e => new { e.UserId, e.LessonTypeId, e.CreatedAtUtc });

            modelBuilder.Entity<ProductEntitlement>()
                .HasOne(pe => pe.Product)
                .WithMany(p => p.Entitlements)
                .HasForeignKey(pe => pe.ProductId);

            modelBuilder.Entity<ProductEntitlement>()
                .HasOne(pe => pe.LessonType)
                .WithMany()
                .HasForeignKey(pe => pe.LessonTypeId);


            modelBuilder.Entity<HorseFoto>()
            .HasKey(x => x.HorseId);

            modelBuilder.Entity<HorseFoto>()
                .HasOne(x => x.Horse)
                .WithOne(h => h.HorseFoto)
                .HasForeignKey<HorseFoto>(x => x.HorseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFoto>()
            .HasKey(x => x.UserId);

            modelBuilder.Entity<UserFoto>()
            .Property(x => x.UserId)
            .IsRequired();

            modelBuilder.Entity<SchoolPhoto>()
            .HasKey(x => x.SchoolId);


            modelBuilder.Entity<SchoolPhoto>()
                .HasOne(x => x.School)
                .WithOne(s => s.SchoolPhoto)
                .HasForeignKey<SchoolPhoto>(x => x.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<HorseFoto> HorseFotos { get; set; }
        public DbSet<UserFoto> UserFotos { get; set; }
        public DbSet<SchoolPhoto> SchoolPhotos { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Horse> Horses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonHorse> LessonHorses { get; set; }
        public DbSet<LessonProf> LessonProfs { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<SchoolUser> SchoolUsers { get; set; }
        public DbSet<UserHorse> UserHorses { get; set; }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductEntitlement> ProductEntitlements => Set<ProductEntitlement>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<UserSubscriptionEntitlement> UserSubscriptionEntitlements => Set<UserSubscriptionEntitlement>();
        public DbSet<UserCreditLedgerEntry> UserCreditLedgerEntries => Set<UserCreditLedgerEntry>();

    }
}
