/*
    File: cart.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
*/

document.addEventListener("DOMContentLoaded", function() {
    $(document).on('click', '.clear-cart-btn', function() {
        $.ajax({
            url: "/Customer/ClearCart",
            method: "POST",
            success: function(data) {
                    location.reload();
            },
            error: function() {
                alert("Error clearing cart!");
            }
        });
    });

    // Add to cart button
    $(document).on('click', '.add-to-cart-btn', function() {
        var productID = $(this).attr("id");
        var customizationIDs = [];

        $(".customization-btn.active").each(function() {
            customizationIDs.push($(this).attr("id"));
        });

        $.ajax({
            url: "/Customer/AddToCart",
            type: "POST",
            data: { productID: productID, customizationIDs: customizationIDs },
            success: function (data) {
                console.log(data);
            },
            error: function () {
                console.log("Error adding to cart");
            }
        });

        // Go back to the index page
        location.reload();
    });
});
