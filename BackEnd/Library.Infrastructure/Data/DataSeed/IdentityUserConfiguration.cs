using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Entity;

namespace Library.Infrastructure.Data.DataSeed
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            builder.HasData(new ApplicationUser
            {
                Id = "adminuser123412903847192311234",
                FirstName = "Art",
                LastName = "Morina",
                Gender = "Male",
                Nationality = "Kosovar",
                UserName = "artinjobro@gmail.com",
                NormalizedUserName = "ARTINJOBRO@GMAIL.COM",
                Email = "artinjobro@gmail.com",
                NormalizedEmail = "ARTINJOBRO@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "Artmorina.1"),
                SecurityStamp = Guid.NewGuid().ToString(),
            });
        }
    }
}
