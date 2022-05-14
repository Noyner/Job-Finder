using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Resume;
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
        
        public string City { get; set; }

        public DateTime CreatedAt { get; set; }

        public string RequiredSkills { get; set; }
        
        public ICollection<VacancyApplication> VacancyApplications { get; set; }
        
        public Kontragents.Kontragent Kontragent { get; set; }
        
        public Guid KontragentId { get; set; }

    }
    
    public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
    {
        public void Configure(EntityTypeBuilder<Vacancy> item)
        {
            
            item.HasOne<Kontragents.Kontragent>(i => i.Kontragent)
                .WithMany(i => i.Vacancies)
                .HasForeignKey(i => i.KontragentId);

            item.Property(i => i.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        }
    }
}