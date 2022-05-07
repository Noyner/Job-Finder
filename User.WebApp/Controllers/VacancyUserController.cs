using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.VacancysUsers;
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
    [ODataRoutePrefix(nameof(VacancyUser))]
    public class VacancyUserController : BaseController<VacancyUserController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;

        public VacancyUserController(ILogger<VacancyUserController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.mapper = mapper;
        }


        /// <summary>
        ///     Get VacancyUsers.
        /// </summary>
        /// <returns>The requested VacancyUsers.</returns>
        /// <response code="200">The VacancyUsers was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<VacancyUser>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public async Task<IEnumerable<VacancyUser>> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            var user = await userManager.GetUserAsync(User);
            
            return userDbContext.VacancyUsers
                .IncludeOptimized(r => r.Vacancy.Requirements)
                .IncludeOptimized(r => r.Vacancy.Tags)
                .IncludeOptimized(r => r.Vacancy.VacancyKontragents.Select(r => r.Kontragent))
                .Where(r=>r.UserId==user.Id);
        }
        
    }
}