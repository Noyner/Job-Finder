using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Language;
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
    [ODataRoutePrefix(nameof(Language))]
    public class LanguageController : BaseController<LanguageController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;
        
        public LanguageController(ILogger<LanguageController> logger, UserDbContext userDbContext,
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
        [ProducesResponseType(typeof(IEnumerable<Language>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public IEnumerable<Language> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            return userDbContext.Languages;
        }

        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]Language item)
        {
            var user = await userManager.GetUserAsync(User);

            await userDbContext.Languages.AddAsync(item);

            await userDbContext.SaveChangesAsync();

           // await userManager.AddToRoleAsync(user, IdentityServer.Extensions.Constants.UserRoles.Kontragent);
            
            return Ok(item);
        }
        
    }
}