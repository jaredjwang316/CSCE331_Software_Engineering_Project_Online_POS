/*
    File: customer.js
    Author: Griffin Beaudreau
    Date: November 12, 2023
    Purpose: Basic functionality for the customer page.
        Mainly used for loading data from the AWS database when
        a button is clicked.
*/

// Path for back button
var path = [];

document.addEventListener('DOMContentLoaded', (event) => {
    loadData($(".category-btn.active").attr("endpoint"));

    // Click event for category buttons
    $(".category-btn").click(function () {
        if ($(this).hasClass("active")) return;
        $(".category-btn").removeClass("active");
        $(this).addClass("active");

        path = [];
    
        loadData($(this).attr("endpoint"));
    });

    // Click event for series buttons
    $(document).on('click', '.series-btn', function() {
        $(".customization-container").html("");
        var seriesName = $(this).attr("id");
        loadData($(this).attr("endpoint"), seriesName);
    });

    // Click event for product buttons
    $(document).on('click', '.product-btn', function() {
        $(".item-container").html("");
        var productID = $(this).attr("id");
        loadData($(this).attr("endpoint"), productID, "customization-menu");
    });

    // Click event for customization buttons
    $(document).on('click', '.customization-btn', function() {
        if ($(this).attr("multiselect")) {
            $(this).toggleClass("active");
        } else {
            $(".customization-btn[series='" + $(this).attr("series") + "']").removeClass("active");
            $(this).toggleClass("active");
        }
    });

    // Click event for back button
    $(document).on('click', '.back-btn', function() {
        var split_path = path[path.length-2].split("/");
        path.pop();
        path.pop();
        var endpoint = split_path[0];
        var argument = split_path[1];
        var element = split_path[2];
        
        loadData(endpoint, argument, element);
    });

    // Click event for cart-msg buttons
    $(document).on('click', '.cart-msg-btn', function() {
        if ($(this).text() == "View Cart") {
            window.location.href = "/Cart";
        } else {
            $(".cart-msg").fadeOut(500);
            location.reload();
        }
    });
});


/*
    Function: loadData
    Author: Griffin Beaudreau
    Date: November 12, 2023
    Purpose: Loads data from a specified endpoint and displays it to an element.
        While the data is loading, a loading screen is displayed.
*/
function loadData(endpoint, argument = null, element = "data-container") {
    path.push(endpoint + "/" + argument + "/" + element);
    var timeout = setTimeout(function () {
        document.dispatchEvent(new Event("DisplayLoadingScreen"));
    }, 10);

    $.ajax({
        url: "/Customer/" + endpoint,
        type: "GET",
        data: { arg: argument },
        success: function (data) {
            clearData();
            $("." + element).html(data).hide().fadeIn(500);
            clearTimeout(timeout);
            document.dispatchEvent(new Event("HideLoadingScreen"));
        },
        error: function () {
            clearData();
            $("." + element).html("Error loading data").show();
            clearTimeout(timeout);
            document.dispatchEvent(new Event("HideLoadingScreen"));
        }
    });

    if (path.length > 1) {
        $(".back-btn").show();
    }
    else {
        $(".back-btn").hide();
    }
}

/*
    Function: clearData
    Author: Griffin Beaudreau
    Date: November 12, 2023
    Purpose: Clears all data from the data-container and customization-menu elements.
*/
function clearData() {
    $(".data-container").html("");
    $(".customization-menu").html("");
}
