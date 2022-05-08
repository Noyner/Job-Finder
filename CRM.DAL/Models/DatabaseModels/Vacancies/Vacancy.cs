using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Resume;
using CRM.DAL.Models.DatabaseModels.Tags;
using CRM.DAL.Models.DatabaseModels.VacancyResumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.Vacancies
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
        
        public Remoteness Remoteness { get; set; }
        
        public EmploymentType EmploymentType { get; set; }
        
        public DateTime AddedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public int Priority { get; set; }
        
        public City.City City { get; set; }
        
        public Guid CityId { get; set; }
        
        public Language.Language Language { get; set; }
        
        public Guid LanguageId { get; set; }
        
        public ICollection<VacancySkills.VacancySkill> VacancySkills { get; set; }

        public ICollection<VacancyResume> VacancyUsers { get; set; }
        
        public Kontragents.Kontragent Kontragent { get; set; }
        
        public Guid KontragentId { get; set; }

    }
    
    public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
    {
        public void Configure(EntityTypeBuilder<Vacancy> item)
        {

            item.HasMany(i => i.VacancyUsers)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId);

            item.HasMany(i => i.VacancySkills)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId);

            item.HasOne<Kontragents.Kontragent>(i => i.Kontragent)
                .WithMany(i => i.Vacancies)
                .HasForeignKey(i => i.KontragentId);

            item.Property(i => i.Priority)
                .HasDefaultValue(0);

            item.HasOne<Language.Language>(r => r.Language);
            
            item.HasOne<City.City>(r => r.City);

            item.Property(i => i.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            item.Property(i => i.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}