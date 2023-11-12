/*
    File: customer-category-btn.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
*/

var customer_path = [];


document.addEventListener("DOMContentLoaded", function () {
    // Load default data
    loadData($(".category-btn.active").attr("id"), null, $(".category-btn.active").attr("data-to"));
    // loadData("ShowCustomization", 2, "customization-container");
    customer_path = [$(".category-btn.active").attr("id") + "/null/" + $(".category-btn.active").attr("data-to")];

    // Load data when category button is clicked
    $(".category-btn").click(function () {
        if ($(this).hasClass("active")) return;
        $(".category-btn").removeClass("active");
        $(this).addClass("active");

        customer_path = [$(this).attr("id") + "/null/" + $(this).attr("data-to")];
        loadData($(this).attr("id"), null, $(this).attr("data-to"));
    });

    // Load data when series button is clicked
    $(document).on('click', '.series-btn', function() {
        $(".customization-container").html("");
        var seriesName = $(this).attr("id");
        customer_path.push("ShowProductsBySeries/" + seriesName + "/" + $(this).attr("data-to"));
        loadData("ShowProductsBySeries", seriesName, $(this).attr("data-to"));
    });

    // Load data when product button is clicked
    $(document).on('click', '.product-btn', function() {
        $(".item-container").html("");
        var productID = $(this).attr("id");
        customer_path.push("ShowCustomization/" + productID + "/" + $(this).attr("data-to"));
        loadData("ShowCustomization", productID, $(this).attr("data-to"));
    });

    // Back button
    $(document).on('click', '.back-btn', function() {
        $(".customization-container").html("");
        if (customer_path.length == 1) return;

        var action = customer_path[customer_path.length - 2];
        customer_path.pop();

        var _action = action.split("/")[0];
        var args = action.split("/")[1];
        var element = action.split("/")[2];

        loadData(_action, args, element);
    });

    // Customization buttons
    $(document).on('click', '.customization-btn', function() {
        is_multiselect = $(this).attr("multiselect");
        series = $(this).attr("series");
        if (is_multiselect == "True") {
            $(this).toggleClass("active");
        } else {
            // $(".customization-btn").removeClass("active");
            // $(this).toggleClass("active");
            // remove active class from all buttons in the series
            $(".customization-btn[series='" + series + "']").removeClass("active");
            $(this).toggleClass("active");
        }
    });
});

function loadData(action, args, element) {
    var timeout = setTimeout(function () {
        document.dispatchEvent(new Event("DisplayLoadingScreen"));
    }, 10);
    
    $.ajax({
        url: "/Customer/" + action,
        type: "GET",
        data: { arg: args },
        success: function (data) {
            $("." + element).html(data).hide().fadeIn(500);
            clearTimeout(timeout);
            document.dispatchEvent(new Event("HideLoadingScreen"));
        },
        error: function () {
            $("." + element).html("Error loading data").show();
            clearTimeout(timeout);
            document.dispatchEvent(new Event("HideLoadingScreen"));
        }
    });

    if (customer_path.length > 1) {
        $(".back-btn").show();
    } else {
        $(".back-btn").hide();
    }
}
