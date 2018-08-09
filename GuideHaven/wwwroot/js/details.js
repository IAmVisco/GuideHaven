var sidenavTop = $('.sidenav').offset().top;

$(document).ready(function () {
    $("#desc").slideDown();

    $("li").on("click", function (e) {
        $(".step").slideUp();
        setTimeout(function () {
            //$("div:target").fadeIn();
            $("div:target").slideToggle();
        }, 195);

    });

    $("#comment-field").keyup(function (e) {
        if ($("#comment-field").val().length > 0)
            $(".comment-btn").removeAttr("disabled");
        else
            $(".comment-btn").attr("disabled", true);
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
        //alert("Working");
        $.post("/Guide/PostComment", {
            guideId: $("#guideId").attr("value"),
            comment: $("#comment-field").val()
        });
    });
});
