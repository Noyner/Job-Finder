using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Resume;
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
    [ODataRoutePrefix(nameof(Resume))]
    public class ResumeController : BaseController<ResumeController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;
        
        public ResumeController(ILogger<ResumeController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.mapper = mapper;
        }
        
        
        /// <summary>
        ///     Get Resumes.
        /// </summary>
        /// <returns>The requested Vacancies.</returns>
        /// <response code="200">The Vacancies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Resume>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.True)]
        public async Task<IEnumerable<Resume>> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            var user = await userManager.GetUserAsync(User);
            var roles = await userManager.GetRolesAsync(user);
            
            return userDbContext.Resumes
                .IncludeOptimized(p => p.Creator.Avatar)
                .Where(r=> (!roles.Contains(UserRoles.Kontragent) && r.CreatorId == user.Id)||roles.Contains(UserRoles.Kontragent));
        }
        
        /// <summary>
        ///     Get Resumes.
        /// </summary>
        /// <returns>The requested Vacancies.</returns>
        /// <response code="200">The Vacancies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Resume>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.True)]
        public async Task<IActionResult> Get(Guid key)
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            var user = await userManager.GetUserAsync(User);
            var roles = await userManager.GetRolesAsync(user);

            var item = await userDbContext.Resumes
                .IncludeOptimized(p => p.Creator.Avatar)
                .FirstOrDefaultAsync(r => r.Id == key);

            if (item.CreatorId != user.Id && !roles.Contains(UserRoles.Kontragent))
            {
                return Forbid("Kontragent role required");
            }

            return Ok(item);
        }
        
        [Produces("application/json")]
        [ProducesResponseType(typeof(DAL.Models.DatabaseModels.Users.User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(Guid key, Delta<DAL.Models.DatabaseModels.Resume.Resume> delta,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await UserDbContext.Resumes
                .IncludeOptimized(i => i.OwnSkills)
                .FirstOrDefaultAsync(i => i.Id == key, cancellationToken);

            if (item == null)
            {
                return NotFound($"Не удалось найти vacancy с идентификатором {key}");
            }

            if (delta.GetChangedPropertyNames()
                .Any(a => a == nameof(item.Id) || a == nameof(item.CreatorId)))
            {
                return BadRequest("Запрет на редактирование свойства");
            }

            delta.Patch(item);

            await UserDbContext.SaveChangesAsync(cancellationToken);

            return Updated(item);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]Resume item)
        {
            var user = await userManager.GetUserAsync(User);

            await userDbContext.Resumes.AddAsync(item);

            item.CreatorId = user.Id;
            
            await userDbContext.SaveChangesAsync();

            return Ok(item);
        }
        
        
    }
}