/*
    Author: Griffin Beaudreau
    Date: November 29, 2023
*/

export function makeRequest(url, method, data, successCallback, errorCallback, retries = 3) {
    $.ajax({
        url: url,
        method: method,
        data: data,
        success: successCallback,
        error: function (jqXHR, textStatus, errorThrown) {
            if (retries > 0) {
                console.log("Error making request. Retrying...");
                makeRequest(url, method, data, successCallback, errorCallback, retries - 1);
            } else {
                errorCallback(jqXHR, textStatus, errorThrown);
            }
        }
    });
}
