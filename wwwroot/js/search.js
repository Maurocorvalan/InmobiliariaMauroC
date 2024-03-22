$(document).ready(function () {
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $('#searchButton').click(function () {
        var value = $('#searchInput').val().toLowerCase();
        $('tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});