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
    public partial class frmDispositivosNuevo : Page
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

            var urlVolver = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModulo = urlVolver.Split('?')[0];

            if (!PermisoModuloHelper.ValidarPuedeNuevo(this, urlModulo, urlVolver, ConstantesModelo.ConsErrorPermisoRegistrarRegistro))
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadMarcas();
                LoadModelos(null);
            }
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

        protected async void btnRegistrar_Click(object sender, EventArgs e)
        {
            var urlVolverPermiso = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModuloPermiso = urlVolverPermiso.Split('?')[0];

            if (!PermisoModuloHelper.TienePermisoNuevo(this, urlModuloPermiso))
            {
                var scriptSinPermiso =
                    "$('#global-loader').hide();" +
                    "$('#" + btnRegistrar.ClientID + "').val('Registrar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);" +
                    PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoRegistrarDispositivo", scriptSinPermiso, true);
                return;
            }

            if (!uint.TryParse(ddlModelo.SelectedValue, out uint modeloId) || modeloId == 0)
            {
                MostrarAlerta(ConstantesModelo.ConsValidacionDispositivoModeloVacio);
                return;
            }

            var dispositivo = new DispositivoModelo
            {
                RutEmpresa = Convert.ToString(HttpContext.Current.Session["empresa"]),
                ModeloId = modeloId,
                PartNumber = txtPartnumber.Text.Trim(),
                SerialNumber = txtSerialnumber.Text.Trim(),
                Comentario = txtComentarios.Text.Trim(),
                Estado = 0,
                RutUsuario = Convert.ToString(Session["cliente"])
            };

            var respuesta = await DispositivoNegocio.RegistrarDispositivoAsync(dispositivo);

            string script;

            var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
            var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
            var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

            if (respuesta.Exitoso)
            {
                LimpiarFormulario();

                var activationCode = respuesta.Datos != null ? respuesta.Datos.ActivationCode : string.Empty;
                var activationCodeJs = HttpUtility.JavaScriptStringEncode(activationCode);

                script =
                    "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');" +
                    "mostrarModalCodigoActivacion('" + activationCodeJs + "');";
            }
            else
            {
                script = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "btnConfirmar_Click", script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
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

        private void LimpiarFormulario()
        {
            ddlMarca.ClearSelection();
            ddlModelo.Items.Clear();
            ddlModelo.Items.Add(new ListItem("Seleccione", string.Empty));

            txtPartnumber.Text = string.Empty;
            txtSerialnumber.Text = string.Empty;
            txtComentarios.Text = string.Empty;
        }

        private void MostrarAlerta(string mensaje)
        {
            var mensajeJs = HttpUtility.JavaScriptStringEncode(mensaje);
            var script = "mostrarAlerta('warning', '', '" + mensajeJs + "');";

            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarAlerta", script, true);
        }
    }
}