using System;
using System.Linq;
using System.Threading.Tasks;
using CRM.DAL.Models.DatabaseModels.Users;
using CRM.DAL.Models.DatabaseModels.Users.VerifyCodes.Enums;
using CRM.IdentityServer.Extensions.Constants;
using CRM.IdentityServer.Models;
using CRM.IdentityServer.Services;
using CRM.IdentityServer.ViewModels;
using CRM.IdentityServer.ViewModels.Account;
using CRM.ServiceCommon.Services;
using CRM.ServiceCommon.Services.CodeService;
using Hangfire;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CRM.IdentityServer.Controllers.Api
{
    [SecurityHeaders]
    [AllowAnonymous]
    [Controller]
    [DisableConcurrentExecution(10)]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{apiVersion:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly IClientStore clientStore;
        private readonly IAuthenticationSchemeProvider schemeProvider;
        private readonly IEventService events;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IdentityServerDbContext identityServerDbContext;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly ICodeService codeService;


        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IdentityServerDbContext identityServerDbContext,
            IEmailService emailService,
            IConfiguration configuration, ICodeService codeService)
        {
            this.interaction = interaction;
            this.clientStore = clientStore;
            this.schemeProvider = schemeProvider;
            this.events = events;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identityServerDbContext = identityServerDbContext;
            this.emailService = emailService;
            this.configuration = configuration;
            this.codeService = codeService;
        }

        [HttpPost]
        [Route("[action]")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var needsContinue = true;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }


            var userWithName =
                await identityServerDbContext.Users.FirstOrDefaultAsync(u => u.UserName == model.Username);
            if (userWithName != null || identityServerDbContext.Users.Any(u => u.UserName == model.Username))
            {
                needsContinue = false;
                ModelState.AddModelError(nameof(model.Username), "Этот ник уже зарегистрирован!");
            }

            var userWithEmail =
                await identityServerDbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (userWithEmail != null || identityServerDbContext.Users.Any(u => u.Email == model.Email))
            {
                needsContinue = false;
                ModelState.AddModelError(nameof(model.Username), "Этот адрес электронной почты уже зарегистрирован!");
            }

            //@TODO: EMAIL

            if (needsContinue == false)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = false,
                IsActive = false,
            };
            var passwordValidator = new PasswordValidator<User>();
            var isPasswordValid = await passwordValidator.ValidateAsync(userManager, user, model.Password);
            if (!isPasswordValid.Succeeded)
            {
                ModelState.AddModelError(nameof(model.Password), "Пароль не удовлетворяет требованиям безопасности");
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }

            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.Password);

            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
                return BadRequest(errors);
            }

            await userManager.AddToRoleAsync(user, UserRoles.User);
            await signInManager.SignInAsync(user, true);

            user.EmailConfirmed = true;
            user.IsActive = true;

            await identityServerDbContext.SaveChangesAsync();

            return Ok("Registration done!"); //RedirectToAction("RegisterContinue", "Account");
        }
    }
}