using Library.Application.Encryption;
using Library.Domain.Entity;
using Library.Infrastructure.Data.DataSeed;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Library.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IEncryptionService _encryptionService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEncryptionService encryptionService)
            : base(options)
        {
            _encryptionService = encryptionService;
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookClub> Clubs { get; set; }

        public DbSet<BookClubJoinRequest> JoinRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfiguration(new IdentityUserConfiguration(_encryptionService));
            modelBuilder.ApplyConfiguration(new IdentityUserRoleConfiguration(_encryptionService));
            modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration(_encryptionService));

            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookClubConfiguration());
            modelBuilder.ApplyConfiguration(new BookClubJoinRequestConfiguration());
        }
    }
}
