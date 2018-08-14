$(document).ready(function () {

    var didScroll;
    var lastScrollTop = 0;
    var delta = 5;
    var navbarHeight = $('.navbar').outerHeight();

    $(window).scroll(function () {                  
        didScroll = true;
    });

    setInterval(function () {
        if (didScroll) {
            hasScrolled();
            didScroll = false;
        }
    }, 250);

    function hasScrolled() {
        var st = $(this).scrollTop();

        if (Math.abs(lastScrollTop - st) <= delta)
            return;

        if (st > lastScrollTop && st > navbarHeight) {
            // Scroll Down
            $('.navbar').addClass('navbar-hidden');
        } else {
            // Scroll Up
            if (st + $(window).height() < $(document).height()) {
                $('.navbar').removeClass('navbar-hidden');
            }
        }

        lastScrollTop = st;
    }

    $(".admin-user").popover({
        animation: true,
        content: "This user has admin privileges.",
        trigger: "hover",
        placement: "auto top"
    });

    $(".not-admin-user").popover({
        animation: true,
        content: "This user does not have admin privileges.",
        trigger: "hover",
        placement: "auto top"
    });

    var checkBoxes = $('.check');

    $("#checkAll").click(function () {
        checkBoxes.not(this).prop('checked', this.checked);
    });

    $(".check").click(function () {
        $("#checkAll").prop('checked', false, checkBoxes.filter(':checked').length < checkBoxes.length);
        $("#checkAll").prop('checked', checkBoxes.filter(':checked').length === checkBoxes.length);
    });

    $("#themeChanger").click(function () {
        changeTheme();
    });

    $('td.link').click(function () {
        window.location = $(this).find('a').attr('href');
    }).hover(function() {$(this).toggleClass('hover');});

    $("#md-btn").click(function () {
        $(".md-help").slideToggle();
        $("#md-btn").toggleClass("glyphicon-plus");
        $("#md-btn").toggleClass("glyphicon-minus");
    });
});