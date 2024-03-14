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
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) { }
        
            public DbSet<Author> Authors { get; set; }

            public DbSet<Book> Books {  get; set; }

            public DbSet<BookClub> Clubs { get; set; }

            

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Book>()
               .HasMany(l => l.Authors)
               .WithMany(l => l.Books);

            modelBuilder.Entity<BookClub>()
                .HasMany(a => a.Authors)
                .WithMany(a => a.Clubs);

            modelBuilder.Entity<BookClub>()
                .HasMany(b => b.Books)
                .WithMany(b => b.Clubs);

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }


    }
}
