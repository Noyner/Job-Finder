using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        public string City { get; set; }
        
        public string OwnSkills { get; set; }
        
        public ICollection<VacancyApplication> VacancyApplications { get; set; }

    }
    
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> item)
        {

        }
    }
}