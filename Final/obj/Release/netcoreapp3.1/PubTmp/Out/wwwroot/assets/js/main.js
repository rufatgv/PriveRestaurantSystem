$(document).ready(function () {
    $('.images').slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        arrows: false,
        autoplay: true,
        autoplaySpeed: 1000,
    });

    $('.aboutimage').slick({
        slidesToShow: 3,
        slidesToScroll: 1,
        arrows: false,
        autoplay: true,
        autoplaySpeed: 1000,
    });

	$('input[name="paymentmethod"]').on('click', function () {
		var $value = $(this).attr('value');
		$('.payment-method-details').slideUp();
		$('[data-method="' + $value + '"]').slideDown();
	});

    $("#create_pwd").on("change", function () {
		$(".account-create").slideToggle("100");
	});

	$("#ship_to_different").on("change", function () {
		$(".ship-to-different").slideToggle("100");
	});
});
$(window).scroll(function () {
        ScrollToTop();
    var header = $('.header'),
        scroll = $(window).scrollTop();

    if (scroll >= 150) {
        header.css({
            'position': 'fixed',
            'top': '0',
            'left': '0',
            'right': '0',
            'z-index': '99999'
        });
    } else {
        header.css({
            'position': 'absolute'

        });
    }
});


document.querySelector(".arrowup").addEventListener("click", function () {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });

})
function ScrollToTop() {
    let topbutton = document.querySelector(".buttontop");
    if (window.scrollY > 250) {
        topbutton.style.opacity = "1"
        topbutton.style.visibility = "visible"
    }
    else {
        topbutton.style.opacity = "0"
        topbutton.style.visibility = "hidden"
    }
}
ScrollToTop();

AOS.init();




  

