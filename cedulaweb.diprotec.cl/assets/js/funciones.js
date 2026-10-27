function fncOpenWindow(tipo, nombrePdf, width, height, name) {
    var screenLeft = 0;

    if (!name) name = "PDF";
    if (!width) width = 600;
    if (!height) height = 600;

    if (typeof window.screenLeft !== "undefined") {
        screenLeft = window.screenLeft;
    }
    else if (typeof window.screenX !== "undefined") {
        screenLeft = window.screenX;
    }
    var featuresDict = {
        toolbar: "no",
        location: "no",
        directories: "no",
        left: screenLeft + ($(window).width() - width) / 2,
        top: 100,
        status: "no",
        menubar: "no",
        scrollbars: "no",
        resizable: "no",
        width: width,
        height: height
    };

    var featuresArr = [];
    for (var k in featuresDict) {
        if (featuresDict.hasOwnProperty(k)) {
            featuresArr.push(k + "=" + featuresDict[k]);
        }
    }
    var featuresStr = featuresArr.join(",");

    var pdf = nombrePdf.split(".");
    var rutaPdf = "/asp/view.asp?tipo=" + tipo + "&pdf=" + pdf[0];

    window.open(rutaPdf, name, featuresStr);
}

function fncScrollWinTop() {
    $("html,body").animate(
        { scrollTop: 0 },
        250
    );
}

function GetNombreDia(numeroDia) {
    var dias = ["Domingo", "Lunes", "Martes", "Mi\xe9rcoles", "Jueves", "Viernes", "S\xe1bado", "Domingo"];
    var nombreDia = dias[numeroDia];
    return nombreDia;
}

function GetNombreMes(numeroMes) {
    var meses = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
    var nombreMes = meses[numeroMes];
    return nombreMes;
}

function ValidateEmail(email, expresionRegular) {
    var expr = new RegExp(expresionRegular, "i");

    if (!expr.test(email)) {
        return false;
    }
    else {
        return true;
    }
}

function ValidateRut(rut) {
    var run = rut.substring(0, rut.length - 1);
    var dv = rut.substring(rut.length - 1, rut.length);

    var suma = 0;
    var multiplicador = 2;

    for (var i = run.length - 1; i >= 0; i--) {
        suma += run.charAt(i) * multiplicador;
        if (multiplicador === 7)
            multiplicador = 2;
        else
            multiplicador++;
    }

    var resto = suma % 11;
    if (resto === 1)
        var dvRut = "k";
    else if (resto === 0)
        dvRut = "0";
    else {
        var dvi = 11 - resto;
        dvRut = dvi + "";
    }

    if (dvRut !== dv.toLowerCase()) {
        return false;
    }
    else {
        return true;
    }
}

function mostrarAlerta(tipo, titulo, mensaje) {
    var claseAlerta = "alert-solid-danger";

    if (tipo === "success") {
        claseAlerta = "alert-solid-success";
    }
    else if (tipo === "info") {
        claseAlerta = "alert-solid-info";
    }
    else if (tipo === "warning") {
        claseAlerta = "alert-solid-warning";
    }
    else if (tipo === "danger") {
        claseAlerta = "alert-solid-danger";
    }

    var html = ''
        + '<div id="alertaMensaje" '
        + 'class="alert ' + claseAlerta + ' shadow mb-0" '
        + 'role="alert" '
        + 'style="display:none; min-width:320px; max-width:500px; position:relative; right:-420px;">'
        + '    <strong>' + titulo + '</strong> ' + mensaje
        + '</div>';

    $("#divAlerta").html(html);

    $("#alertaMensaje")
        .show()
        .animate(
            {
                right: "0px"
            },
            350
        );

    setTimeout(function () {
        ocultarAlerta();
    }, 5000);
}

function ocultarAlerta() {
    var alerta = $("#alertaMensaje");

    if (alerta.length === 0) {
        return;
    }

    alerta.animate(
        {
            right: "-420px"
        },
        350,
        function () {
            $(this).remove();
        }
    );
}

$(document).on("click", ".main-toggle", function () {
    var toggle = $(this);
    var hiddenId = toggle.attr("data-hidden-id");
    var hidden = document.getElementById(hiddenId);
    var onValue = toggle.attr("data-on-value");
    var offValue = toggle.attr("data-off-value");

    if (!hidden) {
        return;
    }

    toggle.toggleClass("on");

    // La convención histórica del sitio es On = 0 / Off = 1.
    // Los toggles que representan indicadores booleanos pueden declararla
    // explícitamente con data-on-value="1" y data-off-value="0".
    hidden.value = toggle.hasClass("on")
        ? (onValue === undefined ? "0" : onValue)
        : (offValue === undefined ? "1" : offValue);
});
