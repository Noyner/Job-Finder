using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.VacancyResumes
{
    public class VacancyResume
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Guid ResumeId { get; set; }
        
        public Guid VacancyId { get; set; }

        public Resume.Resume Resume { get; set; }
        
        public Vacancy Vacancy { get; set; }
        
    }
    public class VacancyUserConfiguration : IEntityTypeConfiguration<VacancyResume>
    {
        public void Configure(EntityTypeBuilder<VacancyResume> item)
        {
            item.HasOne(i => i.Vacancy)
                .WithMany(r => r.VacancyUsers)
                .HasForeignKey(i => i.VacancyId);
            
            item.HasOne(i => i.Resume)
                .WithMany(r => r.VacancyResumes)
                .HasForeignKey(i => i.ResumeId);
        }
    }
}