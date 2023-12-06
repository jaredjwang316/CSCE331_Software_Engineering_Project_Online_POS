/*
    File: wwwroot/js/cart.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
    Purpose: Contains all javascript functions for the cart page.
*/

import { makeRequest } from './utils/make-request.js';
import { Cart, Item } from './models/cartModel.js';

function showCartContainer() {
    $(".cart-container").attr("hidden", false);
    $(".cartlayout-back").attr("hidden", false);
}
function hideCartContainer() {
    $(".cart-container").attr("hidden", true);
    $(".cartlayout-back").attr("hidden", true);
}
function showEmptyCartMessage() {
    $(".emptycart-message").attr("hidden", false);
}
function hideEmptyCartMessage() {
    $(".emptycart-message").attr("hidden", true);
}
function showCheckoutMessage() {
    $(".checkout-message").attr("hidden", false);
}
function hideCheckoutMessage() {
    $(".checkout-message").attr("hidden", true);
}
function showCartCounter() {
    $(".cart-counter").show();
}
function hideCartCounter() {
    $(".cart-counter").hide();
}

var cart = new Cart();  // Global cart object
function GetCartSuccess(data) {
    cart.items = data.items.map(item => Object.assign(new Item, item));
    let length = cart.Length();
    $(".cart-counter").text(length);

    if (length > 0) {
        showCartContainer();
        hideEmptyCartMessage();
        hideCheckoutMessage();
        showCartCounter();
    } else {
        hideCartContainer();
        showEmptyCartMessage();
        hideCheckoutMessage();
        hideCartCounter();
    }
}
function GetCartError() {
    makeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);
}

/* Main */
document.addEventListener("DOMContentLoaded", function() {
    makeRequest("/Cart/GetCartFromSession", "GET", null, GetCartSuccess, GetCartError);
    document.dispatchEvent(new Event("HideLoadingScreen"));

    // Add to cart button
    $(document).on('click', '.add-to-cart-btn', function() {
        if ($(this).prop("disabled")) {
            return;
        }
        $(this).prop("disabled", true);
        var productID = parseInt($(this).attr("id"));
        var customizationIDs = [];

        $(".customization-btn.active").each(function() {
            customizationIDs.push(parseInt($(this).attr("id")));
        });

        function AddToCartSuccess() {
            $(".cart-msg").fadeIn(500);
            $(this).prop("disabled", false);
        }

        function AddToCartError() {
            console.log("Error adding to cart. Retrying...");
        }

        makeRequest("/Cart/AddItem", "POST", { ProductId: productID, CustomizationIds: customizationIDs, Quantity: 1 }, AddToCartSuccess, AddToCartError);

        $(".cart-counter").show();
        $(".cart-counter").text(cart.Length() + 1);
    });

    // Remove from cart button
    $(document).on('click', '.remove-product-btn', function() {
        $(".checkout-btn").prop("disabled", true);

        var index = $(this).attr("item-index");
        makeRequest("/Cart/RemoveItem", "POST", { index: index }, function() {
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
                $(this).attr("item-index", item_index - 1);
            }
        });

        var total_cost = 0;
        cart.items.forEach(function(item) {
            total_cost += item.TotalPrice();
        });

        if (cart.Length() <= 0) {
            hideCartContainer();
            showEmptyCartMessage();
            hideCheckoutMessage();
            hideCartCounter();
        }
        
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
        makeRequest("/Cart/EditCount", "POST", { index: index, isIncrement: isIncrement }, function() {
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

        if (cart.Length() <= 0) {
            hideCartContainer();
            showEmptyCartMessage();
            hideCheckoutMessage();
            hideCartCounter();
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

    // Checkout button
    $(document).on('click', '.checkout-btn', function() {
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
        makeRequest("/Account/GetUserInfo", "GET", null, function(user_data) {
            name = user_data.name;
            role = user_data.role;
            email = user_data.email;

            function CheckoutSuccess() {
                // Update cart and html
                cart.items = [];
                $(".product").remove();
                $(".subtotal-value").text("$0.00");
                clearTimeout(timeout);
                document.dispatchEvent(new Event("HideLoadingScreen"));
                $(this).prop("disabled", false);
                $(".cart-counter").text(cart.Length());
                hideCartContainer();
                hideEmptyCartMessage();
                showCheckoutMessage();
                hideCartCounter();
            }

            function CheckoutError() {
                console.log("Error during checkout");
                clearTimeout(timeout);
                document.dispatchEvent(new Event("HideLoadingScreen"));
            }

            makeRequest("/Cart/Checkout", "POST", {name: name, role: role, email: email}, CheckoutSuccess, CheckoutError);
        }, function() {
            console.log("Error fetching user info");
        });
    });
});
