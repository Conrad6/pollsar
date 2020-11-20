using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Pollsar.Shared.Models;
using Pollsar.Web.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pollsar.Web.Server.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly PollsarContext pollsarContext;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly UserManager<Models.User> userManager;
        private readonly SignInManager<Models.User> signInManager;
        private const string pageLinkFormat = "{0}://{1}/api/v1/polls?page={2}&size={3}";

        public UsersController (PollsarContext pollsarContext, IMapper mapper, UserManager<Models.User> userManager, SignInManager<Models.User> signInManager, IConfiguration configuration)
        {
            this.pollsarContext = pollsarContext;
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        // POsT: api/v1/<UsersController>/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync ()
        {
            var authorizationHeader = Request.Headers["Authorization"];
            if (authorizationHeader.Count == 0) return BadRequest(new { error = "Authorization header required " });

            var authorizationHeaderValue = authorizationHeader.ToString();

            if (authorizationHeaderValue.Split(' ').First() != "Basic") return BadRequest(new { error = "Basic authentication scheme required" });

            var authB64 = authorizationHeaderValue.Split(' ').Last();

            var emailAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(authB64));
            var email = emailAndPassword.Split(':').First();
            var password = emailAndPassword.Split(':').Last();

            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return NotFound(new { error = $"Account not found for email address {email}" });

            var signInResult = await signInManager.PasswordSignInAsync(user, password, true, false);
            if (!signInResult.Succeeded)
            {
                string error;
                if (signInResult.IsLockedOut) error = "You have been locked out";
                else if (signInResult.IsNotAllowed) error = "You are not allowed to access this account";
                else if (signInResult.RequiresTwoFactor) error = "This account requires Two Factor Authentication";
                else error = "Invalid email or password provided";

                return Problem(detail: error, title: "Authentication Failed", statusCode: 401);
            }

            var userRoles = await userManager.GetRolesAsync(user);

            var authToken = BuildAuthenticationToken(user, userRoles);

            return Json(new { authToken });
        }

        private string BuildAuthenticationToken (Models.User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.Names)
            };

            if(roles is { } && roles.Any())
            {
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var exp = DateTime.UtcNow.AddMonths(2);

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(claims: claims, expires: exp, signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/v1/<UsersController>
        [HttpGet]
        public async Task<ActionResult<ResponseEntity<UserViewModel>>> Get ([FromQuery] int page = 0, [FromQuery] int size = 50)
        {
            if (page < 0) return BadRequest(new { error = "Page cannot be less than zero" });
            if (size <= 0) return BadRequest(new { error = "Page size cannot be less than or equal to zero" });

            var query = pollsarContext.Users;

            var users = await query
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

            var totalCount = await pollsarContext.Users.CountAsync();

            var totalPages = System.Math.Ceiling((double) totalCount / size);

            bool nextPageExists = await query.Skip((page + 1) * size).AnyAsync(), previousPageExists = page > 0;

            string nextPage = null, lastPage = null, previousPage = null;
            StringBuilder pageLinkBuilder = null;
            if (nextPageExists)
            {
                pageLinkBuilder ??= new StringBuilder();

                pageLinkBuilder.AppendFormat(pageLinkFormat, Request.Scheme, Request.Host, page + 1, size);
                nextPage = pageLinkBuilder.ToString();
            }

            if (previousPageExists)
            {
                pageLinkBuilder ??= new StringBuilder();
                pageLinkBuilder.Clear();
                pageLinkBuilder.AppendFormat(pageLinkFormat, Request.Scheme, Request.Host, page - 1, size);
                previousPage = pageLinkBuilder.ToString();
            }

            pageLinkBuilder ??= new StringBuilder();
            pageLinkBuilder.Clear();
            pageLinkBuilder.AppendFormat(pageLinkFormat, Request.Scheme, Request.Host, totalPages - 1, size);
            lastPage = pageLinkBuilder.ToString();

            var response = new ResponseEntity<UserViewModel>
            {

                Content = users.Select(p => mapper.Map<UserViewModel>(p)),
                Page = page,
                Size = size,
                TotalCount = totalCount,
                TotalPages = (int) totalPages,
                NextPage = nextPage,
                PreviousPage = previousPage,
                LastPage = lastPage
            };

            return Json(response);
        }

        // GET api/v1/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> Get (long id)
        {
            var user = await pollsarContext.Users.FindAsync(id);
            if (user is null) return NotFound();

            var userEntry = pollsarContext.Entry(user);
            await userEntry.Navigation(nameof(Models.User.PollsCreated)).LoadAsync();

            return mapper.Map<UserViewModel>(user);
        }

        // POST api/v1/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] NewUserViewModel newUser)
        {
            if (newUser is null) return BadRequest();

            if (!TryValidateModel(newUser)) return ValidationProblem();

            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is { }) return Conflict(newUser);

            var _newUser = mapper.Map<Models.User>(newUser);

            var result = await userManager.CreateAsync(_newUser, newUser.Password);
            if (!result.Succeeded) return UnprocessableEntity(new { errors = result.Errors });

            await userManager.AddToRoleAsync(_newUser, "User");

            var link = $"{Request.Scheme}://{Request.Host}/api/users/{_newUser.Id}";

            return Created(link, mapper.Map<UserViewModel>(_newUser));
        }

        // PUT api/v1/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put (int id, [FromBody] UserViewModel userViewModel)
        {
            var user = await pollsarContext.Users.FindAsync(id);
            if (user is null) return NotFound(id);

            if (!TryValidateModel(userViewModel)) return ValidationProblem();

            var userEntry = pollsarContext.Entry(user);
            foreach (var property in userViewModel.GetType().GetProperties().Where(p => !p.GetCustomAttributes(typeof(EditableAttribute), true).Any()))
            {
                userEntry.Property(property.Name).CurrentValue = property.GetValue(userViewModel);
            }

            await pollsarContext.SaveChangesAsync();

            var link = $"{Request.Scheme}://{Request.Host}/api/users/{id}";

            return Accepted(link);
        }

        // DELETE api/v1/<UsersController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete (long id)
        {
            var user = await pollsarContext.Users.FindAsync(id);
            if (user is null) return NotFound(id);

            var userIsAdmin = await userManager.IsInRoleAsync(user, "Admin");
            if (userIsAdmin) return Unauthorized(new { error = "You cannot delete an administrator's account" });

            var userEntry = pollsarContext.Entry(user);
            foreach (var navigation in userEntry.Navigations) await navigation.LoadAsync();

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded) return Ok();

            return Problem();
        }

        // DELETE api/v1/<UsersController>
        [HttpDelete, Authorize]
        public async Task<IActionResult> DeleteUserAsync ()
        {

            var user = await userManager.GetUserAsync(User);
            await signInManager.SignOutAsync();

            await userManager.DeleteAsync(user);

            return Ok();
        }
    }
}
