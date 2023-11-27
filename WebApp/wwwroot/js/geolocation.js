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

// document.addEventListener("DOMContentLoaded", function() {
//     var weather = getWeather();

//     function getWeather() {
//         console.log("Getting weather");
//         var latitude = getCookie("latitude");
//         var longitude = getCookie("longitude");
//         var subscriptionKey = "tfRy4n7VajYe8wsQMttrAWDbJVkXyc1AI76MgZwUCPQ";
//         var url = `https://atlas.microsoft.com/weather/currentConditions/json?api-version=1.0&subscription-key=${subscriptionKey}&query=${latitude},${longitude}`;
//         var weather = null;
//         $.ajax({
//             url: url,
//             method: "GET",
//             async: false,
//             success: function (data) {
//                 weather = data;
//                 let temperature = data.results[0].temperature;
//                 let icon = data.results[0].iconCode;

//                 console.log(temperature.value + "°" + temperature.unit);
//                 console.log(icon);
//             },
//             error: function (data) {
//                 console.log(data);
//             }
//         });
//         return weather;
//     }
// });