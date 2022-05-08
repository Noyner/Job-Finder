using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Resume;
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
        ///     Get Vacancies.
        /// </summary>
        /// <returns>The requested Vacancies.</returns>
        /// <response code="200">The Vacancies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Resume>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public IEnumerable<Resume> Get()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            return userDbContext.Resumes
                .IncludeOptimized(p => p.ResumeSkills.Select(r=>r.Skill))
                .IncludeOptimized(r=>r.Language)
                .IncludeOptimized(r=>r.City)
                .IncludeOptimized(p => p.Creator);
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
                .IncludeOptimized(i => i.ResumeSkills)
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

           // await userManager.AddToRoleAsync(user, IdentityServer.Extensions.Constants.UserRoles.Kontragent);
            
            return Ok(item);
        }
        
        
    }
}