using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM.DAL.Models.DatabaseModels.Files;
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
    public class FileController : BaseController<FileController>
    {

        private readonly IMapper mapper;
        private readonly UserDbContext userDbContext;
        
        public FileController(ILogger<FileController> logger, UserDbContext userDbContext,
            UserManager<DAL.Models.DatabaseModels.Users.User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            logger, userDbContext,
            userManager, httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.mapper = mapper;
        }
        
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]File item)
        {
            var user = await userManager.GetUserAsync(User);
            item.Id=Guid.NewGuid();
            switch (item.Type)
            {
                case FileType.Unknown:
                    return BadRequest("Unknown file type isn't permited");
                case FileType.UserAvatar:
                    await userDbContext.Files.AddAsync(item);
                    user.AvatarId = item.Id;
                    break;
                case FileType.KontragentAvatar:
                    var kontragent = await userDbContext.Kontragents.FirstOrDefaultAsync(r => r.UserId == user.Id);
                    if (kontragent == null)
                    {
                        return BadRequest("You have no kontragent!");
                    }
                    await userDbContext.Files.AddAsync(item);
                    kontragent.IconId = item.Id;
                    break;
                
            }

            await userDbContext.SaveChangesAsync();

            return Ok(item);
        }
    }
}