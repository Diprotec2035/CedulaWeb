<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="frmFaq.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion.faq.frmFaq" %>
<%@ Import Namespace="cedulaweb.diprotec.cl.aplicacion.helpers" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        function normalizarTextoFaq(valor) {
            if (!valor) {
                return "";
            }

            return valor
                .toString()
                .toLowerCase()
                .normalize("NFD")
                .replace(/[\u0300-\u036f]/g, "")
                .trim();
        }

        function obtenerCategoriaFaq() {
            var categoria = $("#hfCategoriaFaq").val();

            if (!categoria || categoria.trim() === "") {
                return "todos";
            }

            return categoria;
        }

        function cerrarFaqAbiertasNoVisibles() {
            $(".faq-item.d-none .accordion-collapse").each(function () {
                var collapse = bootstrap.Collapse.getInstance(this);

                if (collapse) {
                    collapse.hide();
                } else {
                    $(this).removeClass("show");
                }
            });

            $(".faq-item.d-none .accordion-button")
                .addClass("collapsed")
                .attr("aria-expanded", "false");
        }

        function filtrarFaq() {
            var texto = normalizarTextoFaq($("#txtBuscarFaq").val());
            var categoriaActiva = obtenerCategoriaFaq();
            var totalVisibles = 0;

            $(".faq-section").removeClass("d-none");
            $(".faq-item").removeClass("d-none").attr("data-visible-faq", "0");

            $(".faq-item").each(function () {
                var item = $(this);
                var categoria = item.attr("data-categoria");
                var contenido = normalizarTextoFaq(item.text());

                var coincideTexto = texto === "" || contenido.indexOf(texto) >= 0;
                var coincideCategoria = categoriaActiva === "todos" || categoria === categoriaActiva;

                if (coincideTexto && coincideCategoria) {
                    item.removeClass("d-none");
                    item.attr("data-visible-faq", "1");
                    totalVisibles++;
                } else {
                    item.addClass("d-none");
                    item.attr("data-visible-faq", "0");
                }
            });

            $(".faq-section").each(function () {
                var seccion = $(this);
                var visibles = seccion.find(".faq-item[data-visible-faq='1']").length;

                if (visibles > 0) {
                    seccion.removeClass("d-none");
                } else {
                    seccion.addClass("d-none");
                }
            });

            cerrarFaqAbiertasNoVisibles();

            if (totalVisibles === 0) {
                $("#divFaqSinResultados").removeClass("d-none");
            } else {
                $("#divFaqSinResultados").addClass("d-none");
            }
        }

        function marcarCategoriaFaq(boton) {
            $(".btn-categoria-faq")
                .removeClass("btn-main-primary")
                .addClass("btn-outline-info");

            if (boton) {
                $(boton)
                    .removeClass("btn-outline-info")
                    .addClass("btn-main-primary");
            }
        }

        function seleccionarCategoriaFaq(categoria, boton) {
            $("#hfCategoriaFaq").val(categoria);
            $("#txtBuscarFaq").val("");

            marcarCategoriaFaq(boton);
            filtrarFaq();

            return false;
        }

        function buscarFaq() {
            $("#hfCategoriaFaq").val("todos");

            marcarCategoriaFaq($("#btnCategoriaTodos")[0]);
            filtrarFaq();

            return false;
        }

        $(document).ready(function () {
            $("#txtBuscarFaq").validText("<%=HttpUtility.JavaScriptStringEncode(ConstantesModelo.ConsTextoBusquedaFaq)%>");

            $("#txtBuscarFaq").on("keydown", function (e) {
                if (e.key === "Enter" || e.keyCode === 13) {
                    e.preventDefault();
                    buscarFaq();
                    return false;
                }
            });

            filtrarFaq();
        });

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Main Content-->
    <div class="main-content side-content pt-0">
        <div class="side-app">

            <div class="main-container container-fluid">

                <!-- Page Header -->
                <div class="page-header mb-2 mt-2">
                    <div>
                        <ol class="breadcrumb mb-0">
                            <li class="breadcrumb-item">
                                <a href="<%=ConstantesModelo.ConsModulosDefault%>">Inicio</a>
                            </li>
                            <li class="breadcrumb-item active text-primary" aria-current="page">
                                <%:ModuloContextoHelper.ObtenerTituloPagina(this)%>
                            </li>
                        </ol>
                    </div>
                </div>
                <!-- End Page Header -->

                <!-- Filtros FAQ -->
                <div class="row">
                    <div class="col-lg-12 col-md-12">
                        <div class="card custom-card">
                            <div class="card-body py-3">

                                <div class="row row-sm align-items-end">
                                    <div class="col-xl-6 col-lg-6 col-md-12 col-sm-12 mb-2">
                                        <label class="form-label mg-b-5">Buscar en preguntas frecuentes</label>
                                        <input id="txtBuscarFaq"
                                               type="text"
                                               class="form-control"
                                               maxlength="100"
                                               placeholder="Buscar por usuario, visita, RUT, regla, permiso, carga..." />
                                    </div>

                                    <div class="col-xl-2 col-lg-2 col-md-4 col-sm-12 mb-2">
                                        <label class="form-label mg-b-5 d-block">&nbsp;</label>
                                        <button type="button"
                                                class="btn ripple btn-outline-info btn-block w-100"
                                                onclick="return buscarFaq();">
                                            Buscar
                                        </button>
                                    </div>
                                </div>

                                <input type="hidden" id="hfCategoriaFaq" value="todos" />

                                <div class="mt-3 d-flex flex-wrap gap-2">
                                    <button id="btnCategoriaTodos" type="button" class="btn btn-sm btn-main-primary btn-categoria-faq" onclick="return seleccionarCategoriaFaq('todos', this);">Todos</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('primeros-pasos', this);">Primeros pasos</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('acceso', this);">Acceso y permisos</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('usuarios', this);">Usuarios</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('perfiles', this);">Perfiles</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('visitas', this);">Visitas</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('cargas', this);">Cargar visitas</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('registro', this);">Registro visitas</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('reglas', this);">Reglas</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('problemas', this);">Problemas frecuentes</button>
                                    <button type="button" class="btn btn-sm btn-outline-info btn-categoria-faq" onclick="return seleccionarCategoriaFaq('soporte', this);">Soporte</button>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Filtros FAQ -->

                <!-- FAQ -->
                <div class="row">
                    <div class="col-lg-12 col-md-12">
                        <div class="card custom-card">
                            <div class="card-header">
                                <h6 class="card-title">
                                    <%:ModuloContextoHelper.ObtenerTituloContenido(this)%>
                                </h6>
                            </div>

                            <div class="card-body">

                                <div id="divFaqSinResultados" class="alert alert-warning d-none" role="alert">
                                    No se encontraron preguntas frecuentes que coincidan con la búsqueda o categoría seleccionada.
                                </div>

                                <div class="accordion" id="accordionFaq">

                                    <!-- Primeros pasos -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-2 mb-3">
                                            Primeros pasos
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="primeros-pasos">
                                            <h2 class="accordion-header" id="faqHeading1">
                                                <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse1" aria-expanded="true" aria-controls="faqCollapse1">
                                                    ¿Qué debo revisar al ingresar por primera vez?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse1" class="accordion-collapse collapse show" aria-labelledby="faqHeading1" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Revisa que tu nombre de usuario sea correcto, que tu perfil corresponda a tus funciones y que el menú muestre los módulos que necesitas utilizar. Si falta alguna opción, solicita la revisión de permisos al administrador del sistema.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="primeros-pasos">
                                            <h2 class="accordion-header" id="faqHeading2">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse2" aria-expanded="false" aria-controls="faqCollapse2">
                                                    ¿Cuál es el flujo básico recomendado para operar el sistema?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse2" class="accordion-collapse collapse" aria-labelledby="faqHeading2" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Primero se mantienen usuarios, perfiles y permisos. Luego se configuran las reglas de captura y se asigna una regla a cada perfil. Después se registran o cargan las visitas y se mantienen los dispositivos autorizados. Finalmente, los movimientos que registra la aplicación móvil se consultan en Registro visitas.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="primeros-pasos">
                                            <h2 class="accordion-header" id="faqHeading3">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse3" aria-expanded="false" aria-controls="faqCollapse3">
                                                    ¿Qué debo preparar antes de usar la aplicación móvil en una portería?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse3" class="accordion-collapse collapse" aria-labelledby="faqHeading3" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Verifica que el perfil del usuario tenga asignada la regla correcta (qué datos se piden al registrar un movimiento y si se usa lista blanca o negra), que el dispositivo esté registrado y que las visitas necesarias estén cargadas y vigentes. La aplicación descarga las reglas y la lista de visitas al sincronizar.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Acceso y permisos -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Acceso y permisos
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="acceso">
                                            <h2 class="accordion-header" id="faqHeading4">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse4" aria-expanded="false" aria-controls="faqCollapse4">
                                                    ¿Qué hago si no puedo ingresar al sistema?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse4" class="accordion-collapse collapse" aria-labelledby="faqHeading4" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Verifica que tu usuario y contraseña sean correctos. Si el problema continúa, contacta a soporte indicando tu empresa, usuario y el mensaje que aparece en pantalla.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="acceso">
                                            <h2 class="accordion-header" id="faqHeading5">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse5" aria-expanded="false" aria-controls="faqCollapse5">
                                                    ¿Por qué no veo una opción del menú?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse5" class="accordion-collapse collapse" aria-labelledby="faqHeading5" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Las opciones del menú dependen de los permisos asignados a tu perfil. Si necesitas acceso a una opción adicional, solicita la revisión al administrador del sistema.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="acceso">
                                            <h2 class="accordion-header" id="faqHeading6">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse6" aria-expanded="false" aria-controls="faqCollapse6">
                                                    ¿Por qué no aparece el botón Nuevo?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse6" class="accordion-collapse collapse" aria-labelledby="faqHeading6" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    El botón Nuevo se muestra solo cuando tu perfil tiene permiso para registrar nuevos elementos en el módulo correspondiente.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="acceso">
                                            <h2 class="accordion-header" id="faqHeading7">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse7" aria-expanded="false" aria-controls="faqCollapse7">
                                                    ¿Por qué no puedo editar un registro?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse7" class="accordion-collapse collapse" aria-labelledby="faqHeading7" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    La edición depende de los permisos del perfil y de las reglas propias de cada módulo. En algunos casos, aunque tengas permiso de modificación, el registro puede estar bloqueado por su estado u otra condición de negocio, por ejemplo una regla asignada a perfiles vigentes.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="acceso">
                                            <h2 class="accordion-header" id="faqHeading8">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse8" aria-expanded="false" aria-controls="faqCollapse8">
                                                    ¿Qué significan los permisos de módulo?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse8" class="accordion-collapse collapse" aria-labelledby="faqHeading8" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    El permiso de visualización permite acceder al módulo. El permiso de nuevo permite registrar nuevos elementos. El permiso de modificación permite editar registros o ejecutar acciones que cambian información.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Usuarios -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Usuarios
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="usuarios">
                                            <h2 class="accordion-header" id="faqHeading9">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse9" aria-expanded="false" aria-controls="faqCollapse9">
                                                    ¿Cómo creo un usuario?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse9" class="accordion-collapse collapse" aria-labelledby="faqHeading9" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Ingresa al módulo Usuarios, presiona Nuevo, completa los datos obligatorios del usuario, asigna un perfil, define su contraseña y registra. Si el botón Nuevo no aparece, tu perfil no tiene permiso para crear usuarios.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="usuarios">
                                            <h2 class="accordion-header" id="faqHeading10">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse10" aria-expanded="false" aria-controls="faqCollapse10">
                                                    ¿Qué datos son importantes al crear un usuario?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse10" class="accordion-collapse collapse" aria-labelledby="faqHeading10" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Revisa especialmente el RUT, nombre, apellido, correo, teléfono, perfil asignado y contraseña. El perfil define qué opciones podrá ver y qué acciones podrá realizar.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="usuarios">
                                            <h2 class="accordion-header" id="faqHeading11">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse11" aria-expanded="false" aria-controls="faqCollapse11">
                                                    ¿Cómo cambio el perfil de un usuario?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse11" class="accordion-collapse collapse" aria-labelledby="faqHeading11" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Ingresa al módulo Usuarios, abre la edición del usuario, selecciona el nuevo perfil y presiona Actualizar. El cambio aplicará los permisos asociados al perfil seleccionado.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="usuarios">
                                            <h2 class="accordion-header" id="faqHeading12">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse12" aria-expanded="false" aria-controls="faqCollapse12">
                                                    ¿Por qué un usuario no puede ingresar?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse12" class="accordion-collapse collapse" aria-labelledby="faqHeading12" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Puede deberse a credenciales incorrectas, usuario no vigente, perfil sin permisos, empresa incorrecta o un problema de sesión. Revisa el estado del usuario y el perfil asignado antes de contactar a soporte.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="usuarios">
                                            <h2 class="accordion-header" id="faqHeading13">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse13" aria-expanded="false" aria-controls="faqCollapse13">
                                                    ¿Puedo cambiar mis propios datos?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse13" class="accordion-collapse collapse" aria-labelledby="faqHeading13" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Si la opción Mis datos está disponible, puedes actualizar la información permitida por el sistema. Algunas opciones, como perfil o vigencia, solo pueden ser modificadas por un administrador autorizado.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Perfiles -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Perfiles y permisos
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="perfiles">
                                            <h2 class="accordion-header" id="faqHeading14">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse14" aria-expanded="false" aria-controls="faqCollapse14">
                                                    ¿Para qué sirve un perfil?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse14" class="accordion-collapse collapse" aria-labelledby="faqHeading14" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Un perfil agrupa permisos de acceso para uno o más usuarios. Permite definir qué módulos puede ver cada usuario y qué acciones puede realizar dentro de la plataforma.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="perfiles">
                                            <h2 class="accordion-header" id="faqHeading15">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse15" aria-expanded="false" aria-controls="faqCollapse15">
                                                    ¿Cuál es la diferencia entre Perfiles y Módulos x Perfil?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse15" class="accordion-collapse collapse" aria-labelledby="faqHeading15" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Perfiles permite crear o editar perfiles. Módulos x Perfil permite definir qué módulos puede usar cada perfil y si puede ver, crear o modificar información.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="perfiles">
                                            <h2 class="accordion-header" id="faqHeading16">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse16" aria-expanded="false" aria-controls="faqCollapse16">
                                                    ¿Qué debo revisar si un usuario dice que le falta una opción?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse16" class="accordion-collapse collapse" aria-labelledby="faqHeading16" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Revisa qué perfil tiene asignado el usuario y luego valida en Módulos x Perfil si ese perfil tiene habilitado el módulo correspondiente. También verifica si necesita permiso para crear o modificar.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Visitas -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Visitas
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="visitas">
                                            <h2 class="accordion-header" id="faqHeading101">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse101" aria-expanded="false" aria-controls="faqCollapse101">
                                                    ¿Para qué sirve el módulo Visitas?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse101" class="accordion-collapse collapse" aria-labelledby="faqHeading101" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Visitas mantiene el listado de personas registradas para la empresa (RUT, nombre y empresa). La aplicación móvil descarga las visitas vigentes y, según la regla del perfil, las usa como lista blanca o lista negra al registrar un movimiento.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="visitas">
                                            <h2 class="accordion-header" id="faqHeading102">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse102" aria-expanded="false" aria-controls="faqCollapse102">
                                                    ¿Cómo registro una visita?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse102" class="accordion-collapse collapse" aria-labelledby="faqHeading102" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Ingresa a Visitas, presiona Nuevo, ingresa el RUT, el nombre y, si corresponde, la empresa, y presiona Registrar. El RUT se guarda siempre con el formato 12345678-9, aunque se escriba con puntos. Si el RUT ya existe para la empresa, el sistema lo indica y no crea un duplicado.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="visitas">
                                            <h2 class="accordion-header" id="faqHeading103">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse103" aria-expanded="false" aria-controls="faqCollapse103">
                                                    ¿Puedo cambiar el RUT de una visita?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse103" class="accordion-collapse collapse" aria-labelledby="faqHeading103" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    No. El RUT identifica a la visita y no se modifica. Desde Editar puedes actualizar el nombre, la empresa y el estado. Si el RUT fue mal ingresado, anula esa visita y registra una nueva con el RUT correcto.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="visitas">
                                            <h2 class="accordion-header" id="faqHeading104">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse104" aria-expanded="false" aria-controls="faqCollapse104">
                                                    ¿Qué significa que una visita esté Vigente o Anulada?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse104" class="accordion-collapse collapse" aria-labelledby="faqHeading104" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Solo las visitas vigentes se envían a la aplicación móvil. Con lista blanca, solo los RUT vigentes del listado pueden registrarse; con lista negra, los RUT vigentes del listado son rechazados. Una visita anulada deja de considerarse en ambos casos.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="visitas">
                                            <h2 class="accordion-header" id="faqHeading105">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse105" aria-expanded="false" aria-controls="faqCollapse105">
                                                    ¿Dónde veo quién modificó una visita?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse105" class="accordion-collapse collapse" aria-labelledby="faqHeading105" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    En Editar, presiona Ver bitácora. Ahí se registran la creación y cada cambio de nombre, empresa o estado, con la fecha, la hora y el usuario que lo realizó.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Cargar visitas -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Cargar visitas
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="cargas">
                                            <h2 class="accordion-header" id="faqHeading106">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse106" aria-expanded="false" aria-controls="faqCollapse106">
                                                    ¿Cómo cargo visitas de forma masiva?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse106" class="accordion-collapse collapse" aria-labelledby="faqHeading106" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Ingresa a Cargar visitas, presiona Nuevo, selecciona un archivo CSV y procésalo. El archivo debe tener encabezado con las columnas RUT, nombre y empresa, separadas por punto y coma o por coma. Se aceptan archivos guardados desde Excel.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="cargas">
                                            <h2 class="accordion-header" id="faqHeading107">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse107" aria-expanded="false" aria-controls="faqCollapse107">
                                                    ¿Qué pasa si el RUT ya existe?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse107" class="accordion-collapse collapse" aria-labelledby="faqHeading107" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    La visita se actualiza con el nombre y la empresa del archivo y conserva su estado actual: una visita anulada no se reactiva con la carga. Si los datos son iguales a los registrados, la fila queda como Sin cambios. La carga nunca elimina visitas.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="cargas">
                                            <h2 class="accordion-header" id="faqHeading108">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse108" aria-expanded="false" aria-controls="faqCollapse108">
                                                    ¿Qué pasa si algunas filas tienen errores?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse108" class="accordion-collapse collapse" aria-labelledby="faqHeading108" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    La carga es parcial: las filas válidas se aplican y las filas con error se informan en el detalle de la carga con su número y el motivo, por ejemplo RUT inválido, nombre vacío o RUT repetido dentro del mismo archivo. Corrige solo esas filas y vuelve a cargarlas.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="cargas">
                                            <h2 class="accordion-header" id="faqHeading109">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse109" aria-expanded="false" aria-controls="faqCollapse109">
                                                    ¿Dónde reviso el resultado de una carga?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse109" class="accordion-collapse collapse" aria-labelledby="faqHeading109" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    En Cargar visitas, abre el detalle de la carga. Cada fila indica si fue Insertada, Actualizada, Sin cambios o Error. Puedes filtrar por resultado y buscar por RUT, nombre, empresa o mensaje.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Registro visitas -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Registro visitas
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="registro">
                                            <h2 class="accordion-header" id="faqHeading110">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse110" aria-expanded="false" aria-controls="faqCollapse110">
                                                    ¿Qué muestra Registro visitas?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse110" class="accordion-collapse collapse" aria-labelledby="faqHeading110" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Muestra los movimientos de entrada y salida registrados desde la aplicación móvil, con RUT, nombre, empresa, patente, fecha y hora. Si la regla pide fotografía, puedes ver la evidencia desde la columna Evidencia.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="registro">
                                            <h2 class="accordion-header" id="faqHeading111">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse111" aria-expanded="false" aria-controls="faqCollapse111">
                                                    ¿Por qué no veo movimientos recientes?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse111" class="accordion-collapse collapse" aria-labelledby="faqHeading111" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Por defecto se muestra el último año. Revisa el rango de fechas y los filtros de operación y coincidencia. Los movimientos aparecen una vez que la aplicación móvil los envía, por lo que un dispositivo sin conexión puede demorar en informarlos.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="registro">
                                            <h2 class="accordion-header" id="faqHeading112">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse112" aria-expanded="false" aria-controls="faqCollapse112">
                                                    ¿Puedo exportar los movimientos?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse112" class="accordion-collapse collapse" aria-labelledby="faqHeading112" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Sí. Presiona Excel para descargar los movimientos según los filtros aplicados, incluyendo el comentario y los datos del dispositivo que registró cada movimiento.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="registro">
                                            <h2 class="accordion-header" id="faqHeading113">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse113" aria-expanded="false" aria-controls="faqCollapse113">
                                                    ¿Puedo modificar un movimiento registrado?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse113" class="accordion-collapse collapse" aria-labelledby="faqHeading113" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    No. Registro visitas es solo de consulta. Los movimientos reflejan lo registrado en portería; si un dato de la visita es incorrecto, corrígelo en el módulo Visitas para los próximos registros.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Reglas -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Reglas
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="reglas">
                                            <h2 class="accordion-header" id="faqHeading114">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse114" aria-expanded="false" aria-controls="faqCollapse114">
                                                    ¿Para qué sirven las reglas?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse114" class="accordion-collapse collapse" aria-labelledby="faqHeading114" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Una regla define cómo trabaja la aplicación móvil al registrar un movimiento: si pide nombre, empresa, patente, comentario o fotografía, si se debe indicar entrada o salida, si se usa lista blanca o negra y si se permite eliminar los registros ya enviados. Cada perfil tiene asignada una regla.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="reglas">
                                            <h2 class="accordion-header" id="faqHeading115">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse115" aria-expanded="false" aria-controls="faqCollapse115">
                                                    ¿Cuál es la diferencia entre lista blanca y lista negra?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse115" class="accordion-collapse collapse" aria-labelledby="faqHeading115" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Con lista blanca, la aplicación solo permite registrar RUT que estén vigentes en Visitas. Con lista negra, rechaza los RUT vigentes en Visitas y permite el resto. Con Ninguna, no se valida contra el listado de visitas.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="reglas">
                                            <h2 class="accordion-header" id="faqHeading116">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse116" aria-expanded="false" aria-controls="faqCollapse116">
                                                    ¿Por qué no puedo anular una regla?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse116" class="accordion-collapse collapse" aria-labelledby="faqHeading116" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Una regla no se puede anular mientras esté asignada a uno o más perfiles vigentes. Primero asigna otra regla a esos perfiles o anúlalos, y luego anula la regla.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="reglas">
                                            <h2 class="accordion-header" id="faqHeading117">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse117" aria-expanded="false" aria-controls="faqCollapse117">
                                                    ¿Cuándo se aplican los cambios de una regla en la aplicación?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse117" class="accordion-collapse collapse" aria-labelledby="faqHeading117" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    La aplicación móvil descarga las reglas al sincronizar. Después de modificar una regla, sincroniza los dispositivos para que trabajen con la configuración actualizada. Los cambios quedan registrados en la bitácora de la regla.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Problemas frecuentes -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Problemas frecuentes
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="problemas">
                                            <h2 class="accordion-header" id="faqHeading40">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse40" aria-expanded="false" aria-controls="faqCollapse40">
                                                    ¿Qué hago si aparece un mensaje de validación?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse40" class="accordion-collapse collapse" aria-labelledby="faqHeading40" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Lee el mensaje completo y revisa los campos indicados. Normalmente estos mensajes se producen por datos obligatorios faltantes, formatos incorrectos, fechas inválidas o reglas de negocio que impiden guardar.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="problemas">
                                            <h2 class="accordion-header" id="faqHeading41">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse41" aria-expanded="false" aria-controls="faqCollapse41">
                                                    ¿Qué hago si el sistema no guarda los cambios?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse41" class="accordion-collapse collapse" aria-labelledby="faqHeading41" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Revisa si apareció una alerta, valida que tengas permiso para modificar, confirma que los campos obligatorios estén completos y verifica que el registro no esté bloqueado por su estado u otra regla del módulo.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="problemas">
                                            <h2 class="accordion-header" id="faqHeading42">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse42" aria-expanded="false" aria-controls="faqCollapse42">
                                                    ¿Qué hago si una opción se queda cargando?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse42" class="accordion-collapse collapse" aria-labelledby="faqHeading42" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Espera unos segundos y, si no responde, recarga la página. Si vuelve a ocurrir, informa a soporte el módulo, acción realizada y hora aproximada del problema.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="problemas">
                                            <h2 class="accordion-header" id="faqHeading43">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse43" aria-expanded="false" aria-controls="faqCollapse43">
                                                    ¿Qué hago si mi sesión expira?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse43" class="accordion-collapse collapse" aria-labelledby="faqHeading43" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Debes ingresar nuevamente al sistema. Si estabas editando información, revisa si los cambios quedaron guardados antes de repetir la operación.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Soporte -->
                                    <div class="faq-section">
                                        <h6 class="text-primary fw-semibold mt-4 mb-3">
                                            Soporte
                                        </h6>

                                        <div class="accordion-item faq-item" data-categoria="soporte">
                                            <h2 class="accordion-header" id="faqHeading44">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse44" aria-expanded="false" aria-controls="faqCollapse44">
                                                    ¿Cómo contacto a soporte?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse44" class="accordion-collapse collapse" aria-labelledby="faqHeading44" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Puedes usar la opción Contacto soporte disponible en el menú lateral. Para agilizar la atención, indica tu empresa, usuario, módulo afectado y una breve descripción del problema.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="soporte">
                                            <h2 class="accordion-header" id="faqHeading45">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse45" aria-expanded="false" aria-controls="faqCollapse45">
                                                    ¿Qué información debo enviar a soporte?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse45" class="accordion-collapse collapse" aria-labelledby="faqHeading45" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Indica tu empresa, usuario, módulo afectado, acción que estabas realizando, mensaje mostrado por el sistema y, si corresponde, adjunta el archivo utilizado o una captura de pantalla del problema.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="soporte">
                                            <h2 class="accordion-header" id="faqHeading46">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse46" aria-expanded="false" aria-controls="faqCollapse46">
                                                    ¿Qué debo hacer antes de reportar un error?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse46" class="accordion-collapse collapse" aria-labelledby="faqHeading46" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Intenta repetir la acción, revisa si el mensaje indica un dato faltante o inválido, verifica tu conexión y confirma que tienes permisos para la operación. Luego informa a soporte los pasos realizados.
                                                </div>
                                            </div>
                                        </div>

                                        <div class="accordion-item faq-item" data-categoria="soporte">
                                            <h2 class="accordion-header" id="faqHeading47">
                                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faqCollapse47" aria-expanded="false" aria-controls="faqCollapse47">
                                                    ¿Dónde reviso errores técnicos o eventos del sistema?
                                                </button>
                                            </h2>
                                            <div id="faqCollapse47" class="accordion-collapse collapse" aria-labelledby="faqHeading47" data-bs-parent="#accordionFaq">
                                                <div class="accordion-body">
                                                    Si tienes acceso al módulo Logs, puedes revisar registros técnicos o eventos relevantes del sistema. Esta información puede servir como apoyo para el diagnóstico de problemas.
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <!-- End FAQ -->

            </div>

        </div>
    </div>
    <!-- End Main Content-->

</asp:Content>
