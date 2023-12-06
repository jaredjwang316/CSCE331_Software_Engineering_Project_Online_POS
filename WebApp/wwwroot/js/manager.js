/*
    File: manager.js
    Author: Christopher Kelley
    Date: November 5th, 2023
*/

var manager_path = [];


document.addEventListener("DOMContentLoaded", function () {
    // Load default data
    loadData($(".category-btn.active").attr("id"), null, $(".category-btn.active").attr("data-to"));
    manager_path = [$(".category-btn.active").attr("id") + "/null/" + $(".category-btn.active").attr("data-to")];

    // Load data when category button is clicked
    $(".category-btn").click(function () {
        $(".category-btn").removeClass("active");
        $(".table-button").removeClass("active");
        $(this).addClass("active");

        // manager_path = [$(this).attr("id") + "/null/" + $(this).attr("data-to")];
        // console.log(manager_path);
        // loadData($(this).attr("id"), null, $(this).attr("data-to"));
        if ($(this).attr("id") == "ShowManager") {
            ShowManagerPage();
        }
        else if ($(this).attr("id") == "ShowProducts") {
          //  document.getElementById('prod-table').style.width = '100%';
            ShowProductPage();
        }
        else if ($(this).attr("id") == "ShowInventory") {
         //   document.getElementById('inv-table').style.width = '100%';
            ShowInventoryPage();
        }

    });

    $(".table-button").click(function () {
        if ($(this).hasClass("active")) return;
        $(".table-button").removeClass("active");
        $(this).addClass("active");

    });
        
    document.getElementById('salesReport').addEventListener('click', function() {
        ShowManagerPage();
        ClearDates();
        document.getElementById('salesReportTable').style.display = 'inline-table';
        var Data = document.getElementById('starttime').value;
        var Data2 = document.getElementById('endtime').value;
        console.log(Data);
        console.log(Data2);
        var output = {
            data: JSON.stringify(Data),
            data2: JSON.stringify(Data2)
        };
        $.ajax({
            url: "/Manager/ShowSalesReport",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(output),
            success: function (response) {
                var table = document.getElementById("salesReportTable");
                console.log(response);
                for (var ing of response) {
                    console.log(ing);
                    var row = table.insertRow(-1);
                    var cell1 = row.insertCell(0);
                    var cell2 = row.insertCell(1);
                    cell1.innerHTML = ing.item1;
                    cell2.innerHTML = ing.item2;
                }
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success Showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error Showing table");
            }
        });
    });

    document.getElementById('restockReport').addEventListener('click', function() {
        ShowManagerPage();
        ClearDates();
        document.getElementById('restockReportTable').style.display = 'inline-table';

        $.ajax({
            url: "/Manager/ShowRestockReport",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
                var table = document.getElementById("restockReportTable");
                console.log(response);
                for (var ing of response) {
                    console.log(ing);
                    var row = table.insertRow(-1);
                    var cell1 = row.insertCell(0);
                    var cell2 = row.insertCell(1);
                    cell1.innerHTML = ing.id;
                    cell2.innerHTML = ing.name;
                }
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success Showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error Showing table");
            }
        });
    });

    document.getElementById('excessReport').addEventListener('click', function() {
        ShowManagerPage();
        ClearDates();
        document.getElementById('excessReportTable').style.display = 'inline-table';
        var Data = document.getElementById('starttime').value;
        var Data2 = document.getElementById('endtime').value;
        console.log(Data);
        console.log(Data2);
        var output = {
            data: JSON.stringify(Data),
            data2: JSON.stringify(Data2)
        };
        $.ajax({
            url: "/Manager/ShowExcessReport",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(output),
            success: function (response) {
                var table = document.getElementById("excessReportTable");
                console.log(response);
                for (var ing of response) {
                    console.log(ing);
                    var row = table.insertRow(-1);
                    var cell1 = row.insertCell(0);
                    var cell2 = row.insertCell(1);
                    cell1.innerHTML = ing.id;
                    cell2.innerHTML = ing.name;
                }
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success Showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error Showing table");
            }
        });
    });

    document.getElementById('salesTogether').addEventListener('click', function() {
        ShowManagerPage();
        ClearDates();
        document.getElementById('salesTogetherTable').style.display = 'inline-table';
        var Data = document.getElementById('starttime').value;
        var Data2 = document.getElementById('endtime').value;
        console.log(Data);
        console.log(Data2);
        var output = {
            data: JSON.stringify(Data),
            data2: JSON.stringify(Data2)
        };
        $.ajax({
            url: "/Manager/ShowSalesTogether",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(output),
            success: function (response) {
                var table = document.getElementById("salesTogetherTable");
                console.log(response);
                for (var ing of response) {
                    var row = table.insertRow(-1);
                    var cell1 = row.insertCell(0);
                    var cell2 = row.insertCell(1);
                    var cell3 = row.insertCell(2);
                    cell1.innerHTML = ing.item1;
                    cell2.innerHTML = ing.item2;
                    cell3.innerHTML = ing.item3.toString();
                }
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success Showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error Showing table");
            }
        });
    });

    document.getElementById('popularityAnalysis').addEventListener('click', function() {
        ShowManagerPage();
        ClearDates();
        document.getElementById('popularityAnalysisTable').style.display = 'inline-table';
        var Data = document.getElementById('starttime').value;
        var Data2 = document.getElementById('endtime').value;
        console.log(Data);
        console.log(Data2);
        var output = {
            data: JSON.stringify(Data),
            data2: JSON.stringify(Data2)
        };
        $.ajax({
            url: "/Manager/ShowPopularityAnalysis",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(output),
            success: function (response) {
                document.getElementById("saveSuccess").style.display = 'block';
                    setTimeout(function() {
                        $('#saveSuccess').fadeOut('fast');
                    }, 5000);
                console.log("Success Showing table");
            },
            error: function () {
                document.getElementById("saveFail").style.display = 'block';
                    setTimeout(function() {
                        $('#saveFail').fadeOut('fast');
                    }, 5000);
                console.log("Error Showing table");
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

function ClearView() {
    document.getElementById('prod-table').style.display = 'none';
    document.getElementById('saveButtonProd').style.display = 'none';
    document.getElementById('addButtonProd').style.display = 'none';
    document.getElementById('inv-table').style.display = 'none';
    document.getElementById('saveButtonInv').style.display = 'none';
    document.getElementById('addButtonInv').style.display = 'none';
    document.getElementById('confirm').style.display = 'none';
    document.getElementById('cancel').style.display = 'none';
    document.getElementById('proding').style.display = 'none';

    var input = document.getElementsByClassName('ingredients');
    for(var i = 0; input[i]; ++i) {
        if (input[i].checked) {
            document.getElementById(input[i].value).checked = false;
        }
    }

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

    $('#salesReportTable').find("tr:not(:first)").remove();
    $('#excessReportTable').find("tr:not(:first)").remove();
    $('#restockReportTable').find("tr:not(:first)").remove();
    $('#salesTogetherTable').find("tr:not(:first)").remove();
    $('#popularityAnalysisTable').find("tr:not(:first)").remove();

    document.getElementById('starttime').style.display = 'none';
    document.getElementById('endtime').style.display = 'none';
    document.getElementById('slabel').style.display = 'none';
    document.getElementById('elabel').style.display = 'none';
}

function ClearDates() {
    document.getElementById('starttime').style.display = 'none';
    document.getElementById('endtime').style.display = 'none';
    document.getElementById('slabel').style.display = 'none';
    document.getElementById('elabel').style.display = 'none';
}

function ShowManagerPage() {
    ClearView();
    document.getElementById('salesReport').style.display = 'inline-block';
    document.getElementById('excessReport').style.display = 'inline-block';
    document.getElementById('restockReport').style.display = 'inline-block';
    document.getElementById('salesTogether').style.display = 'inline-block';
    document.getElementById('popularityAnalysis').style.display = 'inline-block';

    document.getElementById('starttime').style.display = 'inline-block';
    document.getElementById('endtime').style.display = 'inline-block';
    document.getElementById('slabel').style.display = 'inline-block';
    document.getElementById('elabel').style.display = 'inline-block';
}

function ShowProductPage() {
    ClearView();
    document.getElementById('prod-table').style.display = 'inline-table';
    document.getElementById('saveButtonProd').style.display = 'inline-block';
    document.getElementById('addButtonProd').style.display = 'inline-block';
}

function ShowInventoryPage() {
    ClearView();
    document.getElementById('inv-table').style.display = 'inline-table';
    document.getElementById('saveButtonInv').style.display = 'inline-block';
    document.getElementById('addButtonInv').style.display = 'inline-block';
}

function ShowProductIngredients() {
    ClearView();
    document.getElementById('proding').style.display = 'inline';
    document.getElementById('confirm').style.display = 'inline-block';
    document.getElementById('cancel').style.display = 'inline-block';
}