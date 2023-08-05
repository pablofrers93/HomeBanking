using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Client client)
        {
            try
            {
                // Buscamos al usuario en la base de datos por su correo electrónico.

                Client user = _clientRepository.FindByEmail(client.Email);
                // Verificamos si el usuario existe y si la contraseña proporcionada coincide.

                if (user == null || !String.Equals(user.Password, client.Password))
                    // Si las credenciales no son válidas, devolvemos un resultado Unauthorized.

                    return Unauthorized();

                // Si las credenciales son válidas, creamos una lista de Claims para el usuario.
                var claims = new List<Claim>
                {
                    // Agregamos un Claim con el tipo "Client" y el valor del correo electrónico del usuario.
                    new Claim("Client", user.Email)
                };

                // Creamos una identidad de Claims que representa al usuario autenticado.
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                // Creamos una cookie de autenticación con la identidad de Claims.
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                // Si todo ha ido bien, devolvemos un resultado OK para indicar que el inicio de sesión fue exitoso.
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
