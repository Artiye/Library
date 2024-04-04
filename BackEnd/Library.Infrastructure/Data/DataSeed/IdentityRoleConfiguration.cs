using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Application.Encryption;

namespace Library.Infrastructure.Data.DataSeed
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        private readonly IEncryptionService _encryptionService;

        public IdentityRoleConfiguration(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            
          builder.HasData(
                new IdentityRole { Id = "adminRoleId1293931239438254523", Name = "Admin", NormalizedName ="ADMIN" },
                new IdentityRole { Id = "userRoleId23094852091092347944", Name = "User", NormalizedName = "USER" }

                
            );
        }
    }
}
