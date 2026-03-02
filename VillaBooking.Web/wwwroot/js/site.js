// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    setTimeout(function () {
        document.querySelectorAll('.auto-hide').forEach(function (alert) {
            alert.style.transition = "opacity 0.5s";
            alert.style.opacity = "0";

            setTimeout(function () {
                alert.remove();
            }, 500);
        });
    }, 3000);
});