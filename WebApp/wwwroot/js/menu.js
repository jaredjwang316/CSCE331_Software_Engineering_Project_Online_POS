/*
    File: wwwroot/js/cart.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
    Purpose: Contains all javascript functions for the menu page.
*/

document.addEventListener("DOMContentLoaded", function () {
    // Load default data
    loadProducts();

    // Function to load products
    function loadProducts() {
        // Make an AJAX GET request to the getProducts action
        $.get('/MenuBoard/getProducts', function (data) {
            // Assuming there is an element with a class 'product-container'
            // Replace the content of this element with the data received from the action
            $('.product-container').html(data);
            document.dispatchEvent(new Event("HideLoadingScreen"));
        });
    }

    // Attach a click event handler to an element with the class 'load-products'
    $('.load-products').click(function () {
        // Call the loadProducts function when this element is clicked
        loadProducts();
    });
});
