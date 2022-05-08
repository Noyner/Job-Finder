using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Tags;

namespace CRM.DAL.Models.DatabaseModels.ResumeSkill
{
    public class ResumeSkill
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Resume.Resume Resume { get; set; }
        
        public Guid ResumeId { get; set; }
        
        public Guid SkillId { get; set; }
        
        public Skill.Skill Skill { get; set; }
        
        public SkillType Type { get; set; }
    }
}