/*
    File: wwwroot/js/cart.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
    Purpose: Contains all javascript functions for the cart page.
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

    Length() {
        var count = 0;
        this.items.forEach(function(item) {
            count += item.quantity;
        });
        return count;
    }
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
    $(".cart-counter").text(cart.Length());
}
function GetCartError() {
    MakeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);
}

/* Main */
document.addEventListener("DOMContentLoaded", function() {
    MakeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);
    document.dispatchEvent(new Event("HideLoadingScreen"));

    // Add to cart button
    $(document).on('click', '.add-to-cart-btn', function() {
        if ($(this).prop("disabled")) {
            console.log("disabled...");
            return;
        } else {
            console.log("not disabled...");
        }
        $(this).prop("disabled", true);
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
        $(".checkout-btn").prop("disabled", true);

        var index = $(this).attr("item-index");
        MakeRequest("/Cart/RemoveItem", "POST", { index: index }, function() {
            $(".checkout-btn").prop("disabled", false);
        }, function() {
            console.log("Error removing from cart");
            $(".checkout-btn").prop("disabled", false);
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
        $(".edit-product-count-btn").each(function() {
            var item_index = parseInt($(this).attr("item-index"));
            if (item_index > index) {
                $(this).attr("item-index",  - 1);
            }
        });

        var total_cost = 0;
        cart.items.forEach(function(item) {
            total_cost += item.TotalPrice();
        });

        // Update html cart total
        $(".subtotal-value").text("$" + (total_cost).toFixed(2));

        // Update cart counter
        $(".cart-counter").text(cart.Length());
    });

    var activeRequests = 0;
    $(document).on('click', '.edit-product-count-btn', function() {
        $(".checkout-btn").prop("disabled", true);
        activeRequests++;
        var index = $(this).attr("item-index");
        var isIncrement = $(this).text() == "+";

        // Update session cart object item count
        MakeRequest("/Cart/EditCount", "POST", { index: index, isIncrement: isIncrement }, function() {
            activeRequests--;
            if (activeRequests == 0) {
                $(".checkout-btn").prop("disabled", false);
            }
        }, function() {
            console.log("Error editing cart item count");
            activeRequests--;
            if (activeRequests == 0) {
                $(".checkout-btn").prop("disabled", false);
            }
        });

        // Update js cart object item count
        cart.items[index].quantity += isIncrement ? 1 : -1;

        // Remove item if quantity is 0
        if (cart.items[index].quantity <= 0) {
            $(this).closest(".product").remove();
            cart.items.splice(index, 1);

            // update all html item indices after removed item
            $(".remove-product-btn").each(function() {
                var item_index = parseInt($(this).attr("item-index"));
                if (item_index > index) {
                    $(this).attr("item-index", item_index - 1);
                }
            });
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

        // Update cart counter
        $(".cart-counter").text(cart.Length());
    });

    // $(document).on('click', '.edit-product-options-btn', function() {
    //     // Use customer controller and navigate to customization page
    //     var productID = $(this).attr("id");
    //     var customizations = $(this).attr("customizations");
    // });

    // Checkout button
    $(document).on('click', '.checkout-btn', function() {
        // var confirmCheckout = confirm("Process payment");
        // if (!confirmCheckout) return;
        console.log(cart.Length());
        if (cart.Length() == 0) {
            return;
        }

        $(this).prop("disabled", true);
        var timeout = setTimeout(function () {
            document.dispatchEvent(new Event("DisplayLoadingScreen"));
        }, 10);

        var name = "";
        var role = "";
        var email = "";
        MakeRequest("/Account/GetUserInfo", "GET", null, function(user_data) {
            name = user_data.name;
            role = user_data.role;
            email = user_data.email;

            $.ajax({
                url: "/Cart/Checkout",
                type: "POST",
                data: {name: name, role: role, email: email},
                success: function (response) {
                    // Update cart and html
                    cart.items = [];
                    $(".product").remove();
                    $(".subtotal-value").text("$0.00");
                    clearTimeout(timeout);
                    document.dispatchEvent(new Event("HideLoadingScreen"));
                    $(this).prop("disabled", false);
                    $(".cart-counter").text(cart.Length());
                },
                error: function () {
                    console.log("Error during checkout");
                    clearTimeout(timeout);
                    document.dispatchEvent(new Event("HideLoadingScreen"));
                }
            });
        }, function() {
            console.log("Error fetching user info");
        });
    });
});
