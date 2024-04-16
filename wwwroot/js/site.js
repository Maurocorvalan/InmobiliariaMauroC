// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let fotoFormVisible = false;
let datosFormVisible = false;
let contraseñaFormVisible = false;

function mostrarEditarFoto() {
    if (fotoFormVisible) {
        document.getElementById("editarFotoForm").style.display = "none";
        fotoFormVisible = false;
    } else {
        document.getElementById("editarFotoForm").style.display = "block";
        document.getElementById("editarDatosForm").style.display = "none";
        document.getElementById("cambiarContraseñaForm").style.display = "none";
        fotoFormVisible = true;
        datosFormVisible = false;
        contraseñaFormVisible = false;
    }
}

function mostrarEditarDatos() {
    if (datosFormVisible) {
        document.getElementById("editarDatosForm").style.display = "none";
        datosFormVisible = false;
    } else {
        document.getElementById("editarDatosForm").style.display = "block";
        document.getElementById("editarFotoForm").style.display = "none";
        document.getElementById("cambiarContraseñaForm").style.display = "none";
        datosFormVisible = true;
        fotoFormVisible = false;
        contraseñaFormVisible = false;
    }
}

function mostrarCambiarContraseña() {
    if (contraseñaFormVisible) {
        document.getElementById("cambiarContraseñaForm").style.display = "none";
        contraseñaFormVisible = false;
    } else {
        document.getElementById("cambiarContraseñaForm").style.display = "block";
        document.getElementById("editarFotoForm").style.display = "none";
        document.getElementById("editarDatosForm").style.display = "none";
        contraseñaFormVisible = true;
        fotoFormVisible = false;
        datosFormVisible = false;
    }
}

document.getElementById("avatar").onchange = function (e) {
    var reader = new FileReader();
    reader.onload = function (event) {
        document.getElementById("preview").src = event.target.result;
    }
    reader.readAsDataURL(e.target.files[0]);
};

