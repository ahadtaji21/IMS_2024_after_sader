using System;
using jrs.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace jrs{
    public interface IJWTTokenService
    {
        string GetToken(ImsUser user);
    }

    public class TokenService : IJWTTokenService{
        private IConfiguration _config;
        // private readonly AppSettings _appSettings;

        public TokenService(){

        }
        public TokenService(IConfiguration config){
            this._config = config;
        }

        /// <summary>
        /// Generates authentication token
        /// </summary>
        /// <param name="user">ImsUser for which to generate an authentication token</param>
        /// <returns>Authentication token</returns>
        public string GetToken(ImsUser user){
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _config.GetValue<string>("AppSettings:secret");
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new Claim[]{
                    // new Claim(ClaimTypes.Name, user.ImsUserUid.ToString())
                     new Claim(ClaimTypes.Name, user.ImsUserUsername.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1), //DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Extracts user information from expired JWT token in the form of a ClaimsPrincipal
        /// </summary>
        /// <param name="token">The expired token to retrieve data from</param>
        /// <returns>User information</returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token){
            var secret = _config.GetValue<string>("AppSettings:secret");
            var key = Encoding.ASCII.GetBytes(secret);
            
            var tokenValidationParameters = new TokenValidationParameters{
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)){
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public string GetRefreshToken(){
            var randomNbr = new byte[32];
            using( var rng = RandomNumberGenerator.Create()){
                rng.GetBytes(randomNbr);
                return Convert.ToBase64String(randomNbr);
            }
        }


    }

}