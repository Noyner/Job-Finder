using System;
using System.Collections.Generic;
using CRM.DAL.Models.DatabaseModels.Files;
using CRM.DAL.Models.DatabaseModels.KontragentUsers;
using CRM.DAL.Models.DatabaseModels.VacancyResumes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.Users
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string? FathersName { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public Gender Gender { get; set; }
        
        public File Avatar { get; set; }
        
        public Guid? AvatarId { get; set; }
        
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        
        public ICollection<UserClaim> UserClaims { get; set; }

        public bool IsActive { get; set; }
        
        public ICollection<VacancyApplication> VacancyApplications { get; set; }
        
        public ICollection<KontragentUser> KontragentUsers { get; set; }
        
        public string City { get; set; }
        
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

            item.Property(i => i.RegistrationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            item.HasOne<File>(r => r.Avatar);
        }
    }
}