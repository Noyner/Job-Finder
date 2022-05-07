using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Tags;
using CRM.DAL.Models.DatabaseModels.VacansysUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.Vacancys
{
    public class Vacancy
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string FullDescription { get; set; }
        
        public string ShortDescription { get; set; }
        
        public decimal Salary { get; set; }
        
        public decimal? MinSalary { get; set; }
        
        public DateTime AddedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public int Priority { get; set; }
        
        public ICollection<VacancyRequirements.VacancyRequirements> Requirements { get; set; }

        public ICollection<Tag> Tags { get; set; }
        
        public ICollection<VacancyUser> VacancyUsers { get; set; }
        
        public ICollection<ProductsKontragents.VacancyKontragent> VacancyKontragents { get; set; }
        

    }
    
    public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
    {
        public void Configure(EntityTypeBuilder<Vacancy> item)
        {

            item.HasMany(i => i.Tags)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId);
            
            item.HasMany(i => i.VacancyUsers)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId);

            item.HasMany(i => i.Requirements)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId);
            
            item.HasMany(i => i.VacancyKontragents)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId);

            item.Property(i => i.Priority)
                .HasDefaultValue(0);

            item.Property(i => i.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            item.Property(i => i.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}