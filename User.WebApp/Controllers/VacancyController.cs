using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Vacancies;
using CRM.IdentityServer.Extensions.Constants;
using CRM.User.WebApp.Models.Basic;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        ///     Get Vacancies.
        /// </summary>
        /// <returns>The requested Vacancies.</returns>
        /// <response code="200">The Vacancies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Vacancy>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public IEnumerable<Vacancy> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            return userDbContext.Vacancies
                .IncludeOptimized(p => p.Kontragent.Icon);
        }
        
        /// <summary>
        ///     Get Vacancies.
        /// </summary>
        /// <returns>The requested Vacancies.</returns>
        /// <response code="200">The Vacancies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public async Task<Vacancy> Get(Guid key)
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            return await userDbContext.Vacancies
                .IncludeOptimized(p => p.Kontragent.Icon)
                .FirstOrDefaultAsync(r=>r.Id==key);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = UserRoles.Kontragent)]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]Vacancy item)
        {
            var user = await userManager.GetUserAsync(User);

            if (user.KontragentId == null)
            {
                return BadRequest("U haven't any own company");
            }

            item.KontragentId = user.KontragentId.Value;

            await userDbContext.Vacancies.AddAsync(item);

            await userDbContext.SaveChangesAsync();

            return Ok(item);
        }
        
        [Produces("application/json")]
        [ProducesResponseType(typeof(DAL.Models.DatabaseModels.Users.User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(Guid key, Delta<DAL.Models.DatabaseModels.Vacancies.Vacancy> delta,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await UserDbContext.Vacancies
                .IncludeOptimized(i => i.RequiredSkills)
                .FirstOrDefaultAsync(i => i.Id == key, cancellationToken);

            if (item == null)
            {
                return NotFound($"Не удалось найти vacancy с идентификатором {key}");
            }

            if (delta.GetChangedPropertyNames()
                .Any(a => a == nameof(item.Id) || a == nameof(item.KontragentId)))
            {
                return BadRequest("Запрет на редактирование свойства");
            }

            delta.Patch(item);

            await UserDbContext.SaveChangesAsync(cancellationToken);

            return Updated(item);
        }
    }
}