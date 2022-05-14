using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Files;
using CRM.DAL.Models.DatabaseModels.Kontragents;
using CRM.DAL.Models.DatabaseModels.KontragentUsers;
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
    [ODataRoutePrefix(nameof(Kontragent))]
    public class KontragentController : BaseController<KontragentController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;
        
        public KontragentController(ILogger<KontragentController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.mapper = mapper;
        }


        
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Kontragent>), StatusCodes.Status200OK)]
        [EnableQuery(HandleNullPropagation = HandleNullPropagationOption.False)]
        public IActionResult Get()
        {
           var items= userDbContext.Kontragents
                .IncludeOptimized(r => r.Icon)
                .IncludeOptimized(r => r.Info)
                .IncludeOptimized(r => r.Vacancies);
           
           return StatusCode(StatusCodes.Status200OK, items);
        }

        public async Task<IActionResult> Get(Guid key)
        {
            var item = await userDbContext.Kontragents
                .IncludeOptimized(r => r.Icon)
                .IncludeOptimized(r => r.Info)
                .IncludeOptimized(r => r.KontragentUsers)
                .IncludeOptimized(r => r.Vacancies)
                .FirstOrDefaultAsync(r=>r.Id==key);

            if (item == null)
            {
                return NotFound();
            }
            
            return StatusCode(StatusCodes.Status200OK, item);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]Kontragent item)
        {
            var user = await userManager.GetUserAsync(User);
            
            await userDbContext.Kontragents.AddAsync(item);
            item.KontragentUsers = new List<KontragentUser>();
            item.KontragentUsers.Add(new KontragentUser()
            {
                UserId = user.Id,
                RelationType = KontragentUserRelationType.Owner
            });

            await userDbContext.SaveChangesAsync();

           await userManager.AddToRoleAsync(user, IdentityServer.Extensions.Constants.UserRoles.Kontragent);
            
            return Ok(item);
        }
    }
}