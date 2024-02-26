$(document).ready(function () {

    // Attach a submit handler to the form
    $("#doneButton").on('click', function (event) {
        // Stop form from submitting normally
        event.preventDefault();

        // Get some values from elements on the page:
        var $form = $(this).closest("form"),
          url = $form.attr("action");

        // Send the data using post
        var posting = $.post(url, $form.serialize());

        // Put the results in a div
        posting.done(function (data) {
            BootstrapDialog.closeAll();
        });
    });
    $("#exitButton").on('click', function (event) {
        // Stop form from submitting normally
        event.preventDefault();
        $('#ProjectFieldSampleID').val(0);
        BootstrapDialog.closeAll();
    });

    $('#PFSType').change(function () {
        if (this.value == "O") {
            $("#PFSTypeOther").show();
        } else {
            $("#PFSTypeOther").hide();
        }
    });

    $('#PFSMediaType').change(function () {
        SetupMediaFileUploader(this.value, null);
    });

});
function SetupMediaFileUploader(value, filePath) {
    //alert(value + "," + filePath);
    var mfOption;
    if (value == 'Image') {
        mfOption = {
            previewFileType: "image",
            browseClass: "btn btn-success",
            browseLabel: "Pick Image",
            browseIcon: '<i class="glyphicon glyphicon-picture"></i>',
            removeClass: "btn btn-danger",
            removeLabel: "Delete",
            removeIcon: '<i class="glyphicon glyphicon-trash"></i>',
            uploadClass: "btn btn-info",
            uploadLabel: "Upload",
            uploadIcon: '<i class="glyphicon glyphicon-upload"></i>',
            uploadUrl: pqAnswerUploadURL, // server upload action
            uploadAsync: false,
            maxFileCount: 1,
            maxFileSize: 5000
        };
        if (filePath != null) {
            pqAnswerInitialPreview = [
                "<img src='" + filePath + "' class='file-preview-image' />"
            ];
            mfOption.initialPreview = pqAnswerInitialPreview;
            mfOption.overwriteInitial = true;
        }
        $("#PFSMediaFile").fileinput(mfOption);
        
        $("#MediaFileControl").show();
    } else if (value == 'Audio') {
        mfOption = {
            previewFileType: "audio",
            browseLabel: "Pick Audio",
            allowedFileTypes: ["audio"],
            uploadUrl: pqAnswerUploadURL, // server upload action
            uploadAsync: false,
            maxFileCount: 1
        };
        if (filePath != null) {
            pqAnswerInitialPreview = [
                "<audio controls=''><source src='" + filePath + "' type='audio/mp3'></audio>"
            ];
            mfOption.initialPreview = pqAnswerInitialPreview;
            mfOption.overwriteInitial = true;
        }

        $("#PFSMediaFile").fileinput(mfOption);
        $("#MediaFileControl").show();
    } else if (value == 'Video') {
        mfOption = {
            previewFileType: "video",
            browseLabel: "Pick Video",
            allowedFileTypes: ["video", "*.MOV"],
            uploadUrl: pqAnswerUploadURL, // server upload action
            uploadAsync: false,
            maxFileCount: 1
        };
        if (filePath != null) {
            pqAnswerInitialPreview = [
                "<video width='213px' height='160px' controls=''><source src='" + filePath + "'  type='video/mp4'></video>"
            ];
            mfOption.initialPreview = pqAnswerInitialPreview;
            mfOption.overwriteInitial = true;
        }
        $("#PFSMediaFile").fileinput(mfOption);
        $("#MediaFileControl").show();
    } else {
        $("#MediaFileControl").hide();
    }
}

