$(document).ready(function () {
    $('.btn-checkbox').on('click', function (e) {
        if ($(this).hasClass("item-checked")) {
            $(this).removeClass("item-checked");
            $(this).addClass("item-unchecked");
        } else {
            $(this).removeClass("item-unchecked");
            $(this).addClass("item-checked");
        }
    });
    $('.btn-top').on('click', function (e) {
        if ($(this).hasClass("top-selected")) {
            $(this).removeClass("top-selected");
        } else {
            $(this).addClass("top-selected");
        }
    });
    $('.btn-side').on('click', function (e) {
        if ($(this).hasClass("side-selected")) {
            $(this).removeClass("side-selected");
        } else {
            $(this).addClass("side-selected");
        }
    });

    // Attach a submit handler to the form
    $("#startQuery").on('click', function (event) {

        // Stop form from submitting normally
        event.preventDefault();

        // Get some values from elements on the page:
        var $form = $(this).closest("form"),
          url = $form.attr("action");

        //Show Overlay
        $(".overlay").css('display', 'block');
        $(".loading-img").css('display', 'block');

        // Send the data using post
        var posting = $.post(url, $form.serialize());

        // Put the results in a div
        posting.done(function (data) {
            var content = data; //$(data).find(".content");
            $("#tabulation").empty().append(content);

            //Hide Overlay
            $(".overlay").css('display', 'none');
            $(".loading-img").css('display', 'none');

            //Switch to Tab 3
            $("#tab3").trigger('click');
        });
    });
});
