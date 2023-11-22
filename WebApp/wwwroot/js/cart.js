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

/* Cart */
class Cart {
    items = [Item];
    total = 0;
}
class Item {
    product;
    options = [];
    quantity = 0;
    cost = 0;

    // method to calculate cost
    TotalPrice() {
        var total = 0.0;
        total += this.product.price;
        this.options.forEach(function(option) {
            total += option.price;
        });
        total *= this.quantity;
        return total;
    }
}
var cart = new Cart();  // Global cart object
function GetCartSuccess(data) {
    cart.items = data.items.map(item => Object.assign(new Item, item));
    cart.total = data.total;
}
function GetCartError() {
    MakeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);
}

/* Main */
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
    $(document).on('click', '.remove-product-btn', function() {

        var index = $(this).attr("item-index");
        MakeRequest("/Cart/RemoveItem", "POST", { index: index }, null, function() {
            console.log("Error removing from cart");
        });

        $(this).closest(".product").remove();
        cart.items.splice(index, 1);

        // update all html item indices after removed item
        $(".remove-product-btn").each(function() {
            var item_index = parseInt($(this).attr("item-index"));
            if (item_index > index) {
                $(this).attr("item-index", item_index - 1);
            }
        });

        var total_cost = 0;
        cart.items.forEach(function(item) {
            total_cost += item.TotalPrice();
        });

        // Update html cart total
        $(".subtotal-value").text("$" + (total_cost).toFixed(2));
    });

    $(document).on('click', '.edit-product-count-btn', function() {
        var index = $(this).attr("item-index");
        var isIncrement = $(this).text() == "+";

        // Update session cart object item count
        MakeRequest("/Cart/EditCount", "POST", { index: index, isIncrement: isIncrement }, null, function() {
            console.log("Error editing count");
        });

        // Update js cart object item count
        cart.items[index].quantity += isIncrement ? 1 : -1;

        // Remove item if quantity is 0
        if (cart.items[index].quantity <= 0) {
            $(this).closest(".product").remove();
            cart.items.splice(index, 1);

            // update all html item indices after removed item
            $(".edit-product-count-btn").each(function() {
                var item_index = parseInt($(this).attr("item-index"));
                if (item_index > index) {
                    $(this).attr("item-index", item_index - 1);
                }
            });
        }

        var product_total_cost = 0;
        var total_cost = 0;
        if (cart.items && cart.items[index]) {
            product_total_cost = cart.items[index].TotalPrice();
            cart.items.forEach(function(item) {
                total_cost += item.TotalPrice();
            });
        }

        // Update html item count
        $(this).siblings(".product-count").text(parseInt($(this).siblings(".product-count").text()) + (isIncrement ? 1 : -1));

        // Update html item cost
        $(this).parent().siblings(".product-cost").text("$" + (product_total_cost).toFixed(2));

        // Update html cart total
        $(".subtotal-value").text("$" + (total_cost).toFixed(2));
    });

    $(document).on('click', '.edit-options-btn', function() {
        alert("Not implemented yet!");
    });

    $(document).on('click', '.checkout-btn', function() {
        MakeRequest("/Cart/GetCartFromSession", "GET", null, function(data) {
            var cartItems = data.items;

            var confirmCheckout = confirm("Process payment");

            if (confirmCheckout) {
                $.ajax({
                    url: "/Cart/Checkout",
                    type: "POST",
                    data: { cartItems: cartItems },
                    success: function (response) {
                        console.log("Checkout successful");
                        location.reload();
                    },
                    error: function () {
                        console.log("Error during checkout");
                    }
                });
            }
        }, function() {
            console.log("Error fetching cart data for checkout");
        });
    });
});
