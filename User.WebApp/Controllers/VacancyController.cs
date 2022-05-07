using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Vacancys;
using CRM.DAL.Models.DatabaseModels.VacancysUsers;
using CRM.DAL.Models.DatabaseModels.VacansysUsers;
using CRM.User.WebApp.Models.Basic;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace CRM.User.WebApp.Controllers
{
    [ODataRoutePrefix(nameof(Vacancy))]
    public class VacancyController : BaseController<VacancyController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;
        
        public VacancyController(ILogger<VacancyController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.mapper = mapper;
        }
        
        /// <summary>
        ///     Get Vacancys.
        /// </summary>
        /// <returns>The requested Vacancys.</returns>
        /// <response code="200">The Vacancys was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Vacancy>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public IEnumerable<Vacancy> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            return userDbContext.Vacancys
                .IncludeOptimized(p => p.Requirements)
                .IncludeOptimized(p => p.Tags)
                .IncludeOptimized(p => p.VacancyKontragents);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]Vacancy item)
        {
            var user = await userManager.GetUserAsync(User);

            await userDbContext.Vacancys.AddAsync(item);

            item.VacancyUsers = new List<VacancyUser>();
            item.VacancyUsers.Add(new VacancyUser()
            {
                UserId = user.Id,
                RelationType = VacancyUserRelationType.Creator
            });

            await userDbContext.SaveChangesAsync();

            await userManager.AddToRoleAsync(user, IdentityServer.Extensions.Constants.UserRoles.Kontragent);
            
            return Ok(item);
        }
        
    }
}