/*
    File: customer-category-btn.js
    Author: Griffin Beaudreau
    Date: November 5th, 2023
*/

var manager_path = [];


document.addEventListener("DOMContentLoaded", function () {
    // Load default data
    loadData($(".category-btn.active").attr("id"), null, $(".category-btn.active").attr("data-to"));
    manager_path = [$(".category-btn.active").attr("id") + "/null/" + $(".category-btn.active").attr("data-to")];

    // Load data when category button is clicked
    $(".category-btn").click(function () {
        if ($(this).hasClass("active")) return;
        $(".category-btn").removeClass("active");
        $(this).addClass("active");

        // manager_path = [$(this).attr("id") + "/null/" + $(this).attr("data-to")];
        // console.log(manager_path);
        // loadData($(this).attr("id"), null, $(this).attr("data-to"));
        if ($(this).attr("id") == "ShowManager") {
            showManagerPage();
        }
        else if ($(this).attr("id") == "ShowProducts") {
          //  document.getElementById('prod-table').style.width = '100%';
            showProductPage();
        }
        else if ($(this).attr("id") == "ShowInventory") {
         //   document.getElementById('inv-table').style.width = '100%';
            showInventoryPage();
        }

    });

    $(".table-button").click(function () {
        if ($(this).hasClass("active")) return;
        $(".table-button").removeClass("active");
        $(this).addClass("active");

    });
        
    document.getElementById('salesReport').addEventListener('click', function() {
        showManagerPage();
        document.getElementById('salesReportTable').style.display = 'inline-table';
        $.ajax({
            url: "/Manager/showSalesReport",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error showing table");
            }
        });
    });

    document.getElementById('restockReport').addEventListener('click', function() {
        showManagerPage();
        document.getElementById('restockReportTable').style.display = 'inline-table';
        $.ajax({
            url: "/Manager/showRestockReport",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error showing table");
            }
        });
    });

    document.getElementById('excessReport').addEventListener('click', function() {
        showManagerPage();
        document.getElementById('excessReportTable').style.display = 'inline-table';
        $.ajax({
            url: "/Manager/showExcessReport",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error showing table");
            }
        });
    });

    document.getElementById('salesTogether').addEventListener('click', function() {
        showManagerPage();
        document.getElementById('salesTogetherTable').style.display = 'inline-table';
        $.ajax({
            url: "/Manager/showSalesTogether",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error showing table");
            }
        });
    });

    document.getElementById('popularityAnalysis').addEventListener('click', function() {
        showManagerPage();
        document.getElementById('popularityAnalysisTable').style.display = 'inline-table';
        $.ajax({
            url: "/Manager/showPopularityAnalysis",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error showing table");
            }
        });
    });
});

function loadData(action, args, element) {
    var timeout = setTimeout(function () {
        document.dispatchEvent(new Event("DisplayLoadingScreen"));
    }, 10);

    // console.log(element);
    
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
}

function clearView() {
    document.getElementById('prod-table').style.display = 'none';
    document.getElementById('saveButtonProd').style.display = 'none';
    document.getElementById('addButtonProd').style.display = 'none';
    document.getElementById('inv-table').style.display = 'none';
    document.getElementById('saveButtonInv').style.display = 'none';
    document.getElementById('addButtonInv').style.display = 'none';

    document.getElementById('salesReport').style.display = 'none';
    document.getElementById('excessReport').style.display = 'none';
    document.getElementById('restockReport').style.display = 'none';
    document.getElementById('salesTogether').style.display = 'none';
    document.getElementById('popularityAnalysis').style.display = 'none';

    document.getElementById('salesReportTable').style.display = 'none';
    document.getElementById('excessReportTable').style.display = 'none';
    document.getElementById('restockReportTable').style.display = 'none';
    document.getElementById('salesTogetherTable').style.display = 'none';
    document.getElementById('popularityAnalysisTable').style.display = 'none';
}

function showManagerPage() {
    clearView();
    document.getElementById('salesReport').style.display = 'inline-block';
    document.getElementById('excessReport').style.display = 'inline-block';
    document.getElementById('restockReport').style.display = 'inline-block';
    document.getElementById('salesTogether').style.display = 'inline-block';
    document.getElementById('popularityAnalysis').style.display = 'inline-block';
}

function showProductPage() {
    clearView();
    document.getElementById('prod-table').style.display = 'inline-table';
    document.getElementById('saveButtonProd').style.display = 'inline-block';
    document.getElementById('addButtonProd').style.display = 'inline-block';
}

function showInventoryPage() {
    clearView();
    document.getElementById('inv-table').style.display = 'inline-table';
    document.getElementById('saveButtonInv').style.display = 'inline-block';
    document.getElementById('addButtonInv').style.display = 'inline-block';
}

function showProductIngredients() {
    clearView();
}