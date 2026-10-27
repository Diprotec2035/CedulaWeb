using Modelo;
using Modelo.v1.Visitas;
using Negocio;
using Negocio.v1.Visitas;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.visita
{
    public partial class frmVisitasImportarDetalle : Page
    {
        private uint IdCarga
        {
            get
            {
                if (!uint.TryParse(Request.QueryString["id"], out uint idCarga))
                {
                    return 0;
                }

                return idCarga;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (Session["empresa"] == null || Session["cliente"] == null)
            {
                Response.Redirect(ConstantesModelo.ConsHttpInicio, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Session["SesionExpira"] == null)
            {
                var minutosSesion = FuncionesNegocio.GetSessionTimeoutMinutos();

                HttpContext.Current.Session.Timeout = minutosSesion;
                HttpContext.Current.Session["SesionExpira"] = DateTime.Now.AddMinutes(minutosSesion);
            }

            if (!IsPostBack)
            {
                CargarPagina();
            }
        }

        private void CargarPagina()
        {
            if (IdCarga == 0)
            {
                MostrarAlertaRedireccion(
                    "CargaInvalida",
                    ConstantesModelo.ConsAlertaColorDanger,
                    string.Empty,
                    ConstantesModelo.ConsValidacionCargaIdInvalido,
                    ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)
                );

                return;
            }

            CargarCabecera();
            CargarDetalle();
        }

        private void CargarCabecera()
        {
            var rutEmpresa = Convert.ToString(Session["empresa"]);

            var respuesta = EmpresaVisitaCargaNegocio.GetEmpresaVisitaCargaId(IdCarga, rutEmpresa);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                MostrarAlertaRedireccion(
                    "CargaNoEncontrada",
                    ConstantesModelo.ConsAlertaColorDanger,
                    string.Empty,
                    respuesta != null && !string.IsNullOrWhiteSpace(respuesta.Mensaje)
                        ? respuesta.Mensaje
                        : ConstantesModelo.ConsValidacionVisitaCargaIdInvalido,
                    ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver)
                );

                return;
            }

            var carga = respuesta.Datos;

            litIdCarga.Text = carga.Id.ToString();
            litFecha.Text = carga.CreatedFecha;
            litHora.Text = carga.CreatedHora;

            litResponsable.Text = !string.IsNullOrWhiteSpace(carga.CreatedByNombre)
                ? carga.CreatedByNombre
                : carga.CreatedBy;
        }

        private void CargarDetalle()
        {
            var filtro = new EmpresaVisitaCargaDetalleFiltroModelo
            {
                IdCarga = IdCarga,
                RutEmpresa = Convert.ToString(Session["empresa"]),
                Estado = ConstantesModelo.ConsFiltroEstadoTodos,
                Accion = ObtenerAccionFiltro(),
                Coincidencia = txtCoincidencia.Text.Trim()
            };

            var respuesta = EmpresaVisitaCargaNegocio.GetEmpresaVisitaCargaDetalle(filtro);

            gvListado.DataSource = respuesta != null && respuesta.Exitoso
                ? respuesta.Datos
                : null;

            gvListado.DataBind();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;
            CargarDetalle();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;
            CargarDetalle();
        }

        protected void gvListado_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                return;
            }

            if (e.Row.Cells.Count == 0 || e.Row.Cells[0].Controls.Count == 0)
            {
                return;
            }

            var tablaPaginador = e.Row.Cells[0].Controls[0] as Table;

            if (tablaPaginador == null || tablaPaginador.Rows.Count == 0)
            {
                return;
            }

            tablaPaginador.CssClass = "pagination justify-content-center mb-0";

            foreach (TableCell celda in tablaPaginador.Rows[0].Cells)
            {
                celda.CssClass = "page-item";

                if (celda.Controls.Count == 0)
                {
                    continue;
                }

                var control = celda.Controls[0];

                var link = control as LinkButton;

                if (link != null)
                {
                    link.CssClass = "page-link";
                    continue;
                }

                var label = control as Label;

                if (label != null)
                {
                    label.CssClass = "page-link";
                    celda.CssClass = "page-item active";
                    continue;
                }

                var literal = control as LiteralControl;

                if (literal != null && !string.IsNullOrWhiteSpace(literal.Text))
                {
                    var texto = literal.Text.Trim();

                    celda.Controls.Clear();
                    celda.Controls.Add(new Label
                    {
                        Text = texto,
                        CssClass = "page-link"
                    });

                    celda.CssClass = "page-item active";
                }
            }
        }

        private sbyte ObtenerAccionFiltro()
        {
            if (!sbyte.TryParse(ddlResultado.SelectedValue, out sbyte accion))
            {
                return 9;
            }

            return accion;
        }

        private void MostrarAlertaRedireccion(
            string key,
            string alertaColor,
            string alertaTitulo,
            string mensaje,
            string url
        )
        {
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(alertaColor ?? ConstantesModelo.ConsAlertaColorDanger);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(alertaTitulo ?? string.Empty);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje ?? string.Empty);
            var urlJs = HttpUtility.JavaScriptStringEncode(url ?? ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver));

            var script =
                "$('#global-loader').hide();" +
                "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');" +
                "setTimeout(function(){ window.location = '" + urlJs + "'; }, 1200);";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                key,
                script,
                true
            );
        }
    }
}
