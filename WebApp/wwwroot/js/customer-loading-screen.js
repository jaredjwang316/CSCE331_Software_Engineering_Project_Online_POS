/*
    File: customer-loading-screen.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
*/

document.addEventListener("DOMContentLoaded", function () {
    // Loading spinner
    document.body.appendChild(document.createElement("div")).setAttribute("class", "spinner-border");

    // Opaque loading screen
    document.body.appendChild(document.createElement("div")).setAttribute("id", "loading-screen");
    document.getElementById("loading-screen").setAttribute("style",
        "position: fixed;" +
        "top: 0; left: 0;" +
        "width: 100%; height: 100%;" +
        "background-color: rgba(0, 0, 0, 0.5);" +
        "z-index: 9999;"
    );

    document.addEventListener("DisplayLoadingScreen", function () {
        $("#loading-screen").show();
        $(".spinner-border").show();
        $(".item-container").hide();
    });

    document.addEventListener("HideLoadingScreen", function () {
        $("#loading-screen").hide();
        $(".spinner-border").hide();
    });
});
