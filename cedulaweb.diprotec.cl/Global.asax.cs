using Modelo;
using Negocio.Comunes;
using System;
using System.Web;

namespace cedulaweb.diprotec.cl
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            System.Web.Routing.RouteTable.Routes.MapPageRoute(
                "ModuloPublico",
                ConstantesModelo.ConsModuloUrl.Substring(1) + "{codigo}",
                "~" + ConstantesModelo.ConsModuloUrl + "default.aspx"
            );
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ReescribirUrlModuloPublico();
        }

        private void ReescribirUrlModuloPublico()
        {
            var path = Request.AppRelativeCurrentExecutionFilePath;

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!path.StartsWith("~/aplicacion/modulo/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var codigo = path.Substring("~/aplicacion/modulo/".Length);

            if (string.IsNullOrWhiteSpace(codigo))
            {
                return;
            }

            var urlReal = MenuNegocio.GetUrlRealModulo(codigo);

            if (string.IsNullOrWhiteSpace(urlReal))
            {
                return;
            }

            if (!urlReal.StartsWith("/"))
            {
                return;
            }

            var queryString = Request.Url.Query;

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                urlReal += queryString;
            }

            Context.Items["UrlPublicaActual"] = ConstantesModelo.ConsModuloUrl + codigo + queryString;

            Context.RewritePath(urlReal, false);
        }
    }
}