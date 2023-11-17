/*
    File: cart.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
*/


function MakeRequest(url, method, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        method: method,
        data: data,
        success: successCallback,
        error: errorCallback
    });
}

class Cart {
    items = [Item];
    total = 0;
}
class Item {
    product;
    customizations = [];
    quantity = 0;
    cost = 0;
}
var cart = new Cart();
function GetCartSuccess(data) {
    cart.items = data.items;
    cart.total = data.total;
}
function GetCartError() {
    MakeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);
}

document.addEventListener("DOMContentLoaded", function() {
    MakeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);

    $(document).on('click', '.clear-cart-btn', function() {
        // console.log("Clearing cart")
        // $.ajax({
        //     url: "/Cart/Clear",
        //     method: "POST",
        //     success: function(data) {
        //             location.reload();
        //     },
        //     error: function() {
        //         alert("Error clearing cart!");
        //     }
        // });

        function OnSuccess (data) {
            location.reload();
        }
        function OnError () {
            alert("Error clearing cart!");
        }

        MakeRequest("/Cart/Clear", "POST", null, OnSuccess, OnError);
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
                $(".cart-msg").fadeIn(500);
            },
            error: function () {
                console.log("Error adding to cart. Retrying...");
                $(".add-to-cart-btn").trigger("click");
            }
        });
    });

    // Remove from cart button
    $(document).on('click', '.remove-item-btn', function() {
        var index = $(this).attr("id");

        $.ajax({
            url: "/Cart/RemoveItem",
            type: "POST",
            data: { index: index },
            error: function () {
                console.log("Error removing from cart");
            }
        });

        location.reload();
    });

    var offset = 0;
    $(document).on('click', '.edit-product-count-btn', function() {
        var index = $(this).attr("item-index");
        var isIncrement = $(this).text() == "+";

        $.ajax({
            url: "/Cart/EditCount",
            type: "POST",
            data: { index: index - offset, isIncrement: isIncrement },
            error: function () {
                console.log("Error editing count");
            }
        });


        // <div class="edit-product-container">
        //                 <div class="edit-product-count">
        //                     <button class="edit-product-count-btn" item-index="@item.index" title="Decrease quantity">-</button>
        //                     <p class="product-count">@item.value.Quantity</p>
        //                     <button class="edit-product-count-btn" item-index="@item.index" title="Increase quantity">+</button>
        //                 </div>
        //                 <button class="edit-product-options-btn" item-index="@item.index" title="Edit item customizations">Edit</button>
        //                 <p class="product-cost">@item.value.Price().ToString("C")</p>
        //             </div>

        $(this).siblings(".product-count").text(parseInt($(this).siblings(".product-count").text()) + (isIncrement ? 1 : -1));

        // $(this).siblings(".count").text(parseInt($(this).siblings(".count").text()) + (isIncrement ? 1 : -1));
        
        // // Update cose of item divided by quantity
        // var count = parseInt($(this).siblings(".count").text());
        // var price = parseFloat($(this).closest(".product").find(".cost").text().substring(1));
        // price = price / count;
        // $(this).closest(".product").find(".cost").text("$" + (price * count).toFixed(2));

        // if (parseInt($(this).siblings(".count").text()) <= 0) {
        //     $(this).closest(".product").remove();
        //     offset++;
        // }
    });

    $(document).on('click', '.edit-options-btn', function() {
        alert("Not implemented yet!");
    });

    $(document).on('click', '.checkout-btn', function() {
        alert("Not implemented yet!");
    });
});
