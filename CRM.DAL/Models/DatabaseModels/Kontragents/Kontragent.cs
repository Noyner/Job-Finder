using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.DAL.Models.DatabaseModels.Files;
using CRM.DAL.Models.DatabaseModels.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.DAL.Models.DatabaseModels.Kontragents
{
    public class Kontragent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public File Icon { get; set; }
        
        public Guid? IconId { get; set; }
        
        public string Info { get; set; }
        
        public string UserId { get; set; }
        
        public User User { get; set; }

        public ICollection<Vacancies.Vacancy> Vacancies { get; set; }
    }
    
    public class KontragentConfiguration : IEntityTypeConfiguration<Kontragent>
    {
        public void Configure(EntityTypeBuilder<Kontragent> item)
        {

            item.HasMany(i => i.Vacancies)
                .WithOne(r => r.Kontragent)
                .HasForeignKey(r => r.KontragentId);

            item.HasOne(r => r.User)
                .WithOne(r => r.Kontragent)
                .HasForeignKey<User>(r => r.KontragentId);
                
            item.HasOne<File>(i => i.Icon);

        }
    }
}