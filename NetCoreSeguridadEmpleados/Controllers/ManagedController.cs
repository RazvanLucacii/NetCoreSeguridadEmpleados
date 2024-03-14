using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryEmpleados repo;

        public ManagedController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login
            (string username, string password)
        {
            int idEmpleado = int.Parse(password);
            Empleado empleado = await
                this.repo.LoginEmpleadoAsync(username, idEmpleado);
            if (empleado != null)
            {
                //SEGURIDAD
                ClaimsIdentity identity =
                    new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);
                //CREAMOS EL CLAIM PARA EL NOMBRE (APELLIDO)
                Claim claimName =
                    new Claim(ClaimTypes.Name, empleado.Apellido);
                identity.AddClaim(claimName);
                Claim claimId =
                    new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString());
                identity.AddClaim(claimId);
                Claim claimOficio =
                    new Claim(ClaimTypes.Role, empleado.Oficio);
                identity.AddClaim(claimOficio);
                Claim claimSalario =
                    new Claim("Salario", empleado.Salario.ToString());
                identity.AddClaim(claimSalario);
                Claim claimDepartamento =
                    new Claim("Departamento", empleado.IdDepartamento.ToString());
                identity.AddClaim(claimDepartamento);
                //COMO POR AHORA NO VOY A UTILIZAR NI SE UTILIZAR ROLES
                //NO LO INCLUIMOS
                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal);
                //LO VAMOS A LLEVAR A UNA VISTA con la informacion
                //que nos devuelve el filter en tempdata
                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();
                return RedirectToAction(action, controller);
            }
            else
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
