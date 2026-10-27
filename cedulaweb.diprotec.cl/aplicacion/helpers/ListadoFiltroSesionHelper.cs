using Modelo.Comunes;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace cedulaweb.diprotec.cl.aplicacion.helpers
{
    public static class ListadoFiltroSesionHelper
    {
        private const string PrefijoClaveSesion = "FILTROS_LISTADO_";
        private const string AtributoFiltroSesion = "data-filtro-sesion";

        public static void Guardar(Page page, Control contenedor, int pageIndex)
        {
            if (page == null || contenedor == null || HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return;
            }

            uint moduloId;

            if (!uint.TryParse(Convert.ToString(page.Request.QueryString["mod_id"]), out moduloId) || moduloId == 0)
            {
                return;
            }

            var filtros = new ListadoFiltroSesionModelo
            {
                ModuloId = moduloId,
                PageIndex = pageIndex
            };

            GuardarControles(contenedor, filtros.Filtros);

            HttpContext.Current.Session[GetClaveSesion(moduloId)] = filtros;
        }

        public static ListadoFiltroSesionModelo Obtener(Page page)
        {
            if (page == null || HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return null;
            }

            uint moduloId;

            if (!uint.TryParse(Convert.ToString(page.Request.QueryString["mod_id"]), out moduloId) || moduloId == 0)
            {
                return null;
            }

            return HttpContext.Current.Session[GetClaveSesion(moduloId)] as ListadoFiltroSesionModelo;
        }

        public static void Restaurar(Page page, Control contenedor)
        {
            if (contenedor == null)
            {
                return;
            }

            var filtros = Obtener(page);

            if (filtros == null || filtros.Filtros == null)
            {
                return;
            }

            RestaurarControles(contenedor, filtros.Filtros);
        }

        public static void Limpiar(Page page)
        {
            if (page == null || HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return;
            }

            uint moduloId;

            if (!uint.TryParse(Convert.ToString(page.Request.QueryString["mod_id"]), out moduloId) || moduloId == 0)
            {
                return;
            }

            HttpContext.Current.Session.Remove(GetClaveSesion(moduloId));
        }

        private static string GetClaveSesion(uint moduloId)
        {
            return PrefijoClaveSesion + moduloId;
        }

        private static void GuardarControles(Control control, Dictionary<string, string> filtros)
        {
            if (control == null || filtros == null)
            {
                return;
            }

            var webControl = control as WebControl;

            if (webControl != null)
            {
                var claveFiltro = webControl.Attributes[AtributoFiltroSesion];

                if (!string.IsNullOrWhiteSpace(claveFiltro))
                {
                    filtros[claveFiltro] = ObtenerValorControl(control);
                }
            }

            foreach (Control hijo in control.Controls)
            {
                GuardarControles(hijo, filtros);
            }
        }

        private static void RestaurarControles(Control control, Dictionary<string, string> filtros)
        {
            if (control == null || filtros == null)
            {
                return;
            }

            var webControl = control as WebControl;

            if (webControl != null)
            {
                var claveFiltro = webControl.Attributes[AtributoFiltroSesion];

                if (!string.IsNullOrWhiteSpace(claveFiltro) && filtros.ContainsKey(claveFiltro))
                {
                    AsignarValorControl(control, filtros[claveFiltro]);
                }
            }

            foreach (Control hijo in control.Controls)
            {
                RestaurarControles(hijo, filtros);
            }
        }

        private static string ObtenerValorControl(Control control)
        {
            var textBox = control as TextBox;

            if (textBox != null)
            {
                return textBox.Text.Trim();
            }

            var dropDownList = control as DropDownList;

            if (dropDownList != null)
            {
                return dropDownList.SelectedValue;
            }

            var hiddenField = control as HiddenField;

            if (hiddenField != null)
            {
                return hiddenField.Value;
            }

            var checkBox = control as CheckBox;

            if (checkBox != null)
            {
                return checkBox.Checked ? "1" : "0";
            }

            var radioButtonList = control as RadioButtonList;

            if (radioButtonList != null)
            {
                return radioButtonList.SelectedValue;
            }

            return string.Empty;
        }

        private static void AsignarValorControl(Control control, string valor)
        {
            var textBox = control as TextBox;

            if (textBox != null)
            {
                textBox.Text = valor ?? string.Empty;
                return;
            }

            var dropDownList = control as DropDownList;

            if (dropDownList != null)
            {
                if (!string.IsNullOrWhiteSpace(valor) && dropDownList.Items.FindByValue(valor) != null)
                {
                    dropDownList.SelectedValue = valor;
                }

                return;
            }

            var hiddenField = control as HiddenField;

            if (hiddenField != null)
            {
                hiddenField.Value = valor ?? string.Empty;
                return;
            }

            var checkBox = control as CheckBox;

            if (checkBox != null)
            {
                checkBox.Checked = valor == "1";
                return;
            }

            var radioButtonList = control as RadioButtonList;

            if (radioButtonList != null)
            {
                if (!string.IsNullOrWhiteSpace(valor) && radioButtonList.Items.FindByValue(valor) != null)
                {
                    radioButtonList.SelectedValue = valor;
                }
            }
        }

        public static bool DebeMantenerFiltros(Page page)
        {
            if (page == null || HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return false;
            }

            uint moduloId;

            if (!uint.TryParse(Convert.ToString(page.Request.QueryString["mod_id"]), out moduloId) || moduloId == 0)
            {
                return false;
            }

            var clave = "MANTENER_FILTROS_LISTADO_" + moduloId;

            var mantener = Convert.ToString(HttpContext.Current.Session[clave]) == "1";

            HttpContext.Current.Session.Remove(clave);

            return mantener;
        }
    }
}