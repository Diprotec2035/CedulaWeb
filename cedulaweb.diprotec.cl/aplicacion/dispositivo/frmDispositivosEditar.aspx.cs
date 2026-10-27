using Modelo;
using Modelo.v1.Dispositivo;
using Modelo.v1.DispositivoMarcas;
using Modelo.v1.DispositivoModelos;
using Negocio;
using Negocio.v1.Dispositivo;
using Negocio.v1.DispositivoMarcas;
using Negocio.v1.DispositivoModelos;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.dispositivo
{
    public partial class frmDispositivosEditar : Page
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

            var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModulo = urlVolver.Split('?')[0];

            if (!PermisoModuloHelper.ValidarPuedeModificar(this, urlModulo, urlVolver, ConstantesModelo.ConsErrorPermisoModificarRegistro))
            {
                return;
            }

            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;

            if (!IsPostBack)
            {
                if (!uint.TryParse(Convert.ToString(Request.QueryString["id"]), out uint dispositivoId))
                {
                    Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                    Session["AlertaTitulo"] = string.Empty;
                    Session["AlertaMensaje"] = ConstantesModelo.ConsValidacionRegistroIdInvalido;

                    Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadMarcas();
                LoadDispositivo(dispositivoId);
                ucBitacora.Cargar(dispositivoId.ToString());
            }
        }

        private void LoadMarcas()
        {
            ddlMarca.Items.Clear();
            ddlMarca.Items.Add(new ListItem("Seleccione", string.Empty));

            var filtro = new DispositivoMarcasFiltroModelo
            {
                Coincidencia = string.Empty,
                Estado = 0,
                Orden = 1,
                Orientacion = 1
            };

            var respuesta = DispositivoMarcasNegocio.GetReporteDispositivoMarcas(filtro);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var marca in respuesta.Datos)
            {
                ddlMarca.Items.Add(new ListItem(marca.Nombre, marca.Id.ToString()));
            }
        }

        private void LoadDispositivo(uint id)
        {
            var operacion = 0;

            var dispositivoFiltro = new DispositivoModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Id = id,
                Estado = null
            };

            var dispositivo = DispositivoNegocio.GetDispositivo(ref operacion, dispositivoFiltro);

            if (operacion != 0 || dispositivo == null || dispositivo.Id == 0)
            {
                Session["AlertaColor"] = ConstantesModelo.ConsAlertaColorDanger;
                Session["AlertaTitulo"] = string.Empty;
                Session["AlertaMensaje"] = ConstantesModelo.ConsErrorRegistroObtener;

                Response.Redirect(ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (dispositivo.MarcaId.HasValue &&
                ddlMarca.Items.FindByValue(dispositivo.MarcaId.Value.ToString()) != null)
            {
                ddlMarca.SelectedValue = dispositivo.MarcaId.Value.ToString();
                LoadModelos(dispositivo.MarcaId.Value);
            }
            else
            {
                LoadModelos(null);
            }

            if (dispositivo.ModeloId.HasValue &&
                ddlModelo.Items.FindByValue(dispositivo.ModeloId.Value.ToString()) != null)
            {
                ddlModelo.SelectedValue = dispositivo.ModeloId.Value.ToString();
            }

            txtPartnumber.Text = dispositivo.PartNumber;
            txtSerialnumber.Text = dispositivo.SerialNumber;
            txtComentarios.Text = dispositivo.Comentario;

            var estadoActual = dispositivo.Estado.HasValue
                ? dispositivo.Estado.Value
                : (sbyte)1;

            hfEstadoActual.Value = estadoActual.ToString();
            hfEstado.Value = estadoActual.ToString();

            AplicarEstadoActual(estadoActual);

            tglEstado.Attributes["data-hidden-id"] = hfEstado.ClientID;
            tglEstado.Attributes["class"] = estadoActual == 0 || estadoActual == 2
                ? "main-toggle br-3 text-center on"
                : "main-toggle br-3 text-center";

            UpdatedAtOriginal = dispositivo.UpdatedAt;
        }

        protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(ddlMarca.SelectedValue, out uint marcaId) || marcaId == 0)
            {
                LoadModelos(null);
                return;
            }

            LoadModelos(marcaId);
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            var urlVolverPermiso = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModuloPermiso = urlVolverPermiso.Split('?')[0];

            if (!PermisoModuloHelper.TienePermisoModificar(this, urlModuloPermiso))
            {
                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoModificarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoActualizarDispositivo", scriptSinPermiso, true);
                return;
            }

            if (!uint.TryParse(Convert.ToString(Request.QueryString["id"]), out uint dispositivoId))
            {
                MostrarAlertaRestaurandoBotones(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionRegistroIdInvalido);
                return;
            }

            if (!uint.TryParse(ddlModelo.SelectedValue, out uint modeloId) || modeloId == 0)
            {
                MostrarAlertaRestaurandoBotones(ConstantesModelo.ConsAlertaColorWarning, string.Empty, ConstantesModelo.ConsValidacionDispositivoModeloVacio);
                return;
            }

            sbyte estadoSolicitado;

            if (!sbyte.TryParse(hfEstado.Value, out estadoSolicitado) ||
                (estadoSolicitado != 0 && estadoSolicitado != 1 && estadoSolicitado != 2))
            {
                MostrarAlertaRestaurandoBotones(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionRegistroEstadoInvalido);
                return;
            }

            var operacion = 0;

            var dispositivoFiltro = new DispositivoModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Id = dispositivoId,
                Estado = null
            };

            var dispositivoActual = DispositivoNegocio.GetDispositivo(ref operacion, dispositivoFiltro);

            if (operacion != 0 || dispositivoActual == null || dispositivoActual.Id == 0)
            {
                MostrarAlertaRestaurandoBotones(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsErrorRegistroObtener);
                return;
            }

            var estadoActual = dispositivoActual.Estado.HasValue
                ? dispositivoActual.Estado.Value
                : (sbyte)1;

            if (estadoActual != 0 && estadoActual != 1 && estadoActual != 2)
            {
                MostrarAlertaRestaurandoBotones(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionRegistroEstadoInvalido);
                return;
            }

            if (estadoSolicitado == 2 && estadoActual != 2)
            {
                MostrarAlertaRestaurandoBotones(ConstantesModelo.ConsAlertaColorDanger, string.Empty, ConstantesModelo.ConsValidacionRegistroEstadoInvalido);
                return;
            }

            var dispositivo = new DispositivoModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                Id = dispositivoId,
                ModeloId = modeloId,
                PartNumber = txtPartnumber.Text.Trim(),
                SerialNumber = txtSerialnumber.Text.Trim(),
                Comentario = txtComentarios.Text.Trim(),
                Estado = estadoSolicitado,
                ActivationCodeHash = dispositivoActual.ActivationCodeHash,
                RutUsuario = Convert.ToString(Session["cliente"]),
                UpdatedAt = UpdatedAtOriginal
            };

            var respuesta = DispositivoNegocio.ActualizarDispositivo(dispositivo);

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            var script = "$('#global-loader').hide();";

            if (respuesta.Exitoso)
            {
                var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
                var urlVolverJs = HttpUtility.JavaScriptStringEncode(urlVolver);

                script +=
                    "$('#" + btnActualizar.ClientID + "').val('Actualizado').prop('disabled', true);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', true);" +
                    "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');" +
                    "setTimeout(function(){ window.location = '" + urlVolverJs + "'; }, 1200);";
            }
            else
            {
                script +=
                    "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "btnActualizar_Click", script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LoadModelos(uint? marcaId)
        {
            ddlModelo.Items.Clear();
            ddlModelo.Items.Add(new ListItem("Seleccione", string.Empty));

            if (!marcaId.HasValue || marcaId.Value == 0)
            {
                return;
            }

            var filtro = new DispositivoModelosFiltroModelo
            {
                IdMarca = marcaId,
                Coincidencia = string.Empty,
                Estado = 0,
                Orden = 1,
                Orientacion = 1
            };

            var respuesta = DispositivoModelosNegocio.GetReporteDispositivoModelos(filtro);

            if (respuesta == null || !respuesta.Exitoso || respuesta.Datos == null)
            {
                return;
            }

            foreach (var modelo in respuesta.Datos)
            {
                ddlModelo.Items.Add(new ListItem(modelo.Nombre, modelo.Id.ToString()));
            }
        }

        private void MostrarAlertaRestaurandoBotones(string color, string titulo, string mensaje)
        {
            var colorJs = HttpUtility.JavaScriptStringEncode(color);
            var tituloJs = HttpUtility.JavaScriptStringEncode(titulo);
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);

            var script =
                "$('#global-loader').hide();" +
                "$('#" + btnActualizar.ClientID + "').val('Actualizar').prop('disabled', false);" +
                "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                "mostrarAlerta('" + colorJs + "', '" + tituloJs + "', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarAlertaRestaurandoBotones", script, true);
        }

        private void AplicarEstadoActual(sbyte estado)
        {
            if (estado != 2)
            {
                divEstadoActual.Visible = false;
                return;
            }

            divEstadoActual.Visible = true;

            spnEstadoActual.Attributes["class"] = "badge bg-info-transparent text-info px-3 py-2 fs-12";
            spnEstadoActual.InnerHtml = "<i class=\"fe fe-smartphone me-1\"></i>Activado";
        }
    }
}