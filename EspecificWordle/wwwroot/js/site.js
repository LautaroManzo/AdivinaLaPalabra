
var data;
let inputFocus;
let filaFocus;

// Expresión regular para letras de la A-Z (mayúsculas y minúsculas)
const abcRegex = /^[a-zA-Z]+$/;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/refreshHub")
    .configureLogging(signalR.LogLevel.None)
    .build();

connection.on("RefreshPage", function () {
    location.reload();
});

connection.start().catch(function (err) {
    console.error(err.toString());
});

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

$(document).ready(function () {

    setInterval(() => (updateCountdown(), updateCountdown()), 1000);

    // Guardo en todo momento el input que tiene focus
    $(document).on('focus', '.div-wordle > div:not(.div-disable) input', function () {

        if (inputFocus)
            inputFocus.removeClass("foco");

        inputFocus = $(this);
        inputFocus.addClass("foco");
    });

    // Maneja keyboard app
    $(document).on("click", '.div-keyboard button', function () {
        handleKeyboard($("#GameFinish").val(), this);
    });

});

$(document).on("keypress", function (e) {

    let keyCode = e.keyCode || e.which;

    // Se hace esto para evitar el uso de letras con acento.
    // Si la tecla presionada está dentro del rango de letras del alfabeto inglés.
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 122))
        e.preventDefault();
});

$(document).on("keydown", function (e) {

    if ($(".loader").is(":hidden") && inputFocus) {

        // 13 Tecla de Enter
        if (e.keyCode == 13)
            enter();

        // Se asigna evento de borrado a las teclas delete y suprimir
        if (e.keyCode === 8 || e.keyCode === 46) {
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

                if (inputFocus.parent().next('div').find('input').length != 0) {
                    inputFocus.removeClass("foco");
                    inputFocus.blur();
                    inputFocus = inputFocus.parent().next('div').find('input');
                    inputFocus.addClass("foco");
                }

                return;
            }

        }
        else {
            e.preventDefault();
        }

        inputFocus.focus();
    }
});

$(document).on('input', '.div-wordle > div:not(.div-disable) input', function (e) {
    // Avanza de input cada vez que se modifica el input
    inputFocus.parent().next('div').find('input').focus();
});

$(document).on("touchstart", "input, button", function (event) {
    event.preventDefault();
});

function enableRowByAttempt(intento) {
    $('.div-wordle').children().each(function (index) {
        if (index === intento) {
            $(this).removeClass('div-disable');
            filaFocus = $(this);    // Foco de la fila
        }
    });
}

function obtenerFila(model, intento, length) {
    let palabraIngresada = "";
    for (let col = 0; col < length; col++) {
        let key = `${intento}_${col}`;
        palabraIngresada += model[key];
    }
    return palabraIngresada;
}

function handleKeyboard(gameFinish, clickedButton) {

    if (gameFinish == "False") {

        if ($(clickedButton).hasClass('delete')) {
            onDeleteClick();
            return;
        } else if ($(clickedButton).hasClass('enter'))
            return;

        inputFocus.val($(clickedButton).text());
        inputFocus.parent().next('div').find('input').focus();
    }

}

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

// Funcion para mostrar un mensaje de error/info..
function ShowMessage(message, color, animated = true, time = 3000) {

    if ($("#message").text() != "")
        return;

    $("#message").addClass(`alert alert-${color} mb-0`).text(message);

    $("#message").fadeIn(500);

    if (animated)
        filaFocus.addClass("animated-move"); // Se agrega la animación de movimiento lateral.

    setTimeout(function () {

        $("#message").fadeOut(1000, function () {
            $(this).text("");
        });

        if (animated)
            filaFocus.removeClass("animated-move");  // Se remueve la animación.

    }, time);
}

function addAnimation(divParent) {
    divParent.addClass("animated-flip");
    divParent.toggleClass('rotated');
}

function showPista(pistaDescripcion) {
    ShowMessage(pistaDescripcion, "success", false, 5000);
    $(".btn-pista").parent().remove();
}

function showButtonPista(pistaDescripcion) {
    let divM = $(".div-keyboard div button:contains('M')").parent();

    divM.after(`
        <div class='div-pista'>
                <button type='button' onclick="showPista('${pistaDescripcion}')" class='form-control form-control-lg letras btn-pista rounded-1 p-0'>
                <img src='/Icons/solution.png' alt='Pista'>
            </button>
        </div>
    `);

    setTimeout(function () {
        addAnimation($(".div-pista"));
    }, 200);
}

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

function showLoader() {
    $(".loader").show();
    $("body").css("pointer-events", "none");
}

