using System.Threading.Tasks;
using jrs.Interfaces;
using jrs.Models;
using jrs.Services;
using jrs.DBContexts;
using System.Linq;
// using jrs.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;



using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;


namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class AuthController : ControllerBase
    {

        private readonly GeneralContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;
        public AuthController(
            GeneralContext context,
            ILogger<AuthController> logger,
            IConfiguration config
        // ,IRepository rep
        )
        {
            this._context = context;
            this._config = config;
            this._logger = logger;
            // this._rep = rep;
        }
        [HttpPost]
        [Route("loginext")]
        public async Task<ActionResult<User>>  LoginExt(string username, [FromBody] LoginData data)
        {
            string password= data.password;
            ActionResult<User> actUsr=null;
            User usr = null;
            Task<ActionResult<User>> task = Login(username,password);
            task.Wait();
            actUsr = task.Result;
            usr = actUsr.Value;
            var u = _context.ImsUser.SingleOrDefault<ImsUser>(user =>
                   user.ImsUserIsDeleted == false);
        
           if(usr==null || u == null)
                return NotFound(new { message = "Error: Incorrect username or password.", translationKey = "error.bad-login" });
 

            // create a claim for each requested scope
            //var claims = data.RequestedScopes.Select(s => new Claim("Scope", s));

            var claimsIdentity = new ClaimsIdentity(
                data.RequestedScopes.Select(s => new Claim("Scope", s)), CookieAuthenticationDefaults.AuthenticationScheme);

            //var user = new ClaimsPrincipal(claimsIdentity);
            
            HttpContext.User.AddIdentity(claimsIdentity);   
            // await HttpContext.SignInAsync(
            //     CookieAuthenticationDefaults.AuthenticationScheme,
            //     HttpContext.User);
            
            return Ok(usr);
        }
        // [HttpPost]
        // [Route("login")]
        // public async Task<IActionResult> Login([FromBody] LoginData data)
        // {
        //     // create a claim for each requested scope
        //     var claims = data.RequestedScopes.Select(s => new Claim("Scope", s));

        //     var claimsIdentity = new ClaimsIdentity(
        //         claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //     var user = new ClaimsPrincipal(claimsIdentity);

        //     await HttpContext.SignInAsync(
        //         CookieAuthenticationDefaults.AuthenticationScheme,
        //         user);

        //     return Ok();
        // }
        /// <summary>
        /// Authentication method
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>a User instance with authentication token</returns>
        [HttpPost("/login", Name = "Login")]
        [AllowAnonymous]
        // public async Task<User> Login(string username,[FromBody] string password)
        public async Task<ActionResult<User>> Login(string username, [FromBody] string password)
        {
            try
            {
                var cryptoService = new CryptoService(_config);
                //Recover crypted password
                var cryptedPassword = cryptoService.GetHash(password);

                var u = _context.ImsUser.SingleOrDefault<ImsUser>(user =>
                   user.ImsUserUsername == username &&
                   user.ImsUserPassword == cryptedPassword && 
                   user.ImsUserIsDeleted == false);

                if (u == null)
                {
                    _logger.LogError("Error: Incorrect username or password.", new object[] { username });
                    return NotFound(new { message = "Error: Incorrect username or password.", translationKey = "error.bad-login" });
                }

                TokenService tokenService = new TokenService(_config);
                string token = tokenService.GetToken(u);
                string refreshToken = tokenService.GetRefreshToken();

                //Save user refresh token
                _context.Entry(u).State = EntityState.Modified;
                try
                {
                    u.ImsUserRefreshToken = refreshToken;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                // if(!string.IsNullOrWhiteSpace(token)){
                //     return new User(u, token);
                // }
                if (!string.IsNullOrWhiteSpace(token))
                {
                    return new User(u, token, refreshToken);
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error auth", new object[] { username });
            }
            //TODO: Return http error code 500
            return null;
        }


        /// <summary>
        /// Authentication using Microsoft Azure AD
        /// </summary>
        /// <param name="accountIdentifier">accountIdentifier</param>
        /// <param name="useUsername">Defines if the method param "accountIdentifier" is the "userName"</param>
        /// <returns>a User instance with authentication token</returns>
        [HttpPost("/loginFromAzure", Name = "loginFromAzure")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> loginFromAzure(string accountIdentifier, bool useUsername = false)
        {
            try
            {
                ImsUser u;

                // Use userName for the authenticatio as opposed to the accountIdentifier
                if (useUsername)
                {
                    u = _context.ImsUser.SingleOrDefault<ImsUser>(user =>
                   user.ImsUserEmail == accountIdentifier && 
                   user.ImsUserIsDeleted == false
                    );
                }
                else
                {
                    // Get user based on "accountIdentifier"
                    u = _context.ImsUser.SingleOrDefault<ImsUser>(user =>
                       user.accountIdentifier == accountIdentifier && 
                        user.ImsUserIsDeleted == false
                        );
                }

                if (u == null)
                {
                    _logger.LogError("Error: Incorrect accountIdentifier.", new object[] { accountIdentifier });
                    return NotFound(new { message = "Error: Incorrect accountIdentifier.", translationKey = "error.bad-login" });
                }

                TokenService tokenService = new TokenService(_config);
                string token = tokenService.GetToken(u);
                string refreshToken = tokenService.GetRefreshToken();

                //Save user refresh token
                _context.Entry(u).State = EntityState.Modified;
                try
                {
                    u.ImsUserRefreshToken = refreshToken;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                if (!string.IsNullOrWhiteSpace(token))
                {
                    return new User(u, token, refreshToken);
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error auth", new object[] { accountIdentifier });
            }
            //TODO: Return http error code 500
            return null;
        }


        /// <summary>
        /// Generate and save new JwtToken and RefreshToken returning them to the caller
        /// </summary>
        /// <param name="token">expired authorization token</param>
        /// <param name="refreshToken">user's refresh token</param>
        /// <returns>An object in the form { token:string, refreshtoken:string }</returns>
        [HttpPost("/refresh-token", Name = "Refresh_Token")]
        public async Task<IActionResult> RefreshToken(string token, string refreshToken)
        {
            TokenService tokenService = new TokenService(_config);
            var principal = tokenService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var imsUser = _context.ImsUser.SingleOrDefault<ImsUser>(user => user.ImsUserUsername == username);
            var savedRefreshToken = imsUser.ImsUserRefreshToken;

            //Check if the provvided refresh token is valid for the given authorization token
            if (savedRefreshToken != refreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newJwtToken = tokenService.GetToken(imsUser);
            var newRefreshToken = tokenService.GetRefreshToken();

            //Update reftresh token
            _context.Entry(imsUser).State = EntityState.Modified;
            try
            {
                imsUser.ImsUserRefreshToken = newRefreshToken;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
    }
    public class LoginData
    {
        public string password;
        public string[] RequestedScopes { get; set; }
    }
}
