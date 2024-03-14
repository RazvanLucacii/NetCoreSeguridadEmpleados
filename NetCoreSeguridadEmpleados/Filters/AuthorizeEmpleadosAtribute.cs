using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute
        , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //POR AHORA, NOS DA IGUAL QUIEN SEA EL EMPLEADO
            //SIMPLEMENTE QUE EXISTA
            var user = context.HttpContext.User;
            //necesitamos el controller y el action de donde hemos pulsado
            //previamente antes de entrar a este filter
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            Debug.WriteLine("Controller: " + controller);
            Debug.WriteLine("Action: " + action);
            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            var TempData = provider.LoadTempData(context.HttpContext);
            //guardamos en tempdata
            TempData["controller"] = controller;
            TempData["action"] = action;
            //devolvemos tempdata a la app
            provider.SaveTempData(context.HttpContext, TempData);


            if (user.Identity.IsAuthenticated == false)
            {
                //ENVIAMOS A LA VISTA LOGIN
                context.Result = this.GetRoute("Managed", "Login");
            }
            else
            {
                if (user.IsInRole("PRESIDENTE") == false && user.IsInRole("ANALISTA") == false && user.IsInRole("DIRECTOR") == false)
                {
                    context.Result = this.GetRoute("Managed", "ErrorAcceso");
                }
            }
        }

        //COMO TENDREMOS MULTIPLES REDIRECCIONES, CREAMOS UN METODO
        //PARA FACILITAR LA REDIRECCION
        private RedirectToRouteResult GetRoute
            (string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(
                    new { controller = controller, action = action });
            RedirectToRouteResult result =
                new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
