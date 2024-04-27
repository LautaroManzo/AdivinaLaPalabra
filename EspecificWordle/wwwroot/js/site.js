
// funcion para manejar el borrado en inputs
function onDeleteClick() {

    // Borro el valor del input
    inputFocus.val("");

    // Busco el input previo al borrado para pasarle el focus.
    inputFocus.parent().prev('div').find('input').focus();

    // Mantiene el foco si el input a borrar es el primero 
    if ($('.divWordle > div:not(.div-disable)').find('input').first().is(inputFocus))
        inputFocus.focus();
}
