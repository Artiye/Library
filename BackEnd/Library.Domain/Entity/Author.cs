using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class Author
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; }

        public string Nationality { get; set; }

        public string Biography { get; set; }

        public string ProfileImage { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public List<Book>? Books { get; set; }

        public List<BookClub>? Clubs { get; set; }

        public List<ApplicationUser>? Users { get; set; }

    }
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.AuthorId);
            builder.Property(a => a.FullName).IsRequired();


            builder.HasMany(a => a.Books)
                .WithMany(b => b.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorBooks",
                    j => j.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Author>().WithMany().HasForeignKey("AuthorId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("AuthorId", "BookId");
                        j.ToTable("AuthorBooks");
                    });

           
        }
    }
}





