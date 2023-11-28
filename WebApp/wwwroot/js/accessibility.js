
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

// Language
document.addEventListener('DOMContentLoaded', function () {
    let languageOptions = document.querySelectorAll(".option");
    languageOptions.forEach(option => {
        let select = option.querySelector("select");
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
    });
});
