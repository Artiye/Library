using Library.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class BookClub
    {
        public int BookClubId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public GenreTypes Genre { get; set; }

        public Languages Languages { get; set; }

        public List<Author> Authors { get; set; }

        public List<Book> Books { get; set; }

        public List<BookClubJoinRequest> BookClubJoinRequests { get; set; }

        public List<ApplicationUser>? Members { get; set; }

        public string? OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public string OwnerEmail { get; set; }


    }
    public class BookClubConfiguration : IEntityTypeConfiguration<BookClub>
    {
        public void Configure(EntityTypeBuilder<BookClub> builder)
        {
            builder.HasKey(bc => bc.BookClubId);
            builder.Property(bc => bc.Name).IsRequired();

            builder.HasMany(bc => bc.Books)
                .WithMany(bc => bc.Clubs)
                .UsingEntity<Dictionary<string, object>>(
                     "BookBookClub",
                     j => j.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.Cascade),
                     j => j.HasOne<BookClub>().WithMany().HasForeignKey("BookClubId").OnDelete(DeleteBehavior.Cascade),
                     j =>
                     {
                         j.HasKey("BookClubId", "BookId");
                         j.ToTable("BookBookClub");
                     });



            builder.HasMany(bc => bc.Authors)
                .WithMany(bc => bc.Clubs)
                .UsingEntity<Dictionary<string, object>>(
                     "AuthorBookClub",
                     j => j.HasOne<Author>().WithMany().HasForeignKey("AuthorId").OnDelete(DeleteBehavior.Cascade),
                     j => j.HasOne<BookClub>().WithMany().HasForeignKey("BookClubId").OnDelete(DeleteBehavior.Cascade),
                     j =>
                     {
                         j.HasKey("BookClubId", "AuthorId");
                         j.ToTable("AuthorBookClub");
                     });

            builder.HasMany(bc => bc.Members)
                 .WithMany() 
                 .UsingEntity<Dictionary<string, object>>(
                     j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("ApplicationUserId").OnDelete(DeleteBehavior.NoAction),
                     j => j.HasOne<BookClub>().WithMany().HasForeignKey("BookClubId").OnDelete(DeleteBehavior.Cascade),
                     j =>
                     {
                         j.HasKey("BookClubId", "ApplicationUserId");
                         j.ToTable("BookClubMember");
                     });

            builder.HasOne(b => b.Owner)
               .WithMany(b => b.Clubs)  
               .HasForeignKey(b => b.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}