using Modelo;
using Negocio.Comunes;
using System;
using System.IO;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.modulo
{
    public partial class _default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var codigo = Page.RouteData.Values["codigo"]?.ToString();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                GuardarAlertaModuloNoDisponible("Código público no informado.");
                RedirigirInicio();
                return;
            }

            var urlReal = MenuNegocio.GetUrlRealModulo(codigo);

            if (string.IsNullOrWhiteSpace(urlReal))
            {
                GuardarAlertaModuloNoDisponible("No existe una ruta asociada a la opción solicitada.");
                RedirigirInicio();
                return;
            }

            if (!urlReal.StartsWith("/"))
            {
                GuardarAlertaModuloNoDisponible("La ruta asociada no tiene un formato válido.");
                RedirigirInicio();
                return;
            }

            var rutaFisica = Server.MapPath(urlReal);

            if (!File.Exists(rutaFisica))
            {
                GuardarAlertaModuloNoDisponible("La página asociada no existe.");
                RedirigirInicio();
                return;
            }

            var queryString = Request.Url.Query;

            var urlPublicaActual = ConstantesModelo.ConsModuloUrl + codigo;

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                urlPublicaActual += queryString;
                urlReal += queryString;
            }

            Context.Items["UrlPublicaActual"] = urlPublicaActual;

            Server.Transfer(urlReal, false);
        }

        private void GuardarAlertaModuloNoDisponible(string detalle)
        {
            Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
            Session["AlertaTitulo"] = string.Empty;
            Session["AlertaMensaje"] = "No fue posible abrir la opción solicitada. " + detalle;
        }

        private void RedirigirInicio()
        {
            var urlRetorno = Request.UrlReferrer != null
                ? Request.UrlReferrer.ToString()
                : ConstantesModelo.ConsHttpAplicacion;

            Response.Redirect(urlRetorno, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}