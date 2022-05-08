using CRM.DAL.Models.DatabaseModels.City;
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
using CRM.DAL.Models.DatabaseModels.VacancyResumes;
using CRM.DAL.Models.DatabaseModels.VacancySkills;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.User.WebApp.Models.Basic
{
    public class UserDbContext : IdentityDbContext<
        DAL.Models.DatabaseModels.Users.User, DAL.Models.DatabaseModels.Users.Role, string,
        UserClaim, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>, IDataProtectionKeyContext
    {
        public string UserId { get; set; }
        

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<File> Files { get; set; }
        
        public DbSet<KontragentInfo> KontragentInfos { get; set; }
        
        public DbSet<DAL.Models.DatabaseModels.Kontragents.Kontragent> Kontragents { get; set; }
        
        public DbSet<KontragentUser> KontragentUsers { get; set; }
        
        
        public DbSet<Skill> Skills { get; set; }
        public DbSet<DAL.Models.DatabaseModels.Resume.Resume> Resumes { get; set; }
        public DbSet<ResumeSkill> ResumeSkills { get; set; }
        public DbSet<VacancySkill> VacancySkills { get; set; }
        public DbSet<DAL.Models.DatabaseModels.City.City> Cities { get; set; }
        public DbSet<DAL.Models.DatabaseModels.Language.Language> Languages { get; set; }
        public DbSet<DAL.Models.DatabaseModels.Vacancies.Vacancy> Vacancies { get; set; }
        public DbSet<DAL.Models.DatabaseModels.VacancyResumes.VacancyApplication> VacancyApplications { get; set; }

        public DbSet<Skill> Tags { get; set; }
        



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new KontragentUserConfiguration());
            modelBuilder.ApplyConfiguration(new KontragentConfiguration());
            modelBuilder.ApplyConfiguration(new KontragentInfoConfiguration());
            modelBuilder.ApplyConfiguration(new FileConfiguration());

        }
    }
}