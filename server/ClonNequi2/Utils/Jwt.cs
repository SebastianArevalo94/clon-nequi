using ClonNequi2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClonNequi.Utils
{
    public class Jwt
    {

        public readonly ClonNequiContext _context;
        public readonly IConfiguration _configuration;

        public Jwt(ClonNequiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string GenerateToken(Cliente cliente)
        {
            IConfigurationSection jwt = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]));

            var claims = new[]
            {
                new Claim("id", cliente.Id.ToString()),
                new Claim("correo", cliente.Correo.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique identifier for the token
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["TokenLifetime"])).ToUniversalTime().Ticks.ToString()) // Set expiration time
            };

            SigningCredentials signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwt["Issuer"],
                Audience = jwt["Audience"],
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["TokenLifetime"])),
                SigningCredentials = signIn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                IConfigurationSection jwt = _configuration.GetSection("Jwt");

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"])),

                    ValidateIssuer = true,
                    ValidIssuer = jwt["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwt["Audience"],

                    ValidateLifetime = true
                };

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // Extract email claim if needed (consider security implications)
                string correo = securityToken.Claims.First(claim => claim.Type == "correo").Value;

                var clienteBD = _context.Clientes.Where(c => c.Correo == correo).FirstOrDefault();

                if(clienteBD == null)
                {
                    return false;
                }

                return true;
            }
            catch (SecurityTokenException ex)
            {
                // Log or handle specific security token exceptions (e.g., SignatureVerificationException)
                Console.WriteLine($"Security token validation error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                Console.WriteLine($"Unexpected error during token validation: {ex.Message}");
                return false;
            }
        }

    }
}
