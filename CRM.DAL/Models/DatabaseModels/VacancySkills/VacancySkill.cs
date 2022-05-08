using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Tags;
using CRM.DAL.Models.DatabaseModels.Vacancies;

namespace CRM.DAL.Models.DatabaseModels.VacancySkills
{
    public class VacancySkill
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Vacancy Vacancy { get; set; }
        
        public Guid VacancyId { get; set; }
        
        public Guid SkillId { get; set; }
        
        public Skill Skill { get; set; }
        
        public SkillType Type { get; set; }
    }
}