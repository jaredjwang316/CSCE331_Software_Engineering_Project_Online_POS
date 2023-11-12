/*
    File: cart.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
*/

document.addEventListener("DOMContentLoaded", function() {
    $(document).on('click', '.clear-cart-btn', function() {
        console.log("Clearing cart")
        $.ajax({
            url: "/Cart/Clear",
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
            url: "/Cart/AddItem",
            type: "POST",
            data: { product_id: productID, customization_ids: customizationIDs, quantity: 1 },
            success: function (data) {
                console.log("Added to cart");
            },
            error: function () {
                console.log("Error adding to cart");
            }
        });

        // Go back to the index page
        location.reload();
    });

    // Remove from cart button
    $(document).on('click', '.remove-item-btn', function() {
        console.log("Removing from cart");
        var productId = $(this).attr("id");

        $.ajax({
            url: "/Cart/RemoveItem",
            type: "POST",
            data: { product_id: productId },
            error: function () {
                console.log("Error removing from cart");
            }
        });

        // Go back to the index page
        location.reload();
    });
});