function hideLoader() {
    $(".loader").hide();
    $("body form").css("pointer-events", "auto");
}

function showInstrucciones() {
    $.ajax({
        url: instruccionesUrl,
        method: 'GET',
        success: function (data) {

            let divModal = $("#instruccionesModal");
            divModal.attr('role', 'dialog');
            divModal.addClass('modal fade');

            divModal.append(data);

            document.getElementById('instruccionesModal').addEventListener('hidden.bs.modal', function () {
                $(".modal-dialog").remove();

                if (document.activeElement)
                    document.activeElement.blur();
            });

            divModal.modal('show');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error al realizar la solicitud AJAX:', errorThrown);
        }
    });
}

function sendEmail(body) {
    let email = "test@test.com";
    let subject = body ? "Reporte de error - Adiviná la palabra" : "Contacto - Adiviná la palabra";

    let mailtoURL = `mailto:${email}?subject=${encodeURIComponent(subject)}`;

    if (body)
        mailtoURL += `&body=${encodeURIComponent(body)}`;

    window.location.href = mailtoURL;
}

function updateCountdown() {
    const now = new Date();
    const midnight = new Date().setHours(24, 0, 0, 0);  // 00:00:00 del día siguiente

    const timeRemaining = midnight - now;

    const msInSecond = 1000;
    const msInMinute = msInSecond * 60;
    const msInHour = msInMinute * 60;

    const hours = Math.floor(timeRemaining / msInHour);
    const minutes = Math.floor((timeRemaining % msInHour) / msInMinute);
    const seconds = Math.floor((timeRemaining % msInMinute) / msInSecond);

    $('#time-remaining').text(
        `${String(hours).padStart(2, '0')} : ${String(minutes).padStart(2, '0')} : ${String(seconds).padStart(2, '0')}`
    );
}

function redirectByMode(modoDescripcion) {
    indexUrl = indexUrl.replace('__MODODESCRIPCION__', modoDescripcion);
    window.location.href = indexUrl;
}

function modalResult(result, intento, modoId, modoDescripcion) {

    $.ajax({
        url: resultModalUrl,
        method: 'GET',
        data: { result: result, intento: intento, modoId: modoId, modoDescripcion: modoDescripcion },
        success: function (data) {

            if (!$(".show").length > 0) {
                let divModal = $("#resultModal");
                divModal.attr('role', 'dialog');
                divModal.addClass('modal fade');

                divModal.append(data);

                $('#resultModal').one('show.bs.modal', function () {

                    if (result != 0)
                        showConfetis();

                    if ($(".btn-pista"))
                        $(".btn-pista").parent().remove();
                });

                document.getElementById('resultModal').addEventListener('hidden.bs.modal', function () {
                    $(".modal-dialog").remove();
                });

                divModal.modal('show');
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error al realizar la solicitud AJAX:', errorThrown);
        }
    });

}

function moveFocusAndEnableNext() {
    filaFocus.next("div").removeClass('div-disable'); // Se saca la class en el div siguiente
    filaFocus.addClass('div-disable');  // Se agrega la class en el div donde se ingresó la palabra
    filaFocus = filaFocus.next("div");  // Se cambia la fila que tiene el foco
    filaFocus.find("input").first().focus();  // Se da el foco al input de la fila siguiente
}

function loadPage(json, modoId, gameFinish) {
    let data = "";

    if (json) {
        data = JSON.parse(json);
        let intentos = data[modoId].length;

        $(".div-wordle").children("div").slice(0, intentos).each(function (index) {

            // Piso la variable en cada recorrido
            filaFocus = $(this);

            let dictionary = data[modoId];
            let obj = dictionary[index];
            let list = obj.Letters;

            // Elementos html
            let fila = $(this);
            let filaDivs = fila.children("div");

            filaDivs.each(function (indexInp) {

                // Actualiza valores del input
                let input = $(this).children("input");
                input.val(list[indexInp].Letra);

                // Actualiza el aspecto de inputs
                if (list[indexInp].Color == "Verde")
                    changeInputAndButton(indexInp, list[indexInp].Letra, "input-success", "button-success", undefined, undefined);
                else if (list[indexInp].Color == "Amarillo")
                    changeInputAndButton(indexInp, list[indexInp].Letra, "input-medium", "button-medium", undefined, undefined);
                else
                    changeInputAndButton(indexInp, list[indexInp].Letra, "input-gray", "button-gray", undefined, undefined);
            });

            if (gameFinish == "True") filaFocus.next().addClass('div-disable');
            else moveFocusAndEnableNext();  // Se hace foco en la fila que corresponde 
        });
    }
}
