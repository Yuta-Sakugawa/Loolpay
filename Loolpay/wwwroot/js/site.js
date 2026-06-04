// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleFavorite(storeId, buttonElement) {
    $.post('/Favorites/Toggle', { storeId: storeId }, function (data) {
        if (data.success) {
            if (data.isFavorite) {
                $(buttonElement).find('i').removeClass('bi-heart').addClass('bi-heart-fill text-danger');
            } else {
                $(buttonElement).find('i').removeClass('bi-heart-fill text-danger').addClass('bi-heart');
            }
        }
    });
}
