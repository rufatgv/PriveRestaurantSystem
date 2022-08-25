$(document).ready(function(){

    // $(".nav-bar").slideUp();
      $(".nav-bar").css({ "display": "none" });
    $(".side-menu").removeClass("side-menu-active");
    $(".side-menu").click(function(){
        $(this).toggleClass("side-menu-active");
        $(".nav-bar").slideToggle();
    })
})