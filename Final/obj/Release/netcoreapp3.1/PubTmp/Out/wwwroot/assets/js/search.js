var searchBtn = document.querySelector(".searchglass");
searchBtn.addEventListener("click",function(e){
    e.preventDefault();
    var secFor = document.querySelector(".forsearch");
    secFor.style.height = "100%";
    secFor.style.opacity = "1";
    secFor.style.top = "0";
    secFor.style.visibility = "visible";
})

var closeIcon = document.querySelector(".mark");
closeIcon.addEventListener("click",function(){
    var secFor = document.querySelector(".forsearch");
    secFor.style.height = "0%";
    secFor.style.top = "0%";
    secFor.style.opacity = "0";
    secFor.style.visibility = "hidden";
})
