import { makeRequest } from './utils/request.js';

window.toggleSidebarAI = function() {
    var sidebar = document.querySelector(".sidebar-robot");
    if (sidebar.classList.contains("sidebar-open")) {
        sidebar.classList.remove("sidebar-open");
        sidebar.classList.add("sidebar-closed");
        document.cookie = "sidebarAI=false";
    } else {
        sidebar.classList.remove("sidebar-closed");
        sidebar.classList.add("sidebar-open");
        document.cookie = "sidebarAI=true";
    }
}

// Load history
function GetHistorySplit() {
    makeRequest("/AI/GetHistorySplit", "GET", null, function(response) {
        for (var i = 0; i < response.length; i++) {
            let responseType = response[i].split(":")[0];
            let responseText = response[i].split(":")[1];

            if (responseType == "User") {
                var responseContainer = createUserResponseContainer();
                responseContainer.querySelector(".response-text").innerHTML = responseText;
                $(".sidebar-output-text").append(responseContainer);
            } else if (responseType == "Assistant") {
                var responseContainer = createAIResponseContainer();
                responseContainer.querySelector(".response-text").innerHTML = responseText;
                $(".sidebar-output-text").append(responseContainer);
            }

            $(".sidebar-output").scrollTop($(".sidebar-output")[0].scrollHeight);
        }
    }, error);
}
document.addEventListener('DOMContentLoaded', (event) => {
    GetHistorySplit();
});

window.clearHistory = function() {
    makeRequest("/AI/ClearHistory", "GET", null, function(response) {
        $(".sidebar-output-text").html("");
    }, error);
}

// TTS
window.enableTTS = function() {
    var ttsButton = $(".sidebar-TTS-button");
    if (ttsButton.attr("on") == "false") {
        ttsButton.attr("on", "true");
        ttsButton.find("img").attr("src", "/img/tts.png");
        document.cookie = "tts=true";
    } else {
        ttsButton.attr("on", "false");
        ttsButton.find("img").attr("src", "/img/tts-off.png");
        document.cookie = "tts=false";
    }
}

window.sendInput = function() {
    var input = $(".sidebar-input-text").val();
    clearInput($(".sidebar-input-text"));

    let isempty = input.replace(/\s/g, '') == "";
    if (isempty) return;

    var responseContainer = createUserResponseContainer();
    responseContainer.querySelector(".response-text").innerHTML = input;
    $(".sidebar-output-text").append(responseContainer);
    $(".sidebar-output").scrollTop($(".sidebar-output")[0].scrollHeight);
    

    makeRequest("/AI/GetResponse", "POST", { input: input }, function(response) {
        var responseContainer = createAIResponseContainer();
        $(".sidebar-output-text").append(responseContainer);
        var responseText = responseContainer.querySelector(".response-text");
        var i = 0;
        var interval = setInterval(function() {
            responseText.innerHTML += response[i];
            $(".sidebar-output").scrollTop($(".sidebar-output")[0].scrollHeight);
            i++;
            if (i >= response.length) {
                clearInterval(interval);
                $(".sidebar-output-text").scrollTop($(".sidebar-output-text")[0].scrollHeight);
            }
        }, 5);
        
    }, error);

    
}

function clearInput(el) {
    $(el).val("");
}

function error(response) {
    console.log(response);
}

function createUserResponseContainer() {
    var responseContainer = document.createElement("div");
    responseContainer.classList.add("response-container");
    responseContainer.classList.add("user");

    var userLogo = document.createElement("img");
    userLogo.src = "/img/user.png";
    userLogo.alt = "User Logo";
    responseContainer.appendChild(userLogo);

    var responseText = document.createElement("p");
    responseText.classList.add("response-text");
    responseContainer.appendChild(responseText);

    return responseContainer;
}

function createAIResponseContainer() {
    var responseContainer = document.createElement("div");
    responseContainer.classList.add("response-container");
    responseContainer.classList.add("ai");

    var aiLogo = document.createElement("img");
    aiLogo.src = "/img/ai-chat.png";
    aiLogo.alt = "AI Logo";
    responseContainer.appendChild(aiLogo);

    var responseText = document.createElement("p");
    responseText.classList.add("response-text");
    responseContainer.appendChild(responseText);

    return responseContainer;
}
