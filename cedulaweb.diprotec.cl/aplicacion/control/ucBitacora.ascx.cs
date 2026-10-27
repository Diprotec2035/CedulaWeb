using cedulaweb.diprotec.cl.aplicacion.helpers;
using Negocio.v1.Bitacora;
using System;
using System.Web.UI;

namespace cedulaweb.diprotec.cl.aplicacion.control
{
    public partial class ucBitacora : UserControl
    {
        public string ModalClientId
        {
            get { return ClientID + "_modalBitacora"; }
        }

        public string ModalLabelClientId
        {
            get { return ClientID + "_modalBitacoraLabel"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Cargar(string idRegistro)
        {
            if (string.IsNullOrWhiteSpace(idRegistro))
            {
                Limpiar();
                return;
            }

            var contexto = ModuloContextoHelper.ValidarModuloPagina(Page);

            if (contexto == null ||
                !contexto.Exitoso ||
                !contexto.BitacoraEstado.HasValue ||
                contexto.BitacoraEstado.Value != 0 ||
                string.IsNullOrWhiteSpace(contexto.BitacoraSp) ||
                string.IsNullOrWhiteSpace(contexto.BitacoraParametro))
            {
                Limpiar();
                return;
            }

            Cargar(
                contexto.BitacoraSp,
                contexto.BitacoraParametro,
                idRegistro
            );
        }

        public void Cargar(string storedProcedure, string parametro, string idRegistro)
        {
            if (string.IsNullOrWhiteSpace(storedProcedure) ||
                string.IsNullOrWhiteSpace(parametro) ||
                string.IsNullOrWhiteSpace(idRegistro))
            {
                Limpiar();
                return;
            }

            var respuesta = BitacoraNegocio.GetBitacora(
                storedProcedure,
                parametro,
                idRegistro
            );

            if (!respuesta.Exitoso || respuesta.Datos == null)
            {
                Limpiar();
                return;
            }

            gvBitacora.DataSource = respuesta.Datos;
            gvBitacora.DataBind();
        }

        private void Limpiar()
        {
            gvBitacora.DataSource = null;
            gvBitacora.DataBind();
        }

        public string GetScriptAbrir()
        {
            return "var modal = new bootstrap.Modal(document.getElementById('" + ModalClientId + "')); modal.show();";
        }
    }
}