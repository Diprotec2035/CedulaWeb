(function ($) {
    "use strict";

    function esTeclaControl(evento) {
        var codigo = evento.which || evento.keyCode || 0;

        if (evento.ctrlKey || evento.metaKey || evento.altKey) {
            return true;
        }

        return codigo === 0 || codigo === 8 || codigo === 9 || codigo === 13 || codigo === 27;
    }

    function esCaracterPermitido(caracter, permitidos, permitidosEnMinuscula) {
        return permitidos.indexOf(caracter) >= 0 || permitidosEnMinuscula.indexOf(caracter.toLowerCase()) >= 0;
    }

    function sanitizarTexto(texto, permitidos, permitidosEnMinuscula) {
        var resultado = "";

        for (var indice = 0; indice < texto.length; indice++) {
            var caracter = texto.charAt(indice);
            if (esCaracterPermitido(caracter, permitidos, permitidosEnMinuscula)) {
                resultado += caracter;
            }
        }

        return resultado;
    }

    function aplicarSanitizacion(control, permitidos, permitidosEnMinuscula) {
        var original = control.value || "";
        var inicio = typeof control.selectionStart === "number" ? control.selectionStart : original.length;
        var fin = typeof control.selectionEnd === "number" ? control.selectionEnd : inicio;
        var sanitizado = sanitizarTexto(original, permitidos, permitidosEnMinuscula);

        if (original === sanitizado) {
            return;
        }

        var inicioSanitizado = sanitizarTexto(original.substring(0, inicio), permitidos, permitidosEnMinuscula).length;
        var finSanitizado = sanitizarTexto(original.substring(0, fin), permitidos, permitidosEnMinuscula).length;
        control.value = sanitizado;

        if (typeof control.setSelectionRange === "function") {
            control.setSelectionRange(inicioSanitizado, finSanitizado);
        }
    }

    $.fn.validText = function (permitidos) {
        if (typeof permitidos !== "string") {
            return this;
        }

        var permitidosEnMinuscula = permitidos.toLowerCase();

        return this.each(function () {
            var control = this;
            var $control = $(control);
            var estaComponiendo = false;

            $control.off(".validText");
            $control.on("compositionstart.validText", function () { estaComponiendo = true; });
            $control.on("compositionend.validText", function () {
                estaComponiendo = false;
                aplicarSanitizacion(control, permitidos, permitidosEnMinuscula);
            });
            $control.on("keypress.validText", function (evento) {
                if (estaComponiendo || esTeclaControl(evento)) {
                    return;
                }

                var caracter = String.fromCharCode(evento.which || evento.keyCode || 0);
                if (!esCaracterPermitido(caracter, permitidos, permitidosEnMinuscula)) {
                    evento.preventDefault();
                }
            });
            $control.on("input.validText", function () {
                if (!estaComponiendo) {
                    aplicarSanitizacion(control, permitidos, permitidosEnMinuscula);
                }
            });
        });
    };
})(jQuery);
