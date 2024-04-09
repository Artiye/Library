using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Gender { get; set; }

        public string? Nationality { get; set; }


        public List<BookClub>? Clubs { get; set; }

        public List<Book>? Books { get; set; }

        public List<Author>? Authors { get; set; }


    }
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(bc => bc.FirstName).IsRequired();

            builder.HasMany(a => a.Books)
                .WithMany(a => a.Users)
                .UsingEntity<Dictionary<string, object>>(
                     "UserBooks",
                     j => j.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.Cascade),
                     j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("ApplicationUserId").OnDelete(DeleteBehavior.Cascade),
                     j =>
                     {
                         j.HasKey("ApplicationUserId", "BookId");
                         j.ToTable("UserBooks");
                     });


            builder.HasMany(a => a.Authors)
                .WithMany(a => a.Users)
                .UsingEntity<Dictionary<string, object>>(
                     "UserAuthors",
                     j => j.HasOne<Author>().WithMany().HasForeignKey("AuthorId").OnDelete(DeleteBehavior.Cascade),
                     j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("ApplicationUserId").OnDelete(DeleteBehavior.Cascade),
                     j =>
                     {
                         j.HasKey("ApplicationUserId", "AuthorId");
                         j.ToTable("UserAuthors");
                     });
        }
    }
}
