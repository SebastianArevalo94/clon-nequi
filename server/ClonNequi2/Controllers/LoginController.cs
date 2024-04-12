using ClonNequi.Utils;
using ClonNequi2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;

namespace ClonNequi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ClonNequiContext _context;
        private readonly IConfiguration _configuration;
        private readonly Jwt _jwt;

        public LoginController(ClonNequiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _jwt = new Jwt(_context, _configuration);
        }

        [HttpPost]
        [Route("SignUp")]
        public IActionResult SignUp([FromBody] Cliente cliente)
        {
            try
            {
                string token = _jwt.GenerateToken(cliente);

                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                var entry = _context.Entry(cliente);

                Cuenta cuenta = new Cuenta { ClienteId = entry.Entity.Id, Balance = 0 };

                _context.Cuenta.Add(cuenta);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Usuario Registrado!!",
                    token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message,
                });
            }
        }
        [HttpPost]
        [Route("SignIn")]
        public IActionResult SignIn([FromBody]InfoLogin loginData)
        {
            try
            {
                var clienteDb = _context.Clientes.Where(c => c.Numero == loginData.Numero).FirstOrDefault();
                if (clienteDb != null)
                {
                    if (clienteDb.Contrasenia == loginData.Contrasenia)
                    {

                        string token = _jwt.GenerateToken(clienteDb);

                        return StatusCode(StatusCodes.Status200OK, new
                        {
                            message = $"Bienvenido {clienteDb.Nombres}",
                            token
                        });

                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            message = "La contrasenia es incorrecta!"
                        });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "EL usuario no existe!"
                    });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message,
                });
            }
        }

    }
}
