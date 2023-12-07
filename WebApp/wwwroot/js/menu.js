// menu.js

import { makeRequest } from './utils/make-request.js';

function loadProducts() {
    console.log("Loading products...");
    makeRequest("/MenuBoard/getProducts", "GET", null, function(response) {
        $(".product-container").html(response);
        document.dispatchEvent(new Event("HideLoadingScreen"));
    }, null);
}

document.addEventListener("DOMContentLoaded", function () {
    document.dispatchEvent(new Event("HideLoadingScreen"));
    // Load default data
    loadProducts();

    // Attach a click event handler to an element with the class 'load-products'
    $('.load-products').click(function () {
        // Call the loadProducts function when this element is clicked
        loadProducts();
    });
});
