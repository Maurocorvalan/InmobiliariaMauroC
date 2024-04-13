// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
    function mostrarEditarFoto() {
        document.getElementById("editarFotoForm").style.display = "block";
        document.getElementById("editarDatosForm").style.display = "none";
        document.getElementById("cambiarContraseñaForm").style.display = "none";
    }

    function mostrarEditarDatos() {
        document.getElementById("editarFotoForm").style.display = "none";
        document.getElementById("editarDatosForm").style.display = "block";
        document.getElementById("cambiarContraseñaForm").style.display = "none";
    }

    function mostrarCambiarContraseña() {
        document.getElementById("editarFotoForm").style.display = "none";
        document.getElementById("editarDatosForm").style.display = "none";
        document.getElementById("cambiarContraseñaForm").style.display = "block";
    }

    document.getElementById("avatar").onchange = function (e) {
        var reader = new FileReader();
        reader.onload = function (event) {
            document.getElementById("preview").src = event.target.result;
        }
        reader.readAsDataURL(e.target.files[0]);
    };
