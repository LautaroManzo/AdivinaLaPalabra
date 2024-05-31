
$(document).on("keypress", function (e) {

    let keyCode = e.keyCode || e.which;

    // Se hace esto para evitar el uso de letras con acento.
    // Si la tecla presionada está dentro del rango de letras del alfabeto inglés.
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 122))
        e.preventDefault();
});

$(document).on("keydown", function (e) {

    // 13 Tecla de Enter
    if (e.keyCode == 13)
        enter();

    // Se asigna evento de borrado a las teclas delete y suprimir
    if (e.keyCode === 8) {
        e.preventDefault();
        onDeleteClick();
    }

    // Desactiva la tecla TAB para que el user no pueda moverse entre los inputs.
    if (e.keyCode === 9)
        e.preventDefault();

    if (abcRegex.test(String.fromCharCode(e.keyCode))) {

        // Esto es para reemplazar el valor si el input ya contiene
        if (inputFocus.val() != "") {
            inputFocus.val(e.key.toUpperCase());
            inputFocus.removeClass("foco");
            inputFocus.blur();
            inputFocus = inputFocus.parent().next('div').find('input');       
            inputFocus.addClass("foco");
            return;
        }

    }
    else {
        e.preventDefault();
    }

    inputFocus.focus();
});

$(document).on('input', '.divWordle > div:not(.div-disable) input', function (e) {
    // Avanza de input cada vez que se modifica el input
    inputFocus.parent().next('div').find('input').focus();
});

// funcion para manejar el borrado en inputs
function onDeleteClick() {

    // Borro el valor del input
    if (inputFocus.val() != "")
        inputFocus.val("");

    // Mantiene el foco si el input a borrar es el primero 
    if (!filaFocus.find('input').first().is(inputFocus)) {
        inputFocus.removeClass("foco");
        inputFocus.parent().prev('div').find('input').focus();    // Busco el input previo al borrado para pasarle el focus.
    }
}

// Función serializeObject
$.fn.serializeObject = function () {
    var obj = {};
    var arr = this.serializeArray();
    $.each(arr, function () {
        if (obj[this.name] !== undefined) {
            if (!obj[this.name].push) {
                obj[this.name] = [obj[this.name]];
            }
            obj[this.name].push(this.value || '');
        } else {
            obj[this.name] = this.value || '';
        }
    });
    return obj;
};

// Funcion para mostrar un mensaje de error/info..
function ShowMessage(message, color) {

    if ($("#message").length)
        return;

    $(".divConteiener").prepend(`<div class="div-messages"><div id="message" class="alert alert-${color}" role="alert">${message}</div></div>`);

    $("#message").fadeIn(500);

    setTimeout(function () {
        $("#message").fadeOut(1000, function () {
            $(this).parent().remove();
        });
    }, 3000);
}

function changeMode() {



}

// Evento que se ejecuta al abrir el modal
$('#resultModal').on('show.bs.modal', function () {

    if ($('#resultModal').hasClass("Win"))
        showConfetis();

});

// Evento que se ejecuta al cerrar el modal
$('#resultModal').on('hidden.bs.modal', function () {
    $('#resultModal').removeClass('modal-center');
});

function showConfetis() {

    var count = 200;

    var defaults = {
        origin: { y: 0.7 }
    };

    function fire(particleRatio, opts) {
        confetti({
            ...defaults,
            ...opts,
            particleCount: Math.floor(count * particleRatio),
            zIndex: 9999
        });
    }

    fire(0.25, {
        spread: 26,
        startVelocity: 55,
    });

    fire(0.2, {
        spread: 60,
    });

    fire(0.35, {
        spread: 100,
        decay: 0.91,
        scalar: 0.8
    });

    fire(0.1, {
        spread: 120,
        startVelocity: 25,
        decay: 0.92,
        scalar: 1.2
    });

    fire(0.1, {
        spread: 120,
        startVelocity: 45,
    });
}
