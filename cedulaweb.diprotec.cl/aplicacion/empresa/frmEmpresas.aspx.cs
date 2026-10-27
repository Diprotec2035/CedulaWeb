using cedulaweb.diprotec.cl.aplicacion.helpers;
using Modelo;
using Modelo.Comunes;
using Modelo.v1.Empresa;
using Negocio;
using Negocio.v1.Empresa;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace cedulaweb.diprotec.cl.aplicacion.empresa
{
    public partial class frmEmpresas : Page
    {
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

            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorPermisoAccesoModulo;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var contextoModulo = ModuloContextoHelper.ValidarModuloPagina(this);

            if (!contextoModulo.Exitoso)
            {
                var mensaje = string.IsNullOrWhiteSpace(contextoModulo.Mensaje) ? ConstantesModelo.ConsValidacionModuloContextoError : contextoModulo.Mensaje;

                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = mensaje;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                if (ListadoFiltroSesionHelper.DebeMantenerFiltros(this))
                {
                    ListadoFiltroSesionHelper.Restaurar(this, pnlFiltros);

                    var filtrosSesion = ListadoFiltroSesionHelper.Obtener(this);

                    if (filtrosSesion != null)
                    {
                        gvListado.PageIndex = filtrosSesion.PageIndex;
                    }
                }
                else
                {
                    ListadoFiltroSesionHelper.Limpiar(this);
                }

                LoadEmpresas();
            }
        }

        private void LoadEmpresas()
        {
            var script = string.Empty;
            sbyte estadoSeleccionado;
            sbyte? estado = null;

            if (sbyte.TryParse(ddlEstado.SelectedValue, out estadoSeleccionado) && (estadoSeleccionado == 0 || estadoSeleccionado == 1))
            {
                estado = estadoSeleccionado;
            }

            gvListado.EmptyDataText = ConstantesModelo.ConsModulosEmpresasTituloListadoVacio;

            var filtro = new EmpresaFiltroModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Coincidencia = txtCoincidencia.Text.Trim(),
                Estado = estado,
                Orden = Convert.ToInt16(ddlOrdenarPor.SelectedValue),
                Orientacion = Convert.ToInt16(ddlDireccion.SelectedValue)
            };

            var respuesta = EmpresaNegocio.GetEmpresas(filtro);

            if (respuesta.Exitoso)
            {
                gvListado.DataSource = respuesta.Datos;
                gvListado.DataBind();
            }
            else
            {
                var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
                var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
                var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

                script = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "LoadEmpresas", script, true);
        }

        protected void gvListado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListado.PageIndex = e.NewPageIndex;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadEmpresas();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvListado.PageIndex = 0;

            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            LoadEmpresas();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            ListadoFiltroSesionHelper.Guardar(this, pnlFiltros, gvListado.PageIndex);

            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlNuevo);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvListado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var id = gvListado.DataKeys[e.Row.RowIndex].Value.ToString();
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlEditar, "id=" + HttpUtility.UrlEncode(id));
            var urlJs = HttpUtility.JavaScriptStringEncode(url);

            e.Row.Attributes["ondblclick"] = "window.location='" + urlJs + "';";
            e.Row.Style["cursor"] = "pointer";

            var lnkEditar = e.Row.FindControl("lnkEditar") as HyperLink;

            if (lnkEditar != null)
            {
                lnkEditar.NavigateUrl = url;
            }
        }

        protected void gvListado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Bitacora")
            {
                return;
            }

            var rutEmpresa = Convert.ToString(e.CommandArgument).Trim();

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                var mensajeEmpresaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionEmpresaTokenEmpresaNoIdentificada);
                var scriptEmpresa = "mostrarAlerta('warning', '', '" + mensajeEmpresaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "BitacoraEmpresaTokenSinRegistro", scriptEmpresa, true);
                return;
            }

            ucBitacora.Cargar("SP_SEL_EMPRESA_TOKEN_BITACORA_EMPRESA", "P_EMP_RUT", rutEmpresa);

            var scriptAbrirBitacora = @"(function () {
                var abrir = function () {
                    abrirBitacora();
                };

                if (document.readyState === 'complete') {
                    abrir();
                }
                else {
                    window.addEventListener('load', abrir);
                }
            })();";

            ScriptManager.RegisterStartupScript(this, GetType(), "AbrirBitacoraEmpresaToken", scriptAbrirBitacora, true);
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

        protected void btnGenerarTokenOculto_Click(object sender, EventArgs e)
        {
            if (!ValidarSuperAdminToken())
            {
                return;
            }

            var rutEmpresa = Convert.ToString(hfTokenEmpresaRut.Value).Trim();

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionEmpresaTokenEmpresaNoIdentificada, "EmpresaTokenGenerarEmpresaInvalida");
                return;
            }

            var empresaToken = new EmpresaTokenModelo
            {
                RutEmpresa = rutEmpresa,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = EmpresaTokenNegocio.GenerarEmpresaToken(empresaToken);

            if (respuesta != null && respuesta.Exitoso)
            {
                LoadEmpresas();
            }

            MostrarResultadoToken(respuesta, ConstantesModelo.ConsErrorEmpresaTokenGenerar, "EmpresaTokenGenerar");
        }

        protected void btnRenovarTokenOculto_Click(object sender, EventArgs e)
        {
            if (!ValidarSuperAdminToken())
            {
                return;
            }

            var rutEmpresa = Convert.ToString(hfTokenEmpresaRut.Value).Trim();

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionEmpresaTokenEmpresaNoIdentificada, "EmpresaTokenRenovarEmpresaInvalida");
                return;
            }

            var empresaToken = new EmpresaTokenModelo
            {
                RutEmpresa = rutEmpresa,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = EmpresaTokenNegocio.RenovarEmpresaToken(empresaToken);

            if (respuesta != null && respuesta.Exitoso)
            {
                LoadEmpresas();
            }

            MostrarResultadoToken(respuesta, ConstantesModelo.ConsErrorEmpresaTokenRenovar, "EmpresaTokenRenovar");
        }

        protected void btnRevocarTokenOculto_Click(object sender, EventArgs e)
        {
            if (!ValidarSuperAdminToken())
            {
                return;
            }

            var rutEmpresa = Convert.ToString(hfTokenEmpresaRut.Value).Trim();

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionEmpresaTokenEmpresaNoIdentificada, "EmpresaTokenRevocarEmpresaInvalida");
                return;
            }

            uint tokenId;

            if (!uint.TryParse(hfTokenId.Value, out tokenId) || tokenId == 0)
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionEmpresaTokenNoIdentificado, "EmpresaTokenRevocarIdInvalido");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMotivoRevocacion.Text))
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorWarning, string.Empty, ConstantesModelo.ConsValidacionEmpresaTokenMotivoRevocacionVacio, "EmpresaTokenRevocarMotivoVacio");
                ReabrirModalRevocarToken(rutEmpresa, tokenId);
                return;
            }

            var empresaToken = new EmpresaTokenModelo
            {
                Id = tokenId,
                RutEmpresa = rutEmpresa,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                RevocationReason = txtMotivoRevocacion.Text.Trim()
            };

            var respuesta = EmpresaTokenNegocio.RevocarEmpresaToken(empresaToken);

            if (respuesta != null && respuesta.Exitoso)
            {
                hfTokenEmpresaRut.Value = string.Empty;
                hfTokenId.Value = string.Empty;
                txtMotivoRevocacion.Text = string.Empty;

                LoadEmpresas();
            }

            MostrarResultadoRevocacion(respuesta, rutEmpresa, tokenId);
        }

        protected string ObtenerClaseEstadoToken(object vigenciaCodigo)
        {
            var codigo = Convert.ToInt32(vigenciaCodigo);

            switch (codigo)
            {
                case 0:
                    return "text-success fs-18";

                case 1:
                    return "text-warning fs-18";

                case 2:
                    return "text-danger fs-18";

                case 3:
                    return "text-danger fs-18";

                case 4:
                    return "text-info fs-18";

                case 5:
                    return "text-warning fs-18";

                case 6:
                    return "text-muted fs-18";

                default:
                    return "text-muted fs-18";
            }
        }

        protected string ObtenerIconoEstadoToken(object vigenciaCodigo)
        {
            var codigo = Convert.ToInt32(vigenciaCodigo);

            switch (codigo)
            {
                case 0:
                    return "fe fe-check-circle";

                case 1:
                    return "fe fe-clock";

                case 2:
                    return "fe fe-x-circle";

                case 3:
                    return "fe fe-slash";

                case 4:
                    return "fe fe-alert-triangle";

                case 5:
                    return "fe fe-refresh-cw";

                case 6:
                    return "bi bi-key-fill";

                default:
                    return "fe fe-help-circle";
            }
        }

        protected bool PuedeGenerarToken(object estado, object tokenTotal)
        {
            return Convert.ToInt32(estado) == 0 && Convert.ToInt32(tokenTotal) == 0;
        }

        protected bool PuedeRenovarToken(object estado, object tokenTotal, object tokenTotalOperativos)
        {
            return Convert.ToInt32(estado) == 0 && Convert.ToInt32(tokenTotal) > 0 && Convert.ToInt32(tokenTotalOperativos) <= 1;
        }

        protected bool PuedeRevocarToken(object tokenId, object tokenTotalOperativos)
        {
            if (tokenId == null || tokenId == DBNull.Value)
            {
                return false;
            }

            return Convert.ToUInt32(tokenId) > 0 && Convert.ToInt32(tokenTotalOperativos) > 0;
        }

        protected bool PuedeVerBitacoraToken(object tokenTotal)
        {
            if (tokenTotal == null || tokenTotal == DBNull.Value)
            {
                return false;
            }

            return Convert.ToInt32(tokenTotal) > 0;
        }

        private bool ValidarSuperAdminToken()
        {
            if (Convert.ToString(Session["superAdmin"]) == "1")
            {
                return true;
            }

            MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsErrorPermisoModificarRegistro, "EmpresaTokenSinPermiso");

            return false;
        }

        private void MostrarResultadoToken(RespuestaModelo<EmpresaTokenGeneradoModelo> respuesta, string mensajeError, string clave)
        {
            if (respuesta == null)
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, mensajeError, clave + "SinRespuesta");
                return;
            }

            var color = string.IsNullOrWhiteSpace(respuesta.AlertaColor) ? (respuesta.Exitoso ? ConstantesModelo.ConsAlertaColorSuccess : ConstantesModelo.ConsAlertaColorDanger) : respuesta.AlertaColor;
            var titulo = respuesta.AlertaTitulo ?? string.Empty;
            var mensaje = string.IsNullOrWhiteSpace(respuesta.Mensaje) ? mensajeError : respuesta.Mensaje;
            var colorJs = HttpUtility.JavaScriptStringEncode(color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(titulo);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);
            var script = "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');";

            if (respuesta.Exitoso && respuesta.Datos != null && !string.IsNullOrWhiteSpace(respuesta.Datos.Token))
            {
                var tokenJs = HttpUtility.JavaScriptStringEncode(respuesta.Datos.Token);
                var fechaVencimientoJs = HttpUtility.JavaScriptStringEncode(respuesta.Datos.ExpiresAt.ToString("dd-MM-yyyy HH:mm:ss"));

                script += "mostrarModalTokenGenerado('" + tokenJs + "', '" + fechaVencimientoJs + "');";
            }
            else if (respuesta.Exitoso)
            {
                script = "mostrarAlerta('danger', '', '" + HttpUtility.JavaScriptStringEncode(mensajeError) + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), clave + "_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private void MostrarResultadoRevocacion(RespuestaModelo<EmpresaTokenModelo> respuesta, string rutEmpresa, uint tokenId)
        {
            if (respuesta == null)
            {
                MostrarAlertaToken(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsErrorEmpresaTokenRevocar, "EmpresaTokenRevocarSinRespuesta");
                ReabrirModalRevocarToken(rutEmpresa, tokenId);
                return;
            }

            var color = string.IsNullOrWhiteSpace(respuesta.AlertaColor) ? (respuesta.Exitoso ? ConstantesModelo.ConsAlertaColorSuccess : ConstantesModelo.ConsAlertaColorDanger) : respuesta.AlertaColor;
            var titulo = respuesta.AlertaTitulo ?? string.Empty;
            var mensaje = string.IsNullOrWhiteSpace(respuesta.Mensaje) ? ConstantesModelo.ConsErrorEmpresaTokenRevocar : respuesta.Mensaje;

            MostrarAlertaToken(color, titulo, mensaje, "EmpresaTokenRevocarResultado");

            if (!respuesta.Exitoso)
            {
                ReabrirModalRevocarToken(rutEmpresa, tokenId);
            }
        }

        private void ReabrirModalRevocarToken(string rutEmpresa, uint tokenId)
        {
            var rutEmpresaJs = HttpUtility.JavaScriptStringEncode(rutEmpresa);
            var script = "abrirModalRevocarToken('" + rutEmpresaJs + "', '" + tokenId + "', false);";

            ScriptManager.RegisterStartupScript(this, GetType(), "EmpresaTokenReabrirRevocacion_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private void MostrarAlertaToken(string color, string titulo, string mensaje, string clave)
        {
            var colorJs = HttpUtility.JavaScriptStringEncode(color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(titulo);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);
            var script = "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), clave + "_" + Guid.NewGuid().ToString("N"), script, true);
        }
    }
}