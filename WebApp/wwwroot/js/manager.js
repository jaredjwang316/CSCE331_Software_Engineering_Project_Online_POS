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
            document.getElementById('manager-msg').style.display = 'block';
            document.getElementById('prod-table').style.display = 'none';
            document.getElementById('saveButtonProd').style.display = 'none';
            document.getElementById('inv-table').style.display = 'none';
            document.getElementById('saveButtonInv').style.display = 'none';
            return;
        }
        else if ($(this).attr("id") == "ShowProducts") {
          //  document.getElementById('prod-table').style.width = '100%';
            document.getElementById('prod-table').style.display = 'inline-table';
            document.getElementById('saveButtonProd').style.display = 'block';
            document.getElementById('inv-table').style.display = 'none';
            document.getElementById('saveButtonInv').style.display = 'none';
            document.getElementById('manager-msg').style.display = 'none';
        }
        else if ($(this).attr("id") == "ShowInventory") {
         //   document.getElementById('inv-table').style.width = '100%';
            document.getElementById('inv-table').style.display = 'inline-table';
            document.getElementById('saveButtonInv').style.display = 'block';
            document.getElementById('prod-table').style.display = 'none';
            document.getElementById('saveButtonProd').style.display = 'none';
            document.getElementById('manager-msg').style.display = 'none';
        }
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
}