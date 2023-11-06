var menu_path = [];

document.addEventListener("DOMContentLoaded", function () {
    // Load default data
    loadData($(".category-btn.active").attr("id"), $(".category-btn.active").attr("data-to"));
    menu_path = [$(".category-btn.active").attr("id") + "/" + $(".category-btn.active").attr("data-to")];

    // Load data when category button is clicked
    $(".category-btn").click(function () {
        if ($(this).hasClass("active")) return;
        $(".category-btn").removeClass("active");
        $(this).addClass("active");

        menu_path = [$(this).attr("id") + "/" + $(this).attr("data-to")];
        loadData($(this).attr("id"), $(this).attr("data-to"));
    });

    // Back button
    $(document).on('click', '.back-btn', function () {
        $(".item-container").html("");
        if (menu_path.length === 1) return;

        var action = menu_path[menu_path.length - 2];
        menu_path.pop();

        var _action = action.split("/")[0];
        var args = action.split("/")[1];
        var element = action.split("/")[2];

        loadData(_action, args, element);
    });

    // Customization buttons
    $(document).on('click', '.customization-btn', function () {
        $(this).toggleClass("active");
    });
});

function loadData(action, args, element) {
    var timeout = setTimeout(function () {
        document.dispatchEvent(new Event("DisplayLoadingScreen"));
    }, 10);

    console.log(element);

    $.ajax({
        url: "/MenuBoard/" + action,
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

    if (menu_path.length > 1) {
        $(".back-btn").show();
    } else {
        $(".back-btn").hide();
    }
}