using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.VacancyResumes;
using CRM.IdentityServer.Extensions.Constants;
using CRM.User.WebApp.Models.Basic;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace CRM.User.WebApp.Controllers
{
    [ODataRoutePrefix(nameof(VacancyApplication))]
    public class VacancyApplicationController : BaseController<VacancyApplicationController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;
        
        public VacancyApplicationController(ILogger<VacancyApplicationController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.mapper = mapper;
        }
        
        /// <summary>
        ///     Get Vacancies.
        /// </summary>
        /// <returns>The requested Vacancies.</returns>
        /// <response code="200">The Vacancies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<VacancyApplication>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public async Task<IActionResult> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            var user = await userManager.GetUserAsync(User);

            var items= userDbContext
                .VacancyApplications
                .IncludeOptimized(r=>r.Resume.Creator)
                .IncludeOptimized(r=>r.Vacancy.Kontragent);

            items = items.Where(r =>
                r.Resume.CreatorId == user.Id ||
                r.Vacancy.Kontragent.UserId == user.Id
            );


            return Ok(items);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]VacancyApplication item)
        {

            var resume = await userDbContext.Resumes.FirstOrDefaultAsync(r => r.Id == item.ResumeId);
            var user = await userManager.GetUserAsync(User);

            if (resume.CreatorId != user.Id)
            {
                return BadRequest("Resume creatorId different from your user id!");
            }
            
            await userDbContext.VacancyApplications.AddAsync(item);

            await userDbContext.SaveChangesAsync();

            return Ok(item);
        }
        
    }
}