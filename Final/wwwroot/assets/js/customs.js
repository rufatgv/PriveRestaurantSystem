$(document).ready(function () {

    //addBasket
    $(document).on("click", ".addtocart", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");
        fetch(url).then(response => response.text()).then(data => {
            $(".forbasket").html(data);
            var closebasket = document.querySelector(".closebasket");
            closebasket.addEventListener("click", function (e) {
                e.preventDefault();
                var basketfor = document.querySelector(".forbasket");
                basketfor.style.visibility = "hidden";
                basketfor.style.right = "-100%";
                basketfor.style.opacity = "0";
            })
        })

    })
    //basketcount
    $(document).on("click", ".basketUpdate", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");
        let count = parseInt($(this).parent().find(".countInp").text());
        let id = 0;

        if ($(this).hasClass("subCount")) {
            if (count != 1) {
                count--;
            }
            id = $(this).next().attr("data-id");


            $(this).next().text(count);
        }
        else if ($(this).hasClass("addCount")) {
            count++;
            id = $(this).prev().attr("data-id");


            $(this).prev().text(count);
        }

        url = "Basket/Update" + "?id=" + id + "&count=" + count;


        fetch(url).then(response => {
            fetch("Basket/GetBasket").then(response => response.text()).then(data => $(".forbasket").html(data))
            return response.text()

        }).then(data => $(".maincart").html(data))

    })
    //deleteproductformcard
    $(document).on("click", ".deletecard", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => {
            fetch("Basket/GetBasket").then(response => response.text()).then(data => $(".forbasket").html(data))

            return response.text()
        }).then(data => $(".maincart").html(data))
    })
    //deleteproductfrombasket
    $(document).on("click", ".deletebasket", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => {
            fetch("Basket/GetCard").then(response => response.text()).then(data => $(".maincart").html(data))

            return response.text()
        }).then(data => $(".forbasket").html(data))
    })
    //search
    $(document).on("keyup", "#searchBtn", function (e) {
        e.preventDefault()
        console.log($(this).val())
        let url = $("#searchForm").attr("action");
        fetch(url + "?key=" + $(this).val()).then(res => {
            return res.text()
        }).then(data => {
            $("#productList").html(data)
        })
    })
    var MessageAction = function () {
        $(".message").css({
            "opacity": "0",
            "visibility": "hidden"
        })
    }
    $(document).on("click", ".resyyyy", function (e) {
        e.preventDefault();
        $(".message").css({
            "opacity": "1"
        })
        setTimeout(MessageAction, 3000);

        let mess = $(".CMessage").val();
        let email = $(".CEmail").val();
        let name = $(".CName").val();
        let phone = $(".CPhone").val();
        //var id = $(this).attr("data-id");
        //console.log(id)
        fetch("Contact/ContactMessage" + "?Message=" + mess + " &Email=" + email + "&Name=" + name + "&Phone=" + phone).then(res => {
            return res.text()
          
        }).then(data => {
            if (data == 0) {
                window.location = window.location.origin + "/Account/Login";
                return;
            }
            $(".contact-form").html(data);
        })
    })
  
    //var TableAction = function () {
    //    $(".success").css({
    //        "opacity": "0",
    //        "visibility": "hidden"
    //    })
    //}
    //$(document).on("click", ".res", function (e) {
    //    e.preventDefault();
    //    $(".success").css({
    //        "opacity": "1"
    //    })
    //    setTimeout(MessageAction, 3000);
    //    let name = $(".CName").val();
    //    let reserve = $(".reserve").val();
    //    let date = $(".CDate").val();
    //    let phone = $(".CPhone").val();
    //    let email = $(".CEmail").val();
       
     
    //    //var id = $(this).attr("data-id");
    //    //console.log(id)
    //    fetch("Table/TableReservation" + "?Name=" + name + "&Reserve=" + reserve + "&Date=" + date +  "&Phone=" + phone + "&Email=" + email).then(res => {
    //        return res.text()

    //    }).then(data => {
    //        if (data == 0) {
    //            window.location = window.location.origin + "/Account/Login";
    //            return;
    //        }
    //        $(".tableform").html(data);
    //    })
    //})
})
 
   
