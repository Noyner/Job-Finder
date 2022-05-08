using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.ResumeSkill;
using CRM.DAL.Models.DatabaseModels.Tags;
using CRM.DAL.Models.DatabaseModels.Users;
using CRM.DAL.Models.DatabaseModels.VacancyResumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.Resume
{
    public class Resume
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public User Creator { get; set; }
        
        public string CreatorId { get; set; }
        
        public string FullDescription { get; set; }
        
        public Remoteness Remoteness { get; set; }
        
        public EmploymentType EmploymentType { get; set; }
        
        public Language.Language Language { get; set; }
        
        public Guid LanguageId { get; set; }
        
        public City.City City { get; set; }
        
        public ICollection<ResumeSkill.ResumeSkill> ResumeSkills { get; set; }
        
        public ICollection<VacancyResume> VacancyResumes { get; set; }

        public Guid? CityId { get; set; }

    }
    
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> item)
        {
            item.HasMany<ResumeSkill.ResumeSkill>(i => i.ResumeSkills)
                .WithOne(r => r.Resume)
                .HasForeignKey(i => i.ResumeId);

            item.HasOne<Language.Language>(r => r.Language);

            item.HasOne<City.City>(r => r.City);

        }
    }
}