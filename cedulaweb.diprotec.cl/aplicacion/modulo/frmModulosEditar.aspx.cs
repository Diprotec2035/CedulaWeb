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
    public partial class frmModulosEditar : Page
    {
        private DateTime UpdatedAtOriginal
        {
            get
            {
                return ViewState["UpdatedAtOriginal"] == null
                    ? DateTime.MinValue
                    : (DateTime)ViewState["UpdatedAtOriginal"];
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

            tglAdministrador.Attributes["data-hidden-id"] = hfAdministrador.ClientID;
            tglMenuVisible.Attributes["data-hidden-id"] = hfMenuVisible.ClientID;
            tglBitacoraEstado.Attributes["data-hidden-id"] = hfBitacoraEstado.ClientID;
            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;
            tglNavegacionEstado.Attributes["data-hidden-id"] = hfNavegacionEstado.ClientID;

            divBitacoraDetalle.Visible = true;

            if (!IsPostBack)
            {
                CargarCategorias();
                CargarOrdenes();
                CargarTablasBitacora();
                CargarModulosNavegacionDestino();
                InicializarDdlBitacoraSp();
                InicializarDdlBitacoraParametro();

                var id = Convert.ToString(Request.QueryString["id"]);
                uint moduloId;

                if (string.IsNullOrWhiteSpace(id) || !uint.TryParse(id, out moduloId))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadModulo(moduloId);
                LoadNavegacion(moduloId);
                ucBitacora.Cargar(moduloId.ToString());
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

        private void CargarModulosNavegacionDestino()
        {
            ddlNavegacionDestino.Items.Clear();
            ddlNavegacionDestino.Items.Add(new ListItem("Seleccione", "0"));

            var filtro = new ModuloFiltroModelo
            {
                Coincidencia = string.Empty,
                Estado = 0,
                CategoriaId = null,
                Orden = 1,
                Orientacion = 1
            };

            var respuesta = ModuloNegocio.GetReporteModulos(filtro);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var modulo in respuesta.Datos)
            {
                ddlNavegacionDestino.Items.Add(new ListItem(modulo.Nombre, modulo.Id.ToString()));
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

        protected void ddlBitacoraTabla_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfBitacoraEstado.Value = "0";

            tglBitacoraEstado.Attributes["data-hidden-id"] = hfBitacoraEstado.ClientID;
            tglBitacoraEstado.Attributes["class"] = "main-toggle br-3 text-center on";

            divBitacoraDetalle.Visible = true;

            CargarProcedimientosTabla(ddlBitacoraTabla.SelectedValue);
            CargarColumnasTabla(ddlBitacoraTabla.SelectedValue);
        }

        private void LoadModulo(uint moduloId)
        {
            var operacion = 0;

            var modulo = ModuloNegocio.GetModuloMantenedor(ref operacion, moduloId);

            if (operacion != 0 || modulo == null || modulo.Id == 0)
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            hfId.Value = modulo.Id.ToString();

            txtNombre.Text = modulo.Nombre;
            txtTituloPagina.Text = modulo.TituloPagina;
            txtTituloContenido.Text = modulo.TituloContenido;
            txtUrl.Text = modulo.Url;
            txtUrlPublica.Text = modulo.UrlPublica;

            if (ddlCategoria.Items.FindByValue(modulo.CategoriaId.ToString()) != null)
            {
                ddlCategoria.SelectedValue = modulo.CategoriaId.ToString();
            }

            if (modulo.Orden.HasValue && ddlOrden.Items.FindByValue(modulo.Orden.Value.ToString()) != null)
            {
                ddlOrden.SelectedValue = modulo.Orden.Value.ToString();
            }

            hfAdministrador.Value = modulo.Administrador.HasValue ? modulo.Administrador.Value.ToString() : "1";
            hfMenuVisible.Value = modulo.MenuVisible.HasValue ? modulo.MenuVisible.Value.ToString() : "1";
            hfBitacoraEstado.Value = modulo.BitacoraEstado.HasValue ? modulo.BitacoraEstado.Value.ToString() : "1";
            hfEstado.Value = modulo.Estado.HasValue ? modulo.Estado.Value.ToString() : "1";

            tglAdministrador.Attributes["class"] = modulo.Administrador.HasValue && modulo.Administrador.Value == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            tglMenuVisible.Attributes["class"] = modulo.MenuVisible.HasValue && modulo.MenuVisible.Value == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            tglBitacoraEstado.Attributes["class"] = modulo.BitacoraEstado.HasValue && modulo.BitacoraEstado.Value == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            tglEstado.Attributes["class"] = modulo.Estado.HasValue && modulo.Estado.Value == 0
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            if (modulo.BitacoraEstado.HasValue && modulo.BitacoraEstado.Value == 0)
            {
                CargarConfiguracionBitacoraActual(modulo);
            }

            UpdatedAtOriginal = modulo.UpdatedAt;
        }

        private void LoadNavegacion(uint moduloId)
        {
            var respuesta = ModuloNegocio.GetModuloNavegacionConfig(moduloId);

            if (respuesta == null || !respuesta.Exitoso)
            {
                gvNavegacion.DataSource = null;
                gvNavegacion.DataBind();

                var colorJs = HttpUtility.JavaScriptStringEncode(respuesta != null ? respuesta.AlertaColor : ConstantesModelo.ConsAlertaColorDanger);
                var tituloJs = HttpUtility.JavaScriptStringEncode(respuesta != null ? respuesta.AlertaTitulo : string.Empty);
                var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta != null ? respuesta.Mensaje : ConstantesModelo.ConsErrorModuloNavegacionConfig);

                ScriptManager.RegisterStartupScript(this, GetType(), "LoadNavegacionError", "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');", true);

                return;
            }

            gvNavegacion.DataSource = respuesta.Datos;
            gvNavegacion.DataBind();
        }

        private void CargarConfiguracionBitacoraActual(ModuloModelo modulo)
        {
            InicializarDdlBitacoraSp();
            InicializarDdlBitacoraParametro();

            if (!string.IsNullOrWhiteSpace(modulo.BitacoraSp))
            {
                ddlBitacoraSp.Items.Add(new ListItem(modulo.BitacoraSp, modulo.BitacoraSp));
                ddlBitacoraSp.SelectedValue = modulo.BitacoraSp;
            }

            var parametro = Convert.ToString(modulo.BitacoraParametro);

            if (!string.IsNullOrWhiteSpace(parametro) && parametro.StartsWith("P_"))
            {
                parametro = parametro.Substring(2);
            }

            if (!string.IsNullOrWhiteSpace(parametro))
            {
                ddlBitacoraParametro.Items.Add(new ListItem(parametro, parametro));
                ddlBitacoraParametro.SelectedValue = parametro;
            }
        }

        private void LimpiarFormularioNavegacion()
        {
            hfNavegacionId.Value = "0";

            if (ddlNavegacionAccion.Items.FindByValue("0") != null)
            {
                ddlNavegacionAccion.SelectedValue = "0";
            }

            if (ddlNavegacionDestino.Items.FindByValue("0") != null)
            {
                ddlNavegacionDestino.SelectedValue = "0";
            }

            hfNavegacionEstado.Value = "0";
            tglNavegacionEstado.Attributes["class"] = "main-toggle br-3 text-center on";

            btnGuardarNavegacion.Text = "Agregar";
        }

        private bool ExisteAccionNavegacion(uint moduloId, string accion, uint navegacionIdActual)
        {
            if (moduloId == 0 || string.IsNullOrWhiteSpace(accion) || accion == "0")
            {
                return false;
            }

            var respuesta = ModuloNegocio.GetModuloNavegacionConfig(moduloId);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return false;
            }

            foreach (var item in respuesta.Datos)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.Id == navegacionIdActual)
                {
                    continue;
                }

                if (string.Equals(item.Accion, accion.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected void btnGuardarNavegacion_Click(object sender, EventArgs e)
        {
            uint moduloId;
            uint navegacionId;
            uint moduloIdDestino;
            sbyte estado;

            if (!uint.TryParse(hfId.Value, out moduloId) || moduloId == 0)
            {
                MostrarAlertaNavegacion(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionOrigenNoIdentificado);
                return;
            }

            if (!uint.TryParse(hfNavegacionId.Value, out navegacionId))
            {
                navegacionId = 0;
            }

            if (!uint.TryParse(ddlNavegacionDestino.SelectedValue, out moduloIdDestino))
            {
                moduloIdDestino = 0;
            }

            if (!sbyte.TryParse(hfNavegacionEstado.Value, out estado))
            {
                estado = -1;
            }

            var accion = ddlNavegacionAccion.SelectedValue;

            if (ExisteAccionNavegacion(moduloId, accion, navegacionId))
            {
                MostrarAlertaNavegacion(ConstantesModelo.ConsAlertaColorWarning, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionAccionDuplicada);
                return;
            }

            var navegacion = new ModuloNavegacionConfigModelo
            {
                Id = navegacionId,
                ModuloIdOrigen = moduloId,
                Accion = accion,
                ModuloIdDestino = moduloIdDestino,
                Estado = estado,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = ModuloNegocio.GuardarModuloNavegacion(navegacion);

            if (respuesta != null && respuesta.Exitoso)
            {
                LimpiarFormularioNavegacion();
                LoadNavegacion(moduloId);
                ucBitacora.Cargar(moduloId.ToString());
            }

            MostrarAlertaNavegacion(respuesta != null ? respuesta.AlertaColor : ConstantesModelo.ConsAlertaColorDanger, respuesta != null ? respuesta.AlertaTitulo : string.Empty, respuesta != null ? respuesta.Mensaje : ConstantesModelo.ConsErrorModuloNavegacionRegistrar);
        }

        protected void gvNavegacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EditarNavegacion")
            {
                return;
            }

            uint moduloId;
            uint navegacionId;

            if (!uint.TryParse(hfId.Value, out moduloId) || moduloId == 0)
            {
                MostrarAlertaNavegacion(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionOrigenNoIdentificado);
                return;
            }

            if (!uint.TryParse(Convert.ToString(e.CommandArgument), out navegacionId) || navegacionId == 0)
            {
                MostrarAlertaNavegacion(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionDatosNoObtenidos);
                return;
            }

            var respuesta = ModuloNegocio.GetModuloNavegacionConfig(moduloId);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                MostrarAlertaNavegacion(respuesta != null ? respuesta.AlertaColor : ConstantesModelo.ConsAlertaColorDanger, respuesta != null ? respuesta.AlertaTitulo : string.Empty, respuesta != null ? respuesta.Mensaje : ConstantesModelo.ConsErrorModuloNavegacionConfig);
                return;
            }

            ModuloNavegacionConfigModelo navegacion = null;

            foreach (var item in respuesta.Datos)
            {
                if (item.Id == navegacionId)
                {
                    navegacion = item;
                    break;
                }
            }

            if (navegacion == null)
            {
                MostrarAlertaNavegacion(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionModuloNavegacionDatosNoObtenidos);
                return;
            }

            hfNavegacionId.Value = navegacion.Id.ToString();

            if (ddlNavegacionAccion.Items.FindByValue(navegacion.Accion) != null)
            {
                ddlNavegacionAccion.SelectedValue = navegacion.Accion;
            }
            else
            {
                ddlNavegacionAccion.SelectedValue = "0";
            }

            if (ddlNavegacionDestino.Items.FindByValue(navegacion.ModuloIdDestino.ToString()) == null)
            {
                ddlNavegacionDestino.Items.Add(new ListItem(navegacion.ModuloNombreDestino, navegacion.ModuloIdDestino.ToString()));
            }

            ddlNavegacionDestino.SelectedValue = navegacion.ModuloIdDestino.ToString();

            if (navegacion.Estado.HasValue)
            {
                hfNavegacionEstado.Value = navegacion.Estado.Value.ToString();

                tglNavegacionEstado.Attributes["class"] = navegacion.Estado.Value == 0
                    ? "main-toggle br-3 text-center on"
                    : "main-toggle br-3 text-center";
            }
            else
            {
                hfNavegacionEstado.Value = "1";
                tglNavegacionEstado.Attributes["class"] = "main-toggle br-3 text-center";
            }

            btnGuardarNavegacion.Text = "Actualizar";
        }

        private void MostrarAlertaNavegacion(string color, string titulo, string mensaje)
        {
            var colorJs = HttpUtility.JavaScriptStringEncode(string.IsNullOrWhiteSpace(color) ? ConstantesModelo.ConsAlertaColorWarning : color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(titulo ?? string.Empty);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje ?? string.Empty);

            var script =
                "$('#global-loader').hide();" +
                "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarAlertaNavegacion", script, true);
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

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarModulo", scriptSinPermiso, true);
                return;
            }

            uint moduloId;
            sbyte administrador;
            sbyte menuVisible;
            sbyte bitacoraEstado;
            sbyte estado;

            if (!uint.TryParse(hfId.Value, out moduloId) || moduloId == 0)
            {
                var mensajeIdInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroIdInvalido);

                var scriptIdInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeIdInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "ModuloIdInvalido", scriptIdInvalido, true);
                return;
            }

            if (!sbyte.TryParse(hfAdministrador.Value, out administrador) || (administrador != 0 && administrador != 1))
            {
                var mensajeAdministradorInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionModuloAdministradorInvalido);

                var scriptAdministradorInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
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
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeMenuVisibleInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "MenuVisibleModuloInvalido", scriptMenuVisibleInvalido, true);
                return;
            }

            if (!sbyte.TryParse(hfBitacoraEstado.Value, out bitacoraEstado) || (bitacoraEstado != 0 && bitacoraEstado != 1))
            {
                var mensajeBitacoraEstadoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionModuloBitacoraEstadoInvalido);

                var scriptBitacoraEstadoInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeBitacoraEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "BitacoraEstadoModuloInvalido", scriptBitacoraEstadoInvalido, true);
                return;
            }

            if (!sbyte.TryParse(hfEstado.Value, out estado) || (estado != 0 && estado != 1))
            {
                var mensajeEstadoInvalidoJs = HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsValidacionRegistroEstadoInvalido);

                var scriptEstadoInvalido =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('danger', '', '" + mensajeEstadoInvalidoJs + "');";

                ScriptManager.RegisterStartupScript(this, GetType(), "EstadoModuloInvalido", scriptEstadoInvalido, true);
                return;
            }

            var bitacoraParametro = string.Empty;

            if (!string.IsNullOrWhiteSpace(ddlBitacoraParametro.SelectedValue))
            {
                bitacoraParametro = ddlBitacoraParametro.SelectedValue.StartsWith("P_")
                    ? ddlBitacoraParametro.SelectedValue
                    : "P_" + ddlBitacoraParametro.SelectedValue;
            }

            var modulo = new ModuloModelo
            {
                Id = moduloId,
                CategoriaId = Convert.ToUInt32(ddlCategoria.SelectedValue),
                Nombre = txtNombre.Text.Trim(),
                TituloPagina = txtTituloPagina.Text.Trim(),
                TituloContenido = txtTituloContenido.Text.Trim(),
                Orden = (sbyte?)Convert.ToInt32(ddlOrden.SelectedValue),
                Url = txtUrl.Text.Trim(),
                UrlPublica = txtUrlPublica.Text.Trim(),
                Estado = estado,
                Administrador = administrador,
                MenuVisible = menuVisible,
                BitacoraEstado = bitacoraEstado,
                BitacoraSp = ddlBitacoraSp.SelectedValue,
                BitacoraParametro = bitacoraParametro,
                UpdatedAt = UpdatedAtOriginal,
                RutUsuario = Convert.ToString(HttpContext.Current.Session["cliente"])
            };

            var respuesta = ModuloNegocio.ActualizarModulo(modulo);

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

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}