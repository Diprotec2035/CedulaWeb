using Modelo;
using Modelo.v1.ResetPassword;
using Negocio.v1.ResetPassword;
using System;
using System.Web;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.recuperar
{
    public partial class _default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Session.Clear();

            if (Page.IsPostBack) return;

            var salida = Request.QueryString["exit"];
            if (salida == "1")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Salir", "UnblockUI(); alert(\"" + ConstantesModelo.ConsErrorSesion + "\");", true);
            }
        }

        protected async void btnReset_Click(object sender, EventArgs e)
        {
            var mensajeAlerta = string.Empty;

            var reset = new ResetPasswordModelo
            {
                RutEmpresa = txtRutEmpresa.Text.Trim(),
                RutUsuario = txtRutUsuario.Text.Trim(),
                EmailUsuario = txtEmailUsuario.Text.Trim(),
                TokenCaptcha = gRecaptchaResponse.Value,
                HttpXForwardedFor = Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
                UserHostAddress = Request.UserHostAddress
            };

            var respuesta = await ResetPasswordNegocio.ResetearPasswordAsync(reset);

            if (!string.IsNullOrWhiteSpace(respuesta.Mensaje))
            {
                var mensajeJs = HttpUtility.JavaScriptStringEncode(respuesta.Mensaje);
                var alertaColorJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaColor);
                var alertaTituloJs = HttpUtility.JavaScriptStringEncode(respuesta.AlertaTitulo);

                mensajeAlerta = "mostrarAlerta('" + alertaColorJs + "', '" + alertaTituloJs + "', '" + mensajeJs + "');";
            }

            var script = "$('#" + btnReset.ClientID + "').val('Resetear Contrase\\u00F1a').prop('disabled', false);" +
                "$('#lnkVolver').css({'pointer-events':'auto','opacity':'1'});" +
                mensajeAlerta;

            ScriptManager.RegisterStartupScript(this, GetType(), "Desbloquear", script, true);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect(ConstantesModelo.ConsHttpInicio, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}