using Modelo.Comunes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace Negocio.Servicios
{
    class NoticiaServicio
    {
        private const int CantidadNoticiasHome = 20;

        private sealed class FuenteRssConfiguracion
        {
            public string Nombre { get; set; }

            public string Url { get; set; }
        }

        private static readonly FuenteRssConfiguracion[] FuentesRssNoticias =
        {
            new FuenteRssConfiguracion
            {
                Nombre = "Cooperativa",
                Url = "https://www.cooperativa.cl/noticias/site/tax/port/all/rss_8___1.xml"
            },
            new FuenteRssConfiguracion
            {
                Nombre = "BioBioChile",
                Url = "https://www.biobiochile.cl/static/feed-rss"
            },
            new FuenteRssConfiguracion
            {
                Nombre = "Emol",
                Url = "https://rss.emol.com/rss.asp?canal=0"
            },
            new FuenteRssConfiguracion
            {
                Nombre = "Diario El Día",
                Url = "https://www.diarioeldia.cl/rss/noticias/"
            }
        };

        public static async Task<List<NoticiaModelo>> GetNoticiasAsync()
        {
            return await Task.Run(() =>
            {
                var noticias = new List<NoticiaModelo>();

                foreach (var fuenteRss in FuentesRssNoticias)
                {
                    var noticiasFuente = ObtenerNoticiasDesdeRss(fuenteRss);

                    if (noticiasFuente != null && noticiasFuente.Count > 0)
                    {
                        noticias.AddRange(noticiasFuente);
                    }
                }

                var noticiasDisponibles = noticias
                    .Where(noticia => noticia != null && !string.IsNullOrWhiteSpace(noticia.Titulo) && !string.IsNullOrWhiteSpace(noticia.Fuente))
                    .GroupBy(noticia => ObtenerClaveNoticia(noticia), StringComparer.OrdinalIgnoreCase)
                    .Select(grupo => grupo.OrderByDescending(noticia => noticia.FechaPublicacion).First())
                    .ToList();

                var colasPorFuente = noticiasDisponibles
                    .GroupBy(noticia => noticia.Fuente, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(grupo => Guid.NewGuid())
                    .Select(grupo => new Queue<NoticiaModelo>(grupo.OrderBy(noticia => Guid.NewGuid())))
                    .ToList();

                var noticiasSeleccionadas = new List<NoticiaModelo>();

                while (noticiasSeleccionadas.Count < CantidadNoticiasHome && colasPorFuente.Any(cola => cola.Count > 0))
                {
                    var colasRonda = colasPorFuente.OrderBy(cola => Guid.NewGuid()).ToList();

                    foreach (var colaFuente in colasRonda)
                    {
                        if (noticiasSeleccionadas.Count >= CantidadNoticiasHome)
                        {
                            break;
                        }

                        if (colaFuente.Count == 0)
                        {
                            continue;
                        }

                        noticiasSeleccionadas.Add(colaFuente.Dequeue());
                    }
                }

                return noticiasSeleccionadas;
            });
        }

        private static List<NoticiaModelo> ObtenerNoticiasDesdeRss(FuenteRssConfiguracion fuenteRss)
        {
            var noticias = new List<NoticiaModelo>();

            if (fuenteRss == null || string.IsNullOrWhiteSpace(fuenteRss.Url))
            {
                return noticias;
            }

            try
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore,
                    XmlResolver = null
                };

                var separadorConsulta = fuenteRss.Url.Contains("?") ? "&" : "?";
                var urlConsulta = fuenteRss.Url + separadorConsulta + "nocache=" + DateTime.UtcNow.Ticks;

                using (var reader = XmlReader.Create(urlConsulta, settings))
                {
                    var feed = SyndicationFeed.Load(reader);

                    if (feed == null || feed.Items == null)
                    {
                        return noticias;
                    }

                    noticias = feed.Items
                        .Where(item => item != null)
                        .Select(item => new NoticiaModelo
                        {
                            Titulo = LimpiarTexto(item.Title != null ? item.Title.Text : string.Empty),
                            Resumen = LimpiarTexto(item.Summary != null ? item.Summary.Text : string.Empty),
                            Url = item.Links != null && item.Links.Count > 0 && item.Links[0].Uri != null ? item.Links[0].Uri.ToString() : string.Empty,
                            Fuente = fuenteRss.Nombre,
                            FechaPublicacion = item.PublishDate.DateTime
                        })
                        .ToList();
                }
            }
            catch
            {
                return noticias;
            }

            return noticias;
        }

        private static string ObtenerClaveNoticia(NoticiaModelo noticia)
        {
            if (noticia == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(noticia.Url))
            {
                return noticia.Url.Trim();
            }

            return noticia.Titulo != null ? noticia.Titulo.Trim() : string.Empty;
        }

        private static string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            texto = WebUtility.HtmlDecode(texto);
            texto = System.Text.RegularExpressions.Regex.Replace(texto, "<.*?>", string.Empty);
            texto = texto.Replace("\r", " ").Replace("\n", " ").Trim();

            if (texto.Length > 180)
            {
                texto = texto.Substring(0, 180).Trim() + "...";
            }

            return texto;
        }
    }
}