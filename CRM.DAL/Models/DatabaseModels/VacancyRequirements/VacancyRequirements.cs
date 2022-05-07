using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Vacancys;

namespace CRM.DAL.Models.DatabaseModels.VacancyRequirements
{
    public class VacancyRequirements
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Vacancy Vacancy { get; set; }
        
        public Guid VacancyId { get; set; }
        
        public string RequirementKey { get; set; }
        
        public string RequirementValue { get; set; }
    }
}