
    // Accessibility Button
    function toggleSidebar() {
        var sidebar = document.querySelector(".sidebar");
        if (sidebar.classList.contains("sidebar-open")) {
            sidebar.classList.remove("sidebar-open");
            sidebar.classList.add("sidebar-closed");
        } else {
            sidebar.classList.remove("sidebar-closed");
            sidebar.classList.add("sidebar-open");
        }
    }

document.addEventListener('DOMContentLoaded', function () {
    let BigCursor = getCookie("BigCursor");
    let TextSize = getCookie("BigText");
    let Contrast = getCookie("Contrast");

    if (BigCursor == "true") {
        document.body.style.cursor = "url('https://wsv3cdn.audioeye.com/v2/build/bafc1df6358adc764094db250cf5a718.cur'), auto";
        document.body.classList.add("big-cursor");
        $(".toggle-big-cursor-btn").addClass("active");
    }
    if (TextSize == "true") {
        document.body.classList.add("big-text");
        $(".toggle-text-size-btn").addClass("active");
    }
    if (Contrast == "true") {
        document.body.classList.add("contrast");
        $(".toggle-contrast-btn").addClass("active");
    }

    // Cursor Size
    $(".toggle-big-cursor-btn").on("click", function () {
        if ($(this).hasClass("active")) {
            document.body.style.cursor = "default";
            document.body.classList.remove("big-cursor");
            $(this).removeClass("active");
            document.cookie = "BigCursor=false";
        } else {
            document.body.style.cursor = "url('https://wsv3cdn.audioeye.com/v2/build/bafc1df6358adc764094db250cf5a718.cur'), auto";
            document.body.classList.add("big-cursor");
            $(this).addClass("active");
            document.cookie = "BigCursor=true";
        }
    });

    // Text Size
    $(".toggle-text-size-btn").on("click", function () {
        if ($(this).hasClass("active")) {
            document.body.classList.remove("big-text");
            $(this).removeClass("active");
            document.cookie = "BigText=false";
        } else {
            document.body.classList.add("big-text");
            $(this).addClass("active");
            document.cookie = "BigText=true";
        }
    });

    // Contrast
    $(".toggle-contrast-btn").on("click", function () {
        if ($(this).hasClass("active")) {
            document.body.classList.remove("contrast");
            $(this).removeClass("active");
            document.cookie = "Contrast=false";
        } else {
            document.body.classList.add("contrast");
            $(this).addClass("active");
            document.cookie = "Contrast=true";
        }
    });
});

// Language
document.addEventListener('DOMContentLoaded', function () {
    let languageOptions = document.querySelectorAll(".option");
    languageOptions.forEach(option => {
        let select = option.querySelector("select");
        if (select) {
            select.addEventListener("change", () => {
                let selectedOption = option.querySelector("option[selected]");
                if (selectedOption) {
                    selectedOption.removeAttribute("selected");
                }
                let selected = select.options[select.selectedIndex];
                selected.setAttribute("selected", "");

                document.cookie = "CurrentLanguage=" + selected.value;
                location.reload();
                $(".toggle-sidebar-btn").click();
            });
        }
    });
});
