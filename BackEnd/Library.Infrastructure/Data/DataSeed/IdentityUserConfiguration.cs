using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Data.DataSeed
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            builder.HasData(new IdentityUser
            {
                Id = "adminuser123412903847192311234",
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
