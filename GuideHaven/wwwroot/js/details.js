var sidenavTop = $('.sidenav').offset().top;
var step = 0;

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/comments')
    .build();

connection.on('addcomment', function (comment) {
    document.getElementById("posted-comments").insertAdjacentHTML('beforeend', comment);

    $(".like-lbl").on("click", function () {
        let likes = parseInt(this.nextSibling.innerHTML);
        if (this.previousSibling.checked)
            likes -= 1;
        else
            likes += 1;
        this.nextSibling.innerHTML = likes;
        post_like($(this));
    });
});

connection.start();

function join_group() {
    connection.invoke("joingroup", $("#guideId").attr("value"));
}

function set_rating(rating) {
    $("#star" + rating).prop("checked", true);
}

function get_rating() {
    $.ajax({
        type: "GET",
        url: '/Guide/GetRating',
        data: { guideId: $("#guideId").attr("value") },
        success: function (response) {
            $("#rating-number").text(response.toFixed(2));
            set_rating(Math.round(response));
        }
    });
}

function get_comments() {
    $.ajax({
        type: "GET",
        url: '/Guide/GetComments',
        data: { guideId: $("#guideId").attr("value") },
        success: function (response) {
            addComments(response);
        }
    });
}

function get_likes() {
    $.ajax({
        type: "GET",
        url: '/Guide/GetLikes',
        data: { guideId: $("#guideId").attr("value") },
        success: function (response) {
            let index = 0;
            $(".like-count").each(function () {
                $(this).html(response[index++]);
            });
        }
    });
}

function post_like(div) {
    $.ajax({
        type: "POST",
        url: '/Guide/PostLike',
        data: { guideId: $("#guideId").attr("value"), commentId: $(div).attr("value") }
    });
}

function addComments(comments) {
    document.getElementById("posted-comments").innerHTML = "";
    document.getElementById("posted-comments").insertAdjacentHTML('beforeend', comments);

    $(".like-lbl").on("click", function () {
        let likes = parseInt(this.nextSibling.innerHTML);
        if (this.previousSibling.checked)
            likes -= 1;
        else
            likes += 1;
        this.nextSibling.innerHTML = likes;
        post_like($(this));
    });
}

function changeStep() {
    $(".step").slideUp();
    setTimeout(function () {
        $("div:target").slideToggle();
    }, 195);
}

function focusOption(el, stepId) {
    step = stepId;
    $(".sidenav-option").css("margin-left", "0");
    $(el).parent().css("margin-left", "-15px");
}

$(document).ready(function () {

    $("#step0").slideDown();
    $("#desc").click();
    get_rating();
    get_comments();

    $("#pdf-btn").on("click", function () {
        var doc = new jsPDF();
        var output = "Guide Name: " + $("#guide-name").text() + '\n\n';
        $("#header").each(function ( index ) {
            output += "Step " + (index + 1) + ": " + $(this).text() + '\n';
            output += $(this).next().text() + '\n\n';
        });
        doc.setFont('product-sans');
        doc.text(output, 10, 10);
        doc.save($("#guide-name").text() + ".pdf");
    });

    setInterval(get_likes, 3000);
    setTimeout(join_group, 500);

    function showPopover() {
        $(".rating").popover("toggle");
        setTimeout(function () {
            $(".rating").popover("hide");
        }, 2000);
    }

    $(".rating").popover({
        animation: true,
        content: "Log in to vote",
        placement: "auto bottom",
        trigger: "manual"
    });

    if (getCookie("ArrowAlert") !== "seen") {
        $(".arrows-alert").slideDown();
    }

    $("#next-btn").on("click", function () {
        if (step < $(".steps-wrap").children().length - 1) {
            window.location.hash = 'step' + ++step;
            changeStep();
        }
    });

    $("#prev-btn").on("click", function () {
        if (step > 0) {
            window.location.hash = 'step' + --step;
            changeStep();
        }
    });

    $(".sidenav li").on("click", changeStep);

    $(".arrows-alert").on("closed.bs.alert", function () {
        var date = new Date();
        date = new Date(date.getTime() + 1000 * 60 * 60 * 24 * 365);
        document.cookie = 'ArrowAlert=seen;expires=' + date.toGMTString() + ';path=/';
    });

    $("#comment-field").keyup(function (e) {
        if ($("#comment-field").val().length > 0) {
            $(".comment-btn").removeAttr("disabled");
        }
        else { 
            $(".comment-btn").attr("disabled", true);
        }
        $("#comment-field").height(42);
        $("#comment-field").height($("#comment-field")[0].scrollHeight);
    });

    $(window).scroll(function () {
        var currentScroll = $(window).scrollTop();

        if (currentScroll + 10 >= sidenavTop) {
            $('.sidenav').css({
                top: '60px'
            });
        } else {
            $('.sidenav').css({
                top: sidenavTop + 'px'
            });
        }
    });

    $("#post-btn").on("click", function () {
        if ($("#comment-field").val().length > 0) {
            $.post("/Guide/PostComment", {
                guideId: $("#guideId").attr("value"),
                comment: $("#comment-field").val()
            }, function (data) { connection.invoke("addcomment", $("#guideId").attr("value"), data) });
            $("#comment-field").val("");
            $(".comment-btn").attr("disabled", true);
        }
    });

    $("input[name=rating]").on("click", function () {
        $.post("/Guide/PostRating", {
            guideId: $("#guideId").attr("value"),
            rating: $(this).attr("value")
        }).fail(showPopover);
        setTimeout(get_rating, 500);
    });
});

$(document).keydown(function (e) {
    if (e.which === 37) { //left
        $("#prev-btn").click();
        $(".sidenav-option").css("margin-left", "0");
        $($("#side-menu").children()[step]).css("margin-left", "-15px"); 
    }
    if (e.which === 39) { //right
        $("#next-btn").click();
        $(".sidenav-option").css("margin-left", "0");
        $($("#side-menu").children()[step]).css("margin-left", "-15px"); 
    }
});
