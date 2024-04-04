using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Entity;
using Library.Application.Encryption;

namespace Library.Infrastructure.Data.DataSeed
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        private readonly IEncryptionService _encryptionService;

        public IdentityUserConfiguration(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            builder.HasData(new ApplicationUser
            {
                Id ="adminuser123412903847192311234",
                FirstName = _encryptionService.EncryptData("Art"),
                LastName = _encryptionService.EncryptData("Morina"),
                Gender = _encryptionService.EncryptData("Male"),
                Nationality = _encryptionService.EncryptData("Kosovar"),
                UserName = "artinjobro@gmail.com",
                NormalizedUserName = "ARTINJOBRO@GMAIL.COM",
                Email = _encryptionService.EncryptData("artinjobro@gmail.com"),
                NormalizedEmail = _encryptionService.EncryptData("ARTINJOBRO@GMAIL.COM"),
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Artmorina.1"),
                SecurityStamp = Guid.NewGuid().ToString(),
            });
        }
    }
}
