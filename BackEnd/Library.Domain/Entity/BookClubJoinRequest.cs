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
    public class BookClubJoinRequest
    {
        public int BookClubJoinRequestId { get; set; }

        public int BookClubId { get; set; }

        public BookClub BookClub { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Reason { get; set; }

        public bool IsAccepted { get; set; }
    }
    public class BookClubJoinRequestConfiguration : IEntityTypeConfiguration<BookClubJoinRequest>
    {
        public void Configure(EntityTypeBuilder<BookClubJoinRequest> builder)
        {
            builder.HasKey(j => j.BookClubJoinRequestId);
            builder.Property(j => j.Reason).IsRequired();

            builder.HasOne(j => j.BookClub)
             .WithMany(b => b.BookClubJoinRequests)
             .HasForeignKey(j => j.BookClubId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(j => j.User)
                .WithMany()
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
