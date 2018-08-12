var sidenavTop = $('.sidenav').offset().top;
var step = 0;

function set_rating(rating) {
    $("#star" + rating).prop("checked", true);
}

function get_rating() {
    $.ajax({
        type: "GET",
        url: '/Guide/GetRating',
        data: { guideId: $("#guideId").attr("value") },
        success: function (response) {
            $("#rating-text").text("Rating: " + response.toFixed(2) + " stars");
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

function focusOption(el) {
    $(".sidenav-option").css("margin-left", "0");
    $(el).parent().css("margin-left", "-15px");
}

$(document).ready(function () {

    function showPopover() {
        $(".rating").popover("toggle");
        setTimeout(function () {
            $(".rating").popover("hide");
        }, 2000);
    }

    $("#step0").slideDown();
    $("#desc").click();
    get_rating();
    get_comments();
    setInterval(get_comments, 3000);

    $("#next-btn").on("click", function () {
        if (step < $(".steps-wrap").children().length - 1) {
            window.location.hash = 'step' + ++step;
            changeStep();
        }
    });

    $(".rating").popover({
        animation: true,
        content: "Log in to vote",
        placement: "auto bottom",
        trigger: "manual"
    });


    $("#prev-btn").on("click", function () {
        if (step > 0) {
            window.location.hash = 'step' + --step;
            changeStep();
        }
    });

    $("li").on("click", changeStep);

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
                top: '170px'
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
        }).fail(showPopover);
        setTimeout(get_rating, 500);
    });
});
