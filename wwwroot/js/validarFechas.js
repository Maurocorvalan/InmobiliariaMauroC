// Función de validación de fechas
document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form");

    forms.forEach(form => {
        form.addEventListener("submit", function (event) {
            const fechaInicioInput = form.querySelector("input[name='FechaInicio']");
            const fechaFinalizacionInput = form.querySelector("input[name='FechaFinalizacion']");

            if (fechaInicioInput && fechaFinalizacionInput) {
                const fechaInicio = new Date(fechaInicioInput.value);
                const fechaFinalizacion = new Date(fechaFinalizacionInput.value);

                if (fechaInicio > fechaFinalizacion) {
                    event.preventDefault();
                    alert("La fecha de inicio no puede ser posterior a la fecha de finalización.");
                }
            }
        });
    });
});
