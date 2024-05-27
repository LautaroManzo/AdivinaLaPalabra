
// funcion para manejar el borrado en inputs
function onDeleteClick() {

    // Borro el valor del input
    inputFocus.val("");

    // Busco el input previo al borrado para pasarle el focus.
    inputFocus.parent().prev('div').find('input').focus();

    // Mantiene el foco si el input a borrar es el primero 
    if (filaFocus.find('input').first().is(inputFocus))
        inputFocus.focus();
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
