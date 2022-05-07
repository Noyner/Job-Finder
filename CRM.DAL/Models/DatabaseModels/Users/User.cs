using System;
using System.Collections.Generic;
using CRM.DAL.Models.DatabaseModels.KontragentUsers;
using CRM.DAL.Models.DatabaseModels.ProductsUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.Users
{
    public class User : IdentityUser
    {
        public ICollection<UserRole> UserRoles { get; set; }
        
        public ICollection<UserClaim> UserClaims { get; set; }

        public bool IsActive { get; set; }
        
        public ICollection<VacancyUser> VacancyUsers { get; set; }
        
        public ICollection<KontragentUser> KontragentUsers { get; set; }


        public DateTime RegistrationDate { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> item)
        {
            item.HasBaseType((Type) null);
            
            item.HasMany(i => i.UserRoles)
                .WithOne(i => i.User)
                .HasForeignKey(i=>i.UserId);
            
            item.HasMany(i => i.UserClaims)
                .WithOne(i => i.User)
                .HasForeignKey(i=>i.UserId);

            item.HasMany(i => i.VacancyUsers)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId);

            item.Property(i => i.RegistrationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}