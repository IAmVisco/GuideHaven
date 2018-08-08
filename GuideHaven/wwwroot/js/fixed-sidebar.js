var sidenavTop = $('.sidenav').offset().top;

$(window).scroll(function () {
    var currentScroll = $(window).scrollTop();

    if (currentScroll + 30 >= sidenavTop) {           
        $('.sidenav').css({                      
            //position: 'fixed',
            top: '60px',
            //right: '13%'
        });
    } else {                                   
        $('.sidenav').css({                      
            //position: 'static',
            top: '150px',
            //right: '0'
        });
    }
});