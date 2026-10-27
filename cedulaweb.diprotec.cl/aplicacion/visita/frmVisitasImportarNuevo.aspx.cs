using Modelo;
using Modelo.v1.Visitas;
using Negocio;
using Negocio.v1.Visitas;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using cedulaweb.diprotec.cl.aplicacion.helpers;

namespace cedulaweb.diprotec.cl.aplicacion.visita
{
    public partial class frmVisitasImportarNuevo : Page
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
        }

        protected void btnProcesar_Click(object sender, EventArgs e)
        {
            var urlVolverPermiso = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);
            var urlModuloPermiso = urlVolverPermiso.Split('?')[0];

            if (!PermisoModuloHelper.TienePermisoNuevo(this, urlModuloPermiso))
            {
                var scriptSinPermiso = PermisoModuloHelper.ScriptSinPermiso(ConstantesModelo.ConsErrorPermisoRegistrarRegistro);

                ScriptManager.RegisterStartupScript(this, GetType(), "SinPermisoProcesarVisitaImportar", scriptSinPermiso, true);
                return;
            }

            var carga = new EmpresaVisitaCargaModelo
            {
                Id = 0,
                RutEmpresa = Convert.ToString(Session["empresa"]),
                ArchivoNombre = fuArchivoCarga.HasFile ? Path.GetFileName(fuArchivoCarga.FileName) : string.Empty,
                ArchivoRuta = string.Empty,
                TotalFilas = 0,
                TotalOk = 0,
                TotalError = 0,
                Estado = ConstantesModelo.ConsCargaEstadoProcesando,
                CreatedAt = string.Empty,
                CreatedBy = Convert.ToString(Session["cliente"]),
                CreatedByNombre = Convert.ToString(Session["clienteNombre"])
            };

            var respuesta = EmpresaVisitaCargaNegocio.ProcesarCargaVisitasCsv(
                carga,
                fuArchivoCarga.HasFile ? fuArchivoCarga.PostedFile.InputStream : null
            );

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
                    "$('#" + btnProcesar.ClientID + "').val('Procesado').prop('disabled', true);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', true);";
            }
            else
            {
                script +=
                    "$('#" + btnProcesar.ClientID + "').val('Procesar').prop('disabled', false);" +
                    "$('#" + btnVolver.ClientID + "').prop('disabled', false);";
            }

            script += mensajeAlerta;

            ScriptManager.RegisterStartupScript(this, GetType(), "btnProcesar_Click", script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            var url = ModuloNavegacionHelper.GetUrlAccion(this, ConstantesModelo.ConsModulosAccionUrlVolver);

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
