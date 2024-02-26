var rowsCoding = [];
var previewMode = 0;

function urldecode(str) {
    return decodeURIComponent((str + '').replace(/\+/g, '%20'));
}

function setRowState(id, state) {
    rowsCoding[id] = state;
}
function containsRow(id) {
    return typeof rowsCoding[id] !== 'undefined';
};

$(document).ready(function () {
    //Show Overlay
    $(".overlay").css('display', 'block');
    $(".loading-img").css('display', 'block');

    var surveyform = $.get("/cati/web.html?pid=" + projectId + "&lang=" + projLang)
        .done(function (data) {
            $(".previewFrame").empty().append(data);

            //Hide Overlay
            $(".overlay").css('display', 'none');
            $(".loading-img").css('display', 'none');
        });
});
