using Modelo;
using Modelo.v1.VersionAplicacion;
using Negocio;
using Negocio.v1.VersionAplicacion;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.version_aplicacion
{
    public partial class frmVersionesAplicacionNueva : Page
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

            Page.Form.Action = ResolveUrl("~/aplicacion/version_aplicacion/frmVersionesAplicacionNueva.aspx");

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
                CargarListasVersion();
                CargarAndroidVersion();
                InicializarToggles();
            }
        }

        private void CargarListasVersion()
        {
            CargarListaNumerica(ddlVersionMayor);
            CargarListaNumerica(ddlVersionMenor);
            CargarListaNumerica(ddlVersionRevision);
        }

        private void CargarAndroidVersion()
        {
            CargarListaNumerica(ddlAndroidVersion);
        }

        private void CargarListaNumerica(DropDownList ddl)
        {
            ddl.Items.Clear();

            for (var i = 0; i <= 99; i++)
            {
                ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        private void InicializarToggles()
        {
            hfForceUpdate.Value = "1";
            hfIsActive.Value = "1";
            hfIsPublished.Value = "1";

            tglForceUpdate.Attributes["class"] = "main-toggle br-3 text-center";
            tglIsActive.Attributes["class"] = "main-toggle br-3 text-center";
            tglIsPublished.Attributes["class"] = "main-toggle br-3 text-center";
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["superAdmin"]) != "1")
            {
                var mensajeSinPermisoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeSinPermisoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarVersionAplicacion", scriptSinPermiso, true);
                return;
            }

            if (!sbyte.TryParse(hfForceUpdate.Value, out sbyte forceUpdate) ||
                (forceUpdate != 0 && forceUpdate != 1))
            {
                var mensajeForceUpdateInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionVersionAplicacionForceUpdateInvalido);

                var scriptForceUpdateInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeForceUpdateInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ForceUpdateVersionAplicacionInvalido", scriptForceUpdateInvalido, true);
                return;
            }

            if (!sbyte.TryParse(hfIsActive.Value, out sbyte isActive) ||
                (isActive != 0 && isActive != 1))
            {
                var mensajeEstadoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionVersionAplicacionEstadoInvalido);

                var scriptEstadoInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoVersionAplicacionInvalida", scriptEstadoInvalido, true);
                return;
            }

            if (!sbyte.TryParse(hfIsPublished.Value, out sbyte isPublished) ||
                (isPublished != 0 && isPublished != 1))
            {
                var mensajePublicacionInvalidaJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionVersionAplicacionPublicacionInvalida);

                var scriptPublicacionInvalida =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajePublicacionInvalidaJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "PublicacionVersionAplicacionInvalida", scriptPublicacionInvalida, true);
                return;
            }

            var rutaTemporalApk = string.Empty;

            if (fuApk.HasFile)
            {
                rutaTemporalApk = GuardarApkTemporal();
            }

            var version = new VersionAplicacionModelo
            {
                Id = 0,

                VersionCode = 0,
                VersionName = ddlVersionMayor.SelectedValue + "." +
                              ddlVersionMenor.SelectedValue + "." +
                              ddlVersionRevision.SelectedValue,
                MinSupportedVersionCode = 0,

                ForceUpdate = forceUpdate,
                IsActive = isActive,
                Ispublished = isPublished,

                Fabricante = txtFabricante.Text.Trim(),
                FabricanteApk = txtFabricanteApk.Text.Trim(),

                AndroidVersion = Convert.ToInt32(ddlAndroidVersion.SelectedValue),

                Modelo = txtModelo.Text.Trim(),
                ModeloApk = txtModeloApk.Text.Trim(),
                ModeloKey = txtModelo.Text.Trim(),

                ApkFileName = fuApk.HasFile ? Path.GetFileName(fuApk.FileName) : string.Empty,
                ApkRelativePath = string.Empty,
                ApkSha256 = string.Empty,
                FileSizeBytes = 0,

                ReleaseNotes = txtReleaseNotes.Text.Trim(),
                CreatedAt = string.Empty,
                CreatedBy = Convert.ToString(HttpContext.Current.Session["cliente"]),
                PublishedAt = string.Empty,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = VersionAplicacionNegocio.RegistrarVersionAplicacion(version, rutaTemporalApk);

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
                    "$('#" + btnRegistrar.ClientID + "').val('Registrado').prop('disabled', true);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', true);";
            }
            else
            {
                script +=
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);";
            }

            script += mensajeAlerta;

            ScriptManager.RegisterStartupScript(this, GetType(), "btnRegistrar_Click", script, true);
        }

        private string GuardarApkTemporal()
        {
            var rutaWebTemporal = "~/" +
                                  ConstantesModelo.ConsDownloadPath.TrimStart('/', '~') +
                                  ConstantesModelo.ConsDownloadPathTemp.TrimStart('/', '~');

            var carpetaTemporal = Server.MapPath(rutaWebTemporal);

            if (!Directory.Exists(carpetaTemporal))
            {
                Directory.CreateDirectory(carpetaTemporal);
            }

            var nombreTemporal = Guid.NewGuid().ToString("N") + Path.GetExtension(fuApk.FileName);
            var rutaTemporal = Path.Combine(carpetaTemporal, nombreTemporal);

            fuApk.SaveAs(rutaTemporal);

            return rutaTemporal;
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}