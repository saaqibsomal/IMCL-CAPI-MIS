$(document).ready(function () {

    // Attach a submit handler to the form
    $("input").on('focusout', onChange);
    $("select").change(onChange);

    setTimeout(function () {
        $('#baseSize').focus();
    }, 0);
});

function onChange() {
}

function validateBaseSize() {
    var invalid = false;
    var _baseSize = parseInt($("#baseSize").val());

    if ($("#baseSize").val().length == 0 || _baseSize < 1) {
        alert("Base Size must be greater than 0 surveys");
        invalid = true;
    }
    if (invalid) {
        setTimeout(function () {
            $('#baseSize').focus();
        }, 0);
        return false;
    }
    return true;
}

function LoadFieldSample(fid) {
    //Show Overlay
    $(".overlay").css('display', 'block');
    $(".loading-img").css('display', 'block');

    $.getJSON("/Project/jsProjectFieldSample/"+fid)
    .done(function (data) {
        var items = [];
        items.push("<label>Base Size</label><br />");

        $.each(data, function (key, val) {
            items.push("<label>" + val.Title + "</label><br /><input name='primaryFieldSample' PFSId='"+val.ID+"' class='form-control' value='0' /><br />");
        });
        $("<div/>", {
            html: items.join("")
        }).appendTo("#primaryFieldSampleBox");
        var content = $(data).find("#primaryFieldSampleBox");
        $("#primaryFieldSampleBox").empty().append(items.join(""));

        //Hide Overlay
        $(".overlay").css('display', 'none');
        $(".loading-img").css('display', 'none');
    });

}

function onNext(stage, existing) {
    if (existing == 1) {
        $("#tab3").trigger('click');    //Switch to Tab 2
    } else {
        switch (stage) {
            case 1:
                $('#distributionTop').html($("#primaryField option:selected").text());

                $("#tab2").trigger('click');    //Switch to Tab 2
                break;
        }
    }
}


function onDeleteDefTab(projectId) {
    BootstrapDialog.confirm({
        title: 'WARNING',
        message: 'Warning! Delete Quota definition?',
        type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
        closable: true, // <-- Default value is false
        draggable: true, // <-- Default value is false
        btnCancelLabel: 'No', // <-- Default value is 'Cancel',
        btnOKLabel: 'Yes DELETE', // <-- Default value is 'OK',
        btnOKClass: 'btn-warning', // <-- If you didn't specify it, dialog type will be used,
        callback: function (result) {
            // result will be true if button was click, while it will be false if users close the dialog directly.
            if (result) {
                //Show Overlay
                $(".overlay").css('display', 'block');
                $(".loading-img").css('display', 'block');

                $.get('/ProjectQuota/Delete/' + projectId)
                    .done(function (data) {
                        //Hide Overlay
                        $(".overlay").css('display', 'none');
                        $(".loading-img").css('display', 'none');
                        location.reload();
                    });
            }
        }
    });
}

function onDeleteDataTab(projectId) {
    BootstrapDialog.confirm({
        title: 'WARNING',
        message: 'Warning! Delete Quota data?',
        type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
        closable: true, // <-- Default value is false
        draggable: true, // <-- Default value is false
        btnCancelLabel: 'No', // <-- Default value is 'Cancel',
        btnOKLabel: 'Yes DELETE', // <-- Default value is 'OK',
        btnOKClass: 'btn-warning', // <-- If you didn't specify it, dialog type will be used,
        callback: function (result) {
            // result will be true if button was click, while it will be false if users close the dialog directly.
            if (result) {
                //Show Overlay
                $(".overlay").css('display', 'block');
                $(".loading-img").css('display', 'block');

                $.get('/ProjectQuota/DeleteData/' + projectId)
                    .done(function (data) {
                        //Hide Overlay
                        $(".overlay").css('display', 'none');
                        $(".loading-img").css('display', 'none');
                        location.reload();
                    });
            }
        }
    });
}


// Update Row after Editing Grid Cell
function UpdateQuotaCell(projectId, rowid, rowdata, commit) {
    //Show Overlay
    $(".overlay").css('display', 'block');
    $(".loading-img").css('display', 'block');

    // Send the data using post
    var request = $.ajax({
        url: "/ProjectQuota/UpdateRow/" + projectId,
        type: "POST",
        data: { "rowid": rowid, "rowdata": JSON.stringify(rowdata) },
        dataType: "application/json"
    });
    request.done(function (msg) {
        //Hide Overlay
        $(".overlay").css('display', 'none');
        $(".loading-img").css('display', 'none');
    });
    request.fail(function (jqXHR, textStatus) {
        //Hide Overlay
        $(".overlay").css('display', 'none');
        $(".loading-img").css('display', 'none');

        if (jqXHR.status == 400) {
            alert("Update cell failed: " + textStatus);
        }
        //For some reason request.fail is being called always
    });
}
