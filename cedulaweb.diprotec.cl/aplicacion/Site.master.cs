using Modelo;
using Modelo.Comunes;
using Modelo.v1.Empresa;
using Negocio;
using Negocio.Comunes;
using Negocio.v1.Empresa;
using Negocio.v1.Modulo;
using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace cedulaweb.diprotec.cl.aplicacion
{
    public partial class Site : MasterPage
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

            RenovarSesionAplicacion();
            ForzarActionUrlPublica();

            lblUsuarioNombre.Text = Convert.ToString(Session["clienteNombre"]);
            lblUsuarioPerfil.Text = Convert.ToString(Session["perfil"]);

            CargarEmpresaCabecera();

            if (!IsPostBack)
            {
                LoadMenu();
                CargarInformacionApp();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            CargarTokenApiCabecera();

            base.OnPreRender(e);

            RegistrarSincronizacionSesion();
            MostrarAlertaPendiente();
        }

        protected bool MostrarOpcionMisDatos()
        {
            var rutUsuario = Convert.ToString(Session["cliente"]);
            var esSuperAdmin = Convert.ToString(Session["superAdmin"]) == "1";

            if (string.IsNullOrWhiteSpace(rutUsuario))
            {
                return false;
            }

            return !esSuperAdmin;
        }

        protected string GetUrlMisDatos()
        {
            var rutUsuario = Convert.ToString(Session["cliente"]);

            if (string.IsNullOrWhiteSpace(rutUsuario))
            {
                return "javascript:void(0);";
            }

            var respuesta = ModuloNegocio.GetModuloNavegacion("/aplicacion/usuario/frmUsuariosEditar.aspx", string.Empty);

            if (!respuesta.Exitoso || respuesta.Datos == null || respuesta.Datos.Id == 0)
            {
                return "javascript:void(0);";
            }

            var urlBase = !string.IsNullOrWhiteSpace(respuesta.Datos.UrlPublica)
                ? ConstantesModelo.ConsModuloUrl + respuesta.Datos.UrlPublica.Trim()
                : respuesta.Datos.Url;

            if (string.IsNullOrWhiteSpace(urlBase))
            {
                return "javascript:void(0);";
            }

            return urlBase + "?id=" + HttpUtility.UrlEncode(rutUsuario) + "&origen=perfil&mod_id=" + HttpUtility.UrlEncode(Convert.ToString(respuesta.Datos.Id));
        }

        private void RenovarSesionAplicacion()
        {
            var minutosSesion = FuncionesNegocio.GetSessionTimeoutMinutos();
            var fechaRenovacion = DateTime.Now;

            HttpContext.Current.Session.Timeout = minutosSesion;
            HttpContext.Current.Session["SesionRenovada"] = fechaRenovacion;
            HttpContext.Current.Session["SesionExpira"] = fechaRenovacion.AddMinutes(minutosSesion);

            if (HttpContext.Current.Session["SesionSincronizacionId"] == null)
            {
                HttpContext.Current.Session["SesionSincronizacionId"] = Guid.NewGuid().ToString("N");
            }
        }

        private void RegistrarSincronizacionSesion()
        {
            var sesionExpiraMs = GetSesionExpiraUnixMilliseconds();
            var sesionRenovadaMs = GetSesionRenovadaUnixMilliseconds();
            var script = "window.sesionExpiraServidorMs = " + sesionExpiraMs + "; window.sesionRenovadaServidorMs = " + sesionRenovadaMs + "; if (window.actualizarExpiracionSesion) { window.actualizarExpiracionSesion(window.sesionExpiraServidorMs, window.sesionRenovadaServidorMs); }";

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SincronizarExpiracionSesion", script, true);
        }

        private void ForzarActionUrlPublica()
        {
            var urlPublicaActual = Convert.ToString(Context.Items["UrlPublicaActual"]);

            if (string.IsNullOrWhiteSpace(urlPublicaActual))
            {
                return;
            }

            frmMaster.Action = ResolveUrl(urlPublicaActual);
        }

        private void CargarEmpresaCabecera()
        {
            var empresaNombre = Convert.ToString(Session["empresaNombre"]);

            if (string.IsNullOrWhiteSpace(empresaNombre))
            {
                empresaNombre = Convert.ToString(Session["empresa"]);
            }

            lblEmpresaNombre.Text = empresaNombre;
        }

        private void CargarTokenApiCabecera()
        {
            liTokenApiCabecera.Visible = false;
            lblTokenApiCabecera.Text = string.Empty;
            spnTokenApiCabecera.Attributes["title"] = string.Empty;

            var rutEmpresa = Convert.ToString(Session["empresa"]);

            if (string.IsNullOrWhiteSpace(rutEmpresa))
            {
                return;
            }

            var operacion = 0;
            var token = EmpresaTokenNegocio.GetEmpresaTokenCabecera(ref operacion, rutEmpresa);

            if (operacion != 0 || token == null)
            {
                return;
            }

            liTokenApiCabecera.Visible = true;

            AplicarEstadoTokenApiCabecera(token);
        }

        private void AplicarEstadoTokenApiCabecera(EmpresaTokenCabeceraModelo token)
        {
            switch (token.TokenVigenciaCodigo)
            {
                case 0:
                    AplicarTokenApiVigente(token);
                    break;

                case 1:
                    AplicarTokenApiProximoVencer(token);
                    break;

                case 2:
                    AplicarTokenApiVencido(token);
                    break;

                case 3:
                    AplicarTokenApiRevocado(token);
                    break;

                case 4:
                    AplicarTokenApiSinFechaVencimiento(token);
                    break;

                case 5:
                    AplicarTokenApiRenovacion(token);
                    break;

                default:
                    AplicarTokenApiNoDisponible(token);
                    break;
            }
        }

        private void AplicarTokenApiVigente(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-success-transparent text-success tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre + " · " + ObtenerTextoDias(token.TokenDiasRestantes);

            spnTokenApiCabecera.Attributes["title"] = token.ExpiresAt.HasValue
                ? token.TokenVigenciaNombre + " hasta el " + token.ExpiresAt.Value.ToString("dd-MM-yyyy HH:mm") + "."
                : token.TokenVigenciaNombre + ".";
        }

        private void AplicarTokenApiProximoVencer(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-warning-transparent text-warning tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre + " · " + ObtenerTextoDias(token.TokenDiasRestantes);

            spnTokenApiCabecera.Attributes["title"] = token.ExpiresAt.HasValue
                ? token.TokenVigenciaNombre + ". Vencerá el " + token.ExpiresAt.Value.ToString("dd-MM-yyyy HH:mm") + ". Contacte a DIPROTEC para gestionar su renovación."
                : token.TokenVigenciaNombre + ". Contacte a DIPROTEC para gestionar su renovación.";
        }

        private void AplicarTokenApiVencido(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-danger-transparent text-danger tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre;

            spnTokenApiCabecera.Attributes["title"] = token.ExpiresAt.HasValue
                ? token.TokenVigenciaNombre + ". Venció el " + token.ExpiresAt.Value.ToString("dd-MM-yyyy HH:mm") + ". Contacte a DIPROTEC."
                : token.TokenVigenciaNombre + ". Contacte a DIPROTEC.";
        }

        private void AplicarTokenApiRevocado(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-danger-transparent text-danger tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre;

            spnTokenApiCabecera.Attributes["title"] = token.TokenVigenciaNombre + ". Contacte a DIPROTEC.";
        }

        private void AplicarTokenApiSinFechaVencimiento(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-warning-transparent text-warning tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre;

            spnTokenApiCabecera.Attributes["title"] = token.TokenVigenciaNombre + ". Contacte a DIPROTEC.";
        }

        private void AplicarTokenApiRenovacion(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-info-transparent text-info tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre + " · " + ObtenerTextoDias(token.TokenDiasRestantes);

            var detalle = token.ExpiresAt.HasValue
                ? token.TokenVigenciaNombre + ". El token nuevo está vigente hasta el " + token.ExpiresAt.Value.ToString("dd-MM-yyyy HH:mm") + "."
                : token.TokenVigenciaNombre + ".";

            if (token.TokenAnteriorExpiresAt.HasValue)
            {
                detalle += " El token anterior permanecerá operativo hasta el " + token.TokenAnteriorExpiresAt.Value.ToString("dd-MM-yyyy HH:mm") + ".";
            }

            spnTokenApiCabecera.Attributes["title"] = detalle;
        }

        private void AplicarTokenApiNoDisponible(EmpresaTokenCabeceraModelo token)
        {
            spnTokenApiCabecera.Attributes["class"] = "badge bg-danger-transparent text-danger tx-12 fw-semibold";

            lblTokenApiCabecera.Text = token.TokenVigenciaNombre;

            spnTokenApiCabecera.Attributes["title"] = token.TokenVigenciaNombre + ". Contacte a DIPROTEC.";
        }

        private string ObtenerTextoDias(int? diasRestantes)
        {
            if (!diasRestantes.HasValue)
            {
                return "vigente";
            }

            if (diasRestantes.Value <= 0)
            {
                return "menos de 1 día";
            }

            if (diasRestantes.Value == 1)
            {
                return "1 día";
            }

            return diasRestantes.Value + " días";
        }

        private void LoadMenu()
        {
            var usuario = new EmpresaUsuarioModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"]),
                EsSuperAdmin = Convert.ToString(HttpContext.Current.Session["superAdmin"]) == "1"
            };

            var menu = MenuNegocio.GetMenu(usuario);

            rptMenu.DataSource = menu;
            rptMenu.DataBind();
        }

        protected void rptMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var categoria = (CategoriaViewModel)e.Item.DataItem;
                var rptSubMenu = (Repeater)e.Item.FindControl("rptSubMenu");

                if (rptSubMenu != null)
                {
                    rptSubMenu.DataSource = categoria.Modulos;
                    rptSubMenu.DataBind();
                }
            }
        }

        public long GetSesionExpiraUnixMilliseconds()
        {
            if (Session["SesionExpira"] == null)
            {
                return 0;
            }

            var sesionExpira = (DateTime)Session["SesionExpira"];

            return new DateTimeOffset(sesionExpira).ToUnixTimeMilliseconds();
        }

        public long GetSesionRenovadaUnixMilliseconds()
        {
            if (Session["SesionRenovada"] == null)
            {
                return 0;
            }

            var sesionRenovada = (DateTime)Session["SesionRenovada"];

            return new DateTimeOffset(sesionRenovada).ToUnixTimeMilliseconds();
        }

        public string GetSesionSincronizacionClave()
        {
            var sesionSincronizacionId = Convert.ToString(Session["SesionSincronizacionId"]);

            return string.IsNullOrWhiteSpace(sesionSincronizacionId) ? string.Empty : "CedulaWeb.SesionExpira." + sesionSincronizacionId;
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();

            Response.Redirect(ConstantesModelo.ConsHttpInicio, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void MostrarAlertaPendiente()
        {
            var rutaActual = Convert.ToString(Request.Url.AbsolutePath);

            if (!rutaActual.EndsWith("/aplicacion/default.aspx", StringComparison.OrdinalIgnoreCase) &&
                !rutaActual.EndsWith("/aplicacion/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (HttpContext.Current.Items["MostrarAlertaPendienteProcesada"] != null)
            {
                return;
            }

            HttpContext.Current.Items["MostrarAlertaPendienteProcesada"] = true;

            litAlertaPendiente.Text = string.Empty;

            var mensaje = Convert.ToString(HttpContext.Current.Session["AlertaMensaje"]);

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return;
            }

            var color = Convert.ToString(HttpContext.Current.Session["AlertaColor"]);
            var titulo = Convert.ToString(HttpContext.Current.Session["AlertaTitulo"]);

            HttpContext.Current.Session.Remove("AlertaColor");
            HttpContext.Current.Session.Remove("AlertaTitulo");
            HttpContext.Current.Session.Remove("AlertaMensaje");

            if (string.IsNullOrWhiteSpace(color))
            {
                color = ConstantesModelo.ConsAlertaColorWarning;
            }

            if (titulo == null)
            {
                titulo = string.Empty;
            }

            var colorJs = HttpUtility.JavaScriptStringEncode(color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlDecode(titulo));
            var mensajeJs = HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlDecode(mensaje));

            litAlertaPendiente.Text =
                "<script type=\"text/javascript\">" +
                "$(window).on('load', function () {" +
                "setTimeout(function () {" +
                "$('#global-loader').hide();" +
                "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');" +
                "}, 0);" +
                "});" +
                "</script>";
        }

        private void CargarInformacionApp()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var titulo = Attribute.GetCustomAttribute(
                assembly,
                typeof(AssemblyTitleAttribute)
            ) as AssemblyTitleAttribute;

            var compania = Attribute.GetCustomAttribute(
                assembly,
                typeof(AssemblyCompanyAttribute)
            ) as AssemblyCompanyAttribute;

            var copyright = Attribute.GetCustomAttribute(
                assembly,
                typeof(AssemblyCopyrightAttribute)
            ) as AssemblyCopyrightAttribute;

            var version = assembly.GetName().Version;

            lblAppTitulo.Text = titulo != null && !string.IsNullOrWhiteSpace(titulo.Title)
                ? titulo.Title
                : "No disponible";

            lblAppCompania.Text = compania != null && !string.IsNullOrWhiteSpace(compania.Company)
                ? compania.Company
                : "No disponible";

            lblAppCopyright.Text = copyright != null && !string.IsNullOrWhiteSpace(copyright.Copyright)
                ? copyright.Copyright
                : "No disponible";

            lblAppVersion.Text = version != null
                ? version.ToString()
                : "No disponible";
        }

        protected string GetUrlModuloMenu(object dataItem)
        {
            if (dataItem == null)
            {
                return "javascript:void(0);";
            }

            var modId = Convert.ToString(DataBinder.Eval(dataItem, "Id"));
            var urlPublica = Convert.ToString(DataBinder.Eval(dataItem, "UrlPublica"));
            var urlReal = Convert.ToString(DataBinder.Eval(dataItem, "Url"));

            var url = !string.IsNullOrWhiteSpace(urlPublica)
                ? ConstantesModelo.ConsModuloUrl + urlPublica.Trim()
                : urlReal;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(modId))
            {
                return "javascript:void(0);";
            }

            var separador = url.Contains("?") ? "&" : "?";

            return url + separador + "mod_id=" + HttpUtility.UrlEncode(modId);
        }
    }
}