
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
