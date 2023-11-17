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
});

function loadData(action, args, element) {
    var timeout = setTimeout(function () {
        document.dispatchEvent(new Event("DisplayLoadingScreen"));
    }, 10);

    console.log(element);
    
    $.ajax({
        url: "/Manager/" + action,
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
