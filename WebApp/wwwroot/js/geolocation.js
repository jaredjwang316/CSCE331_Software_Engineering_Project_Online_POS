/*
    File: wwwroot/js/geolocation.js
    Author: Griffin Beaudreau
    Date: November 26, 2023
    Purpose: Geolocation functions
*/

if (navigator.geolocation && !hasCookie("latitude") && !hasCookie("longitude")) {
    navigator.geolocation.getCurrentPosition(saveAsCookie, error);
}

function saveAsCookie(position) {
    document.cookie = "latitude=" + position.coords.latitude;
    document.cookie = "longitude=" + position.coords.longitude;
}

function error(err) {
    console.warn(`ERROR(${err.code}): ${err.message}`);
}

function hasCookie(name) {
    return document.cookie.split(';').some((item) => item.trim().startsWith(name + '='));
}

function getCookie(name) {
    var cookie = document.cookie.split(';').find((item) => item.trim().startsWith(name + '='));
    if (cookie) {
        return cookie.split("=")[1];
    }
    return null;
}
