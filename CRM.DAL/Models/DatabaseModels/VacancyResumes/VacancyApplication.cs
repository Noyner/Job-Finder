using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.VacancyResumes
{
    public class VacancyApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Guid ResumeId { get; set; }
        
        public Guid VacancyId { get; set; }

        public Resume.Resume Resume { get; set; }
        
        public Vacancy Vacancy { get; set; }
        
    }
    public class VacancyApplicationConfiguration : IEntityTypeConfiguration<VacancyApplication>
    {
        public void Configure(EntityTypeBuilder<VacancyApplication> item)
        {
            item.HasOne(i => i.Vacancy)
                .WithMany(r => r.VacancyApplications)
                .HasForeignKey(i => i.VacancyId);
            
            item.HasOne(i => i.Resume)
                .WithMany(r => r.VacancyApplications)
                .HasForeignKey(i => i.ResumeId);
        }
    }
}