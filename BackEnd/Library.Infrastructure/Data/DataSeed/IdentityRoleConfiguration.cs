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
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole { Id = "adminRoleId1293931239438254523", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "userRoleId23094852091092347944", Name = "User", NormalizedName ="USER" }
            );
        }
    }
}
