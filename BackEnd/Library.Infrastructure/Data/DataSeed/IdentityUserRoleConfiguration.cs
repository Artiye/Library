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
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        private readonly IEncryptionService _encryptionService;

        public IdentityUserRoleConfiguration(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
           
            builder.HasData(
                new IdentityUserRole<string>
                {
                    UserId = "adminuser123412903847192311234",
                    RoleId = "adminRoleId1293931239438254523",


                }
            );

        }
    }
}
