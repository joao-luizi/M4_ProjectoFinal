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
            modelBuilder.Entity<Horse>()
                .HasOne(h => h.HorseFoto)
                .WithOne(p => p.Horse)
                .HasForeignKey<HorseFoto>(p => p.HorseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
           

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users_View>()
                .ToView("Users_View")
                .HasKey(k => k.Id);

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
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductEntitlement> ProductEntitlements => Set<ProductEntitlement>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<UserSubscriptionEntitlement> UserSubscriptionEntitlements => Set<UserSubscriptionEntitlement>();
        public DbSet<UserCreditLedgerEntry> UserCreditLedgerEntries => Set<UserCreditLedgerEntry>();
    }
}
