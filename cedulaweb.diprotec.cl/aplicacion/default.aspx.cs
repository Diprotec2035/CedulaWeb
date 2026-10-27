using Modelo;
using Modelo.Comunes;
using Negocio.Comunes;
using Negocio.v1.AvisoPlataforma;
using Negocio.v1.Modulo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace cedulaweb.diprotec.cl.aplicacion
{
    public partial class _default : Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await CargarIndicadoresEconomicos();
                await CargarNoticiasHome();
                CargarAvisoPlataforma();
            }
        }

        private async Task CargarIndicadoresEconomicos()
        {
            var respuesta = await IndicadorEconomicoNegocio.GetIndicadoresHome();

            if (!respuesta.Exitoso || respuesta.Datos == null || respuesta.Datos.Count == 0)
            {
                LimpiarIndicadoresEconomicos();
                return;
            }

            CargarIndicador(respuesta.Datos, "uf", litUf, litUfFecha);
            CargarIndicador(respuesta.Datos, "dolar", litDolar, litDolarFecha);
            CargarIndicador(respuesta.Datos, "euro", litEuro, litEuroFecha);
            CargarIndicador(respuesta.Datos, "utm", litUtm, litUtmFecha);

            var ultimaFecha = respuesta.Datos
                .Where(indicador => indicador.Fecha != DateTime.MinValue)
                .OrderByDescending(indicador => indicador.Fecha)
                .Select(indicador => indicador.Fecha)
                .FirstOrDefault();
        }

        private void CargarIndicador(List<IndicadorEconomicoModelo> indicadores, string codigo, Literal literalValor, Literal literalFecha)
        {
            var indicador = indicadores.FirstOrDefault(item =>
                string.Equals(item.Codigo, codigo, StringComparison.OrdinalIgnoreCase));

            if (indicador == null || string.IsNullOrWhiteSpace(indicador.ValorFormateado))
            {
                literalValor.Text = "$ --";
                literalFecha.Text = "Sin fecha disponible";
                return;
            }

            literalValor.Text = HttpUtility.HtmlEncode(indicador.ValorFormateado);

            if (indicador.Fecha == DateTime.MinValue)
            {
                literalFecha.Text = "Sin fecha disponible";
                return;
            }

            literalFecha.Text = "Valor al " + indicador.Fecha.ToString("dd-MM-yyyy");
        }

        private void LimpiarIndicadoresEconomicos()
        {
            litUf.Text = "$ --";
            litDolar.Text = "$ --";
            litEuro.Text = "$ --";
            litUtm.Text = "$ --";

            litUfFecha.Text = "Sin fecha disponible";
            litDolarFecha.Text = "Sin fecha disponible";
            litEuroFecha.Text = "Sin fecha disponible";
            litUtmFecha.Text = "Sin fecha disponible";
        }

        private async Task CargarNoticiasHome()
        {
            var respuesta = await NoticiaNegocio.GetNoticiasHome();

            if (!respuesta.Exitoso || respuesta.Datos == null || respuesta.Datos.Count == 0)
            {
                rptNoticias.DataSource = null;
                rptNoticias.DataBind();

                pnlNoticiasNoDisponibles.Visible = true;
                return;
            }

            rptNoticias.DataSource = respuesta.Datos;
            rptNoticias.DataBind();

            pnlNoticiasNoDisponibles.Visible = false;
        }

        private void CargarAvisoPlataforma()
        {
            var operacion = 0;

            var aviso = AvisoPlataformaNegocio.GetAvisoPlataformaVigente(ref operacion);

            divAvisoPlataforma.Visible = true;

            if (operacion == 0 && aviso != null && aviso.Id > 0)
            {
                litAvisoTitulo.Text = HttpUtility.HtmlEncode(aviso.Titulo);
                litAvisoMensaje.Text = HttpUtility.HtmlEncode(aviso.Mensaje);
                litAvisoVersion.Text = HttpUtility.HtmlEncode(aviso.Version);

                pAvisoVersion.Visible = !string.IsNullOrWhiteSpace(aviso.Version);

                return;
            }

            litAvisoTitulo.Text = "Avisos de la plataforma";
            litAvisoMensaje.Text = "Espacio reservado para mensajes internos, mantenimientos o novedades de la plataforma.";

            litAvisoVersion.Text = string.Empty;
            pAvisoVersion.Visible = false;
        }

        protected string GetUrlAccesoRapido(string urlPublica)
        {
            var respuesta = ModuloNegocio.GetModuloNavegacion(
                string.Empty,
                urlPublica
            );

            if (!respuesta.Exitoso || respuesta.Datos == null || respuesta.Datos.Id == 0)
            {
                return "javascript:void(0);";
            }

            return ConstantesModelo.ConsModuloUrl +
                respuesta.Datos.UrlPublica +
                "?mod_id=" +
                HttpUtility.UrlEncode(Convert.ToString(respuesta.Datos.Id));
        }
    }
}