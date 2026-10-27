using Modelo;
using Modelo.Comunes;
using Modelo.v1.Categoria;
using Modelo.v1.Modulo;
using Negocio.v1.Categoria;
using Negocio.v1.Modulo;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Comunes
{
    public class MenuNegocio
    {
        public static List<CategoriaViewModel> GetMenu(EmpresaUsuarioModelo usuario)
        {
            var operacion = 0;

            List<CategoriaModelo> categorias;
            List<ModuloModelo> modulos;

            if (usuario.EsSuperAdmin)
            {
                categorias = CategoriaNegocio.GetCategoriasMenu(ref operacion);

                if (operacion != 0)
                {
                    return new List<CategoriaViewModel>();
                }

                modulos = ModuloNegocio.GetModulosMenu(ref operacion);

                if (operacion != 0)
                {
                    return new List<CategoriaViewModel>();
                }
            }
            else
            {
                categorias = CategoriaNegocio.GetCategoriaUsuario(ref operacion, usuario);

                if (operacion != 0)
                {
                    return new List<CategoriaViewModel>();
                }

                modulos = ModuloNegocio.GetModuloUsuario(ref operacion, usuario);

                if (operacion != 0)
                {
                    return new List<CategoriaViewModel>();
                }
            }

            var menu = categorias
                .OrderBy(c => c.Orden)
                .Select(c => new CategoriaViewModel
                {
                    Id = c.Id,
                    Texto = FuncionesNegocio.CapitalizarTexto(c.Nombre),
                    Icono = c.Icono,
                    Modulos = modulos
                        .Where(m => m.CategoriaId == c.Id)
                        .OrderBy(m => m.Orden)
                        .Select(m =>
                        {
                            m.Nombre = FuncionesNegocio.CapitalizarTexto(m.Nombre);
                            return m;
                        })
                        .ToList()
                })
                .Where(c => c.Modulos.Any())
                .ToList();

            return menu;
        }

        public static string GetUrlRealModulo(string urlPublica)
        {
            var operacion = 0;

            var modulos = ModuloNegocio.GetModulos(ref operacion, null);

            if (operacion != 0)
            {
                return string.Empty;
            }

            var modulo = modulos.FirstOrDefault(m => m.UrlPublica == urlPublica);

            if (modulo == null)
            {
                return string.Empty;
            }

            return modulo.Url;
        }
    }
}
