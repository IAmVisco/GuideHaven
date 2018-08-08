$(document).ready(function () {

    // Hide Header on on scroll down
    var didScroll;
    var lastScrollTop = 0;
    var delta = 5;
    var navbarHeight = $('.navbar').outerHeight();

    var sidenavTop = $('.sidenav').offset().top;

    $(window).scroll(function () {                  
        didScroll = true;
        var currentScroll = $(window).scrollTop(); 

        if (currentScroll >= sidenavTop) {           
            $('.sidenav').css({                      
                position: 'fixed',
                top: '10px',
                right: '13%'
            });
        } else {                                   
            $('.sidenav').css({                      
                position: 'static',
                top: '0',
                right: '0'
            });
        }

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
        placement: "auto top",
    });

    $(".not-admin-user").popover({
        animation: true,
        content: "This user does not have admin privileges.",
        trigger: "hover",
        placement: "auto top",
    });
    var checkBoxes = $('.check');
    $("#checkAll").click(function () {
        checkBoxes.not(this).prop('checked', this.checked);
    });

    $(".check").click(function () {
        $("#checkAll").prop('checked', false, checkBoxes.filter(':checked').length < checkBoxes.length)
        $("#checkAll").prop('checked', checkBoxes.filter(':checked').length === checkBoxes.length)
    });

    $("#themeChanger").click(function () {
        changeTheme();
    });
});