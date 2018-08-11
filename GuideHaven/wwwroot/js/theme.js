﻿var isDark = false;

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function changeTheme() {
    var date = new Date();
    date = new Date(date.getTime() + 1000 * 60 * 60 * 24 * 365);

    if (!isDark) {
        document.getElementById('theme').href = "/css/dark.css";
        document.getElementById('nav').classList.add("navbar-inverse");
        document.cookie = 'theme=dark;expires=' + date.toGMTString() + ';path=/';
        isDark = true;
    }
    else {
        document.getElementById('theme').href = "/css/light.css";
        document.getElementById('nav').classList.remove("navbar-inverse");
        document.cookie = 'theme=light;expires=' + date.toGMTString() + ';path=/';
        isDark = false;
    }
}

if (getCookie("theme") == "dark") {
    document.getElementById('theme').href = "/css/dark.css";
    window.onload = function () {
        changeTheme();
        document.getElementById('themeChanger').checked = true;
    }
}
document.body.style.transition = "all 0.2s ease-in-out";
//console.log(document.body, document.getElementsByTagName("body")[0]);