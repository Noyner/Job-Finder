using CRM.DAL.Models.DatabaseModels.City;
using CRM.DAL.Models.DatabaseModels.Configs;
using CRM.DAL.Models.DatabaseModels.Files;
using CRM.DAL.Models.DatabaseModels.KontragentInfo;
using CRM.DAL.Models.DatabaseModels.Kontragents;
using CRM.DAL.Models.DatabaseModels.KontragentUsers;
using CRM.DAL.Models.DatabaseModels.Language;
using CRM.DAL.Models.DatabaseModels.Resume;
using CRM.DAL.Models.DatabaseModels.ResumeSkill;
using CRM.DAL.Models.DatabaseModels.Skill;
using CRM.DAL.Models.DatabaseModels.Tags;
using CRM.DAL.Models.DatabaseModels.Users;
using CRM.DAL.Models.DatabaseModels.Vacancies;
using CRM.DAL.Models.DatabaseModels.VacancyResumes;
using CRM.DAL.Models.DatabaseModels.VacancySkills;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.DAL
{
    public class GeneralDbContext : IdentityDbContext<
        User, Role, string,
        UserClaim, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>, IDataProtectionKeyContext
    {
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        
        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<File> Files { get; set; }
        public DbSet<KontragentInfo> KontragentInfos { get; set; }
        public DbSet<Kontragent> Kontragents { get; set; }
        public DbSet<KontragentUser> KontragentUsers { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<VacancyApplication> VacancyApplications { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeSkill> ResumeSkills { get; set; }
        public DbSet<VacancySkill> VacancySkills { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Language> Languages { get; set; }
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     base.OnConfiguring(optionsBuilder);
        // }
        //FOR MIGRATIONS
        public GeneralDbContext()
        {

        }
        public GeneralDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("Host=jf-db-dev.cvt8etkjrnau.eu-central-1.rds.amazonaws.com;Port=5432;Database=postgres;Username=postgres;Password=jfdbmaster123;CommandTimeout=1000");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new VacancyApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new VacancyConfiguration());
            modelBuilder.ApplyConfiguration(new KontragentUserConfiguration());
            modelBuilder.ApplyConfiguration(new KontragentConfiguration());
            modelBuilder.ApplyConfiguration(new KontragentInfoConfiguration());
            modelBuilder.ApplyConfiguration(new FileConfiguration());
        }
    }
}