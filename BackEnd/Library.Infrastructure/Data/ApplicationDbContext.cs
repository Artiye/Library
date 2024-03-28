using Library.Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace Library.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) { }
        
            public DbSet<Author> Authors { get; set; }

            public DbSet<Book> Books {  get; set; }

            public DbSet<BookClub> Clubs { get; set; }

            public DbSet<BookClubJoinRequest> JoinRequests { get; set; }

            

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookClubConfiguration());
            modelBuilder.ApplyConfiguration(new BookClubJoinRequestConfiguration());
        }


    }
}
