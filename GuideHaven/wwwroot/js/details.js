var sidenavTop = $('.sidenav').offset().top;

function set_rating(rating) {
    $("#star" + rating).prop("checked", true);
}

function get_rating() {
    $.ajax({
        type: "GET",
        url: '/Guide/GetRating',
        data: { guideId: $("#guideId").attr("value") },
        success: function (response) {
            set_rating(response);
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

function addComments(comments) {
    document.getElementById("posted-comments").innerHTML = "";
    document.getElementById("posted-comments").insertAdjacentHTML('beforeend', comments);
}

function update() {
    get_rating();
    get_comments();
}

$(document).ready(function () {
    $("#desc").slideDown();

    setInterval(update, 5000);

    $("li").on("click", function (e) {
        $(".step").slideUp();
        setTimeout(function () {
            //$("div:target").fadeIn();
            $("div:target").slideToggle();
        }, 195);

    });

    $("#comment-field").keyup(function (e) {
        if ($("#comment-field").val().length > 0) {
            $(".comment-btn").removeAttr("disabled");
            $(".comment-btn").slideDown();
        }
        else { 
            $(".comment-btn").attr("disabled", true);
            $(".comment-btn").slideUp();
        }
        $("#comment-field").height(42);
        $("#comment-field").height($("#comment-field")[0].scrollHeight);
    });

    $(window).scroll(function () {
        var currentScroll = $(window).scrollTop();

        if (currentScroll + 20 >= sidenavTop) {
            $('.sidenav').css({
                top: '60px',
            });
        } else {
            $('.sidenav').css({
                top: '150px',
            });
        }
    });

    $("#post-btn").on("click", function () {
        $.post("/Guide/PostComment", {
            guideId: $("#guideId").attr("value"),
            comment: $("#comment-field").val()
        });
        $("#comment-field").val("");
    });

    $("input[name=rating]").on("click", function () {
        $.post("/Guide/PostRating", {
            guideId: $("#guideId").attr("value"),
            rating: $(this).attr("value")
        });
    });
});
