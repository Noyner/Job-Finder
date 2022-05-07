using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Kontragents;
using CRM.DAL.Models.DatabaseModels.Vacancys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.ProductsKontragents
{
    public class VacancyKontragent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Guid VacancyId { get; set; }
        
        public Vacancy Vacancy { get; set; }
        
        public Guid KontragentId { get; set; }
        
        public Kontragent Kontragent { get; set; }
        
        public RelationType RelationType { get; set; }
    }
    public class VacancyKontragentConfiguration : IEntityTypeConfiguration<VacancyKontragent>
    {
        public void Configure(EntityTypeBuilder<VacancyKontragent> item)
        {
            item.HasOne(i => i.Vacancy)
                .WithMany(r => r.VacancyKontragents)
                .HasForeignKey(i => i.VacancyId);
            
            item.HasOne(i => i.Kontragent)
                .WithMany(r => r.VacancyKontragents)
                .HasForeignKey(i => i.KontragentId);
        }
    }
}