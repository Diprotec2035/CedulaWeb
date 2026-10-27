using cedulaweb.diprotec.cl.aplicacion.helpers;
using Modelo;
using Modelo.v1.VersionAplicacion;
using Negocio;
using Negocio.v1.VersionAplicacion;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.version_aplicacion
{
    public partial class frmVersionesAplicacionEditar : Page
    {
        private DateTime UpdatedAtOriginal
        {
            get
            {
                return ViewState["UpdatedAtOriginal"] == null ? DateTime.MinValue : (DateTime)ViewState["UpdatedAtOriginal"];
            }
            set
            {
                ViewState["UpdatedAtOriginal"] = value;
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
                var mensaje = string.IsNullOrWhiteSpace(contextoModulo.Mensaje)
                    ? ConstantesModelo.ConsValidacionModuloContextoError
                    : contextoModulo.Mensaje;

                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorWarning;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = mensaje;

                Response.Redirect(ConstantesModelo.ConsModulosDefault, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            tglForceUpdate.Attributes["data-hidden-id"] = hfForceUpdate.ClientID;
            tglIsActive.Attributes["data-hidden-id"] = hfIsActive.ClientID;
            tglIsPublished.Attributes["data-hidden-id"] = hfIsPublished.ClientID;

            if (!IsPostBack)
            {
                var id = Convert.ToString(Request.QueryString["id"]);
                uint versionAplicacionId;

                if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out versionAplicacionId))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadVersionAplicacion(versionAplicacionId);
                ucBitacora.Cargar(versionAplicacionId.ToString());
            }
        }

        private void LoadVersionAplicacion(uint versionAplicacionId)
        {
            var operacion = 0;

            var versionAplicacionFiltro = new VersionAplicacionModelo
            {
                Id = versionAplicacionId
            };

            var version = VersionAplicacionNegocio.GetVersionAplicacion(ref operacion, versionAplicacionFiltro);

            if (operacion != 0 || version == null || version.Id == 0)
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtFabricante.Text = version.Fabricante;
            txtModelo.Text = version.Modelo;

            txtFabricanteApk.Text = ObtenerFabricanteApk(version);
            txtModeloApk.Text = ObtenerModeloApk(version);

            txtAndroidVersion.Text = version.AndroidVersion.ToString();
            txtVersionName.Text = version.VersionName;

            lnkApk.Text = version.ApkFileName;
            lnkApk.NavigateUrl = Convert.ToString(version.ApkRelativePath).TrimEnd('/') + "/" + Convert.ToString(version.ApkFileName);

            hfForceUpdate.Value = version.ForceUpdate.ToString();

            tglForceUpdate.Attributes["class"] = version.ForceUpdate == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            hfIsActive.Value = version.IsActive.ToString();

            tglIsActive.Attributes["class"] = version.IsActive == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            hfIsPublished.Value = version.Ispublished.ToString();

            tglIsPublished.Attributes["class"] = version.Ispublished == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            txtReleaseNotes.Text = version.ReleaseNotes;
            UpdatedAtOriginal = version.UpdatedAt;
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                var mensajeSinPermisoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeSinPermisoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarVersionAplicacion", scriptSinPermiso, true);
                return;
            }

            var id = Convert.ToString(Request.QueryString["id"]);
            uint versionAplicacionId;

            if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out versionAplicacionId))
            {
                var mensajeIdInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptIdInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeIdInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "VersionAplicacionIdInvalido", scriptIdInvalido, true);
                return;
            }

            sbyte forceUpdate;

            if (!sbyte.TryParse(hfForceUpdate.Value, out forceUpdate) ||
                (forceUpdate != 0 && forceUpdate != 1))
            {
                var mensajeForceUpdateInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionVersionAplicacionForceUpdateInvalido);

                var scriptForceUpdateInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeForceUpdateInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ForceUpdateVersionAplicacionInvalido", scriptForceUpdateInvalido, true);
                return;
            }

            sbyte isActive;

            if (!sbyte.TryParse(hfIsActive.Value, out isActive) ||
                (isActive != 0 && isActive != 1))
            {
                var mensajeEstadoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionVersionAplicacionEstadoInvalido);

                var scriptEstadoInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoVersionAplicacionInvalida", scriptEstadoInvalido, true);
                return;
            }

            sbyte isPublished;

            if (!sbyte.TryParse(hfIsPublished.Value, out isPublished) ||
                (isPublished != 0 && isPublished != 1))
            {
                var mensajePublicacionInvalidaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionVersionAplicacionPublicacionInvalida);

                var scriptPublicacionInvalida =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajePublicacionInvalidaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "PublicacionVersionAplicacionInvalida", scriptPublicacionInvalida, true);
                return;
            }

            var version = new VersionAplicacionModelo
            {
                Id = versionAplicacionId,
                ForceUpdate = forceUpdate,
                IsActive = isActive,
                Ispublished = isPublished,
                ReleaseNotes = txtReleaseNotes.Text.Trim(),
                UpdatedAt = UpdatedAtOriginal,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = VersionAplicacionNegocio.ActualizarVersionAplicacion(version);

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            var mensajeAlerta = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";

            if (respuesta.Exitoso)
            {
                var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
                var urlVolverJs = HttpUtility.JavaScriptStringEncode(urlVolver);

                mensajeAlerta += "setTimeout(function(){ window.location = '" + urlVolverJs + "'; }, 1200);";
            }

            var script = "$('#global-loader').hide();";

            if (respuesta.Exitoso)
            {
                script +=
                    "$('#" + btnActualizar.ClientID + "').val('Actualizado').prop('disabled', true);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', true);";
            }
            else
            {
                script +=
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);";
            }

            script += mensajeAlerta;

            ScriptManager.RegisterStartupScript(this, GetType(), "btnActualizar_Click", script, true);
        }

        private string ObtenerFabricanteApk(VersionAplicacionModelo version)
        {
            if (version == null || string.IsNullOrWhiteSpace(version.ApkFileName))
            {
                return string.Empty;
            }

            var separador = "_CEDULA_";
            var indice = version.ApkFileName.IndexOf(separador, StringComparison.OrdinalIgnoreCase);

            if (indice <= 0)
            {
                return version.Fabricante;
            }

            var bloque = version.ApkFileName.Substring(0, indice);
            var partes = bloque.Split('_');

            if (partes.Length < 2)
            {
                return version.Fabricante;
            }

            return partes[0];
        }

        private string ObtenerModeloApk(VersionAplicacionModelo version)
        {
            if (version == null || string.IsNullOrWhiteSpace(version.ApkFileName))
            {
                return string.Empty;
            }

            var separador = "_CEDULA_";
            var indice = version.ApkFileName.IndexOf(separador, StringComparison.OrdinalIgnoreCase);

            if (indice <= 0)
            {
                return version.Modelo;
            }

            var bloque = version.ApkFileName.Substring(0, indice);
            var partes = bloque.Split('_');

            if (partes.Length < 2)
            {
                return version.Modelo;
            }

            return partes[1];
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}