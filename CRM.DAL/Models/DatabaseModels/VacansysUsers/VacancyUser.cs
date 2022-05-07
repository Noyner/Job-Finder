using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.ProductsUsers;
using CRM.DAL.Models.DatabaseModels.Vacancys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.VacansysUsers
{
    public class VacancyUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        
        public Guid VacancyId { get; set; }

        public Users.User User { get; set; }
        
        public Vacancy Vacancy { get; set; }
        
        public VacancyUserRelationType RelationType { get; set; }
    }
    public class VacancyUserConfiguration : IEntityTypeConfiguration<VacancyUser>
    {
        public void Configure(EntityTypeBuilder<VacancyUser> item)
        {
            item.HasOne(i => i.Vacancy)
                .WithMany(r => r.VacancyUsers)
                .HasForeignKey(i => i.VacancyId);
            
            item.HasOne(i => i.User)
                .WithMany(r => r.VacancyUsers)
                .HasForeignKey(i => i.UserId);
        }
    }
}