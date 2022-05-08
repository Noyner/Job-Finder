using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.RequestModels.ChangePassword;
using CRM.User.WebApp.Models.Basic;
using CRM.User.WebApp.Models.Basic.User.UserProfileDto;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;
using ClaimTypes = CRM.IdentityServer.Extensions.Constants.ClaimTypes;

namespace CRM.User.WebApp.Controllers
{
    [ODataRoutePrefix(nameof(DAL.Models.DatabaseModels.Users.User))]
    public class UserController : BaseController<UserController>
    {

        private readonly IMapper mapper;

        public UserController(ILogger<UserController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.mapper = mapper;
        }

        /// <summary>
        ///     Get current User.
        /// </summary>
        /// <returns>The current User.</returns>
        /// <response code="200">The User was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(DAL.Models.DatabaseModels.Users.User), StatusCodes.Status200OK)]
        [ODataRoute("Profile")]
        [EnableQuery]
        public async Task<IActionResult> GetProfile()
        {
            QueryIncludeOptimizedManager.AllowIncludeSubPath = true;

            var userId = userManager.GetUserId(User);
            var user = await UserDbContext.Users
                .IncludeOptimized(i => i.UserRoles.Select(ur => ur.Role))
                .IncludeOptimized(i=>i.KontragentUsers.Select(r=>r.Kontragent))
                .IncludeOptimized(i=>i.VacancyUsers.Select(r=>r.Vacancy))
                .IncludeOptimized(r=>r.UserClaims)
                .FirstOrDefaultAsync(i => i.Id == userId);
            
            return StatusCode(StatusCodes.Status200OK, mapper.Map<UserProfileDto>(user));
        }
        
        /// <summary>
        ///     Updates an existing User.
        /// </summary>
        /// <param name="key">The requested User identifier.</param>
        /// <param name="delta">The partial User to update.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created User.</returns>
        /// <response code="204">The User was successfully updated.</response>
        /// <response code="400">The User is invalid.</response>
        /// <response code="404">The User does not exist.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(DAL.Models.DatabaseModels.Users.User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(string key, Delta<DAL.Models.DatabaseModels.Users.User> delta,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await UserDbContext.Users
                .IncludeOptimized(i => i.Avatar)
                .FirstOrDefaultAsync(i => i.Id == key, cancellationToken);

            if (item == null)
            {
                return NotFound($"Не удалось найти User с идентификатором {key}");
            }

            if (delta.GetChangedPropertyNames()
                .Any(a => a == nameof(item.UserName) || a == nameof(item.Email)))
            {
                return BadRequest("Запрет на редактирование свойства");
            }

            delta.Patch(item);

            await UserDbContext.SaveChangesAsync(cancellationToken);

            return Updated(item);
        }

        /// <summary>
        ///     Get current User Policies.
        /// </summary>
        /// <returns>The current User Policies.</returns>
        /// <response code="200">The Policies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ODataRoute("Policies")]
        public IEnumerable<string> GetPolicies()
        {
            return User.FindAll(ClaimTypes.UserPolicy).Select(i => i.Value);
        }

        /// <summary>
        ///     Get current User Roles.
        /// </summary>
        /// <returns>The current User Policies.</returns>
        /// <response code="200">The Policies was successfully retrieved.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ODataRoute("Roles")]
        public IEnumerable<string> GetRoles()
        {
            return User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(i => i.Value);
        }

        /// <summary>
        ///     Update User Password.
        /// </summary>
        /// <response code="200">The User password was successfully changed.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ODataRoute("ChangePassword")]
        public async Task<IdentityResult> PostChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await userManager.GetUserAsync(User);

            var result= await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            await UserDbContext.SaveChangesAsync();
            return result;
        }
        
    }
}