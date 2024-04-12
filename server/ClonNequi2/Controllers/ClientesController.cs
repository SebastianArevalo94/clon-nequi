using ClonNequi.Utils;
using ClonNequi2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClonNequi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ClonNequiContext _context;
        private readonly IConfiguration _configuration;
        private readonly Jwt _jwt;

        public ClientesController(ClonNequiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _jwt = new Jwt(context, configuration);
        }

        [HttpGet]
        [Route("GetCuenta/{Id:int}")]
        public IActionResult GetCuenta(int Id)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString();

                bool isValidToken = _jwt.ValidateToken(token);

                if (!isValidToken)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Token Invalido!",
                    });
                }

                var cuenta = _context.Cuenta.Where(c => c.ClienteId == Id).FirstOrDefault();
                
                if (cuenta == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Cuenta no encontrada!",
                    });
                }

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "Get Cuenta Success!",
                    data = cuenta
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

        [HttpGet]
        [Route("GetMovimientos/{CuentaId:int}")]
        public IActionResult GetMovimientos(int CuentaId)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString();

                bool isValidToken = _jwt.ValidateToken(token);

                if (!isValidToken)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Token Invalido!",
                    });
                }

                var movimientos = _context.Movimientos.Where(m => m.CuentaId == CuentaId).ToList();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "Success!",
                    data = movimientos
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
        [Route("EnviarDinero")]
        public IActionResult EnviarDinero([FromBody] SendMoney data)
        {
            try
            {
                //Token Validation

                string token = HttpContext.Request.Headers["Authorization"].ToString();

                bool isValidToken = _jwt.ValidateToken(token);

                if (!isValidToken)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        message = "Token Invalido"
                    });
                }

                // Obtener cuenta (from)
                var from = _context.Cuenta.FirstOrDefault(c => c.ClienteId == data.MyId);

                if (from == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Cuenta no encontrada!"
                    });
                }

                // Obtener cliente con el numero
                var toCliente = _context.Clientes.FirstOrDefault(c => c.Numero == data.ToNumber);

                if (toCliente == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Cuenta no encontrada!"
                    });
                }

                //Obtener Cuenta del Cliente con su Id

                var toCuenta = _context.Cuenta.FirstOrDefault(c => c.ClienteId == toCliente.Id);

                if (toCuenta == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Cuenta no encontrada!"
                    });
                }

                //Validacion si es la misma cuenta

                if(from.ClienteId == toCuenta.ClienteId)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "No puedes enviarte dinero a ti mismo."
                    });
                }

                
                //Validacion Saldo

                if(data.Cantity > from.Balance)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Saldo Insuficiente!"
                    });
                }


                // Restar dinero de cuenta from y añadir a cuenta to
                from.Balance -= data.Cantity;
                toCuenta.Balance += data.Cantity;


                Movimiento movimientoFrom = new Movimiento
                {
                    CuentaId = from.Id,
                    ClienteId = toCuenta.ClienteId,
                    Fecha = DateTime.Now,
                    Balance = -data.Cantity,
                    TipoMovimiento = 1 //Tipo Envio
                };

                Movimiento movimientoTo = new Movimiento
                {
                    CuentaId = toCuenta.Id,
                    ClienteId = from.ClienteId,
                    Fecha = DateTime.Now,
                    Balance = data.Cantity,
                    TipoMovimiento = 2 //Tipo Recibido
                };

                _context.Movimientos.Add(movimientoFrom);
                _context.Movimientos.Add(movimientoTo);
                
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = $"Dinero enviado correctamente!"
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
    }
}
