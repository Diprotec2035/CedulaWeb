using Modelo;
using Modelo.v1.Modulo;
using Negocio;
using Negocio.v1.BaseDatos;
using Negocio.v1.Categoria;
using Negocio.v1.Modulo;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.modulo
{
    public partial class frmModulosNuevo : Page
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

            tglAdministrador.Attributes["data-hidden-id"] = hfAdministrador.ClientID;
            tglMenuVisible.Attributes["data-hidden-id"] = hfMenuVisible.ClientID;
            tglBitacoraEstado.Attributes["data-hidden-id"] = hfBitacoraEstado.ClientID;

            if (!IsPostBack)
            {
                CargarCategorias();
                CargarOrdenes();
                CargarTablasBitacora();

                InicializarDdlBitacoraSp();
                InicializarDdlBitacoraParametro();

                hfBitacoraEstado.Value = "1";
                hfAdministrador.Value = "1";
                hfMenuVisible.Value = "0";

                divBitacoraDetalle.Visible = true;

                tglBitacoraEstado.Attributes["class"] = "main-toggle br-3 text-center";
                tglAdministrador.Attributes["class"] = "main-toggle br-3 text-center";
                tglMenuVisible.Attributes["class"] = "main-toggle br-3 text-center";
            }
        }

        private void CargarCategorias()
        {
            var operacion = 0;

            var categorias = CategoriaNegocio.GetCategoriasModulosPerfil(ref operacion);

            ddlCategoria.Items.Clear();

            if (operacion == 0)
            {
                ddlCategoria.DataSource = categorias;
                ddlCategoria.DataValueField = "Id";
                ddlCategoria.DataTextField = "Nombre";
                ddlCategoria.DataBind();
            }

            ddlCategoria.Items.Insert(0, new ListItem("Seleccione", "0"));
        }

        private void CargarOrdenes()
        {
            ddlOrden.Items.Clear();

            for (var i = 1; i <= 99; i++)
            {
                ddlOrden.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        private void CargarTablasBitacora()
        {
            ddlBitacoraTabla.Items.Clear();
            ddlBitacoraTabla.Items.Add(new ListItem("Seleccione", string.Empty));

            var respuesta = BaseDatosNegocio.GetTablas();

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var tabla in respuesta.Datos)
            {
                ddlBitacoraTabla.Items.Add(new ListItem(tabla.Nombre, tabla.Nombre));
            }
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

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarModulo", scriptSinPermiso, true);
                return;
            }

            sbyte administrador;
            sbyte menuVisible;

            if (!sbyte.TryParse(hfAdministrador.Value, out administrador) || (administrador != 0 && administrador != 1))
            {
                var mensajeAdministradorInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionModuloAdministradorInvalido);

                var scriptAdministradorInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeAdministradorInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "AdministradorModuloInvalido", scriptAdministradorInvalido, true);
                return;
            }

            if (!sbyte.TryParse(hfMenuVisible.Value, out menuVisible) || (menuVisible != 0 && menuVisible != 1))
            {
                var mensajeMenuVisibleInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionModuloMenuVisibleInvalido);

                var scriptMenuVisibleInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeMenuVisibleInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "MenuVisibleModuloInvalido", scriptMenuVisibleInvalido, true);
                return;
            }

            sbyte bitacoraEstado;

            var modulo = new ModuloModelo
            {
                CategoriaId = Convert.ToUInt32(ddlCategoria.SelectedValue),
                Nombre = txtNombre.Text.Trim(),
                TituloPagina = txtTituloPagina.Text.Trim(),
                TituloContenido = txtTituloContenido.Text.Trim(),
                Orden = (sbyte?)Convert.ToInt32(ddlOrden.SelectedValue),
                Url = txtUrl.Text.Trim(),
                UrlPublica = txtUrlPublica.Text.Trim(),
                Estado = 0,
                Administrador = administrador,
                MenuVisible = menuVisible,
                BitacoraEstado = sbyte.TryParse(hfBitacoraEstado.Value, out bitacoraEstado) ? bitacoraEstado : (sbyte)1,
                BitacoraSp = ddlBitacoraSp.SelectedValue,
                BitacoraParametro = string.IsNullOrWhiteSpace(ddlBitacoraParametro.SelectedValue)
                    ? string.Empty
                    : "P_" + ddlBitacoraParametro.SelectedValue,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = ModuloNegocio.RegistrarModulo(modulo);

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

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void ddlBitacoraTabla_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarProcedimientosTabla(ddlBitacoraTabla.SelectedValue);
            CargarColumnasTabla(ddlBitacoraTabla.SelectedValue);

            divBitacoraDetalle.Visible = true;
        }

        private void CargarProcedimientosTabla(string tablaNombre)
        {
            InicializarDdlBitacoraSp();

            if (string.IsNullOrWhiteSpace(tablaNombre))
            {
                return;
            }

            var respuesta = BaseDatosNegocio.GetProcedimientosTabla(tablaNombre);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var procedimiento in respuesta.Datos)
            {
                ddlBitacoraSp.Items.Add(new ListItem(procedimiento.Nombre, procedimiento.Nombre));
            }
        }

        private void CargarColumnasTabla(string tablaNombre)
        {
            InicializarDdlBitacoraParametro();

            if (string.IsNullOrWhiteSpace(tablaNombre))
            {
                return;
            }

            var respuesta = BaseDatosNegocio.GetColumnasTabla(tablaNombre);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var columna in respuesta.Datos)
            {
                ddlBitacoraParametro.Items.Add(new ListItem(columna.Nombre, columna.Nombre));
            }
        }

        private void InicializarDdlBitacoraSp()
        {
            ddlBitacoraSp.Items.Clear();
            ddlBitacoraSp.Items.Add(new ListItem("Seleccione", string.Empty));
        }

        private void InicializarDdlBitacoraParametro()
        {
            ddlBitacoraParametro.Items.Clear();
            ddlBitacoraParametro.Items.Add(new ListItem("Seleccione", string.Empty));
        }
    }
}