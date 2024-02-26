function bodytext(fno, id, title, mediaList) {
    formControl.call(this, fno, id, title);
    this.type = "bodyText";
    this.title = title;
    this.MediaList = mediaList;
}

bodytext.prototype = Object.create(formControl.prototype); 
bodytext.prototype.constructor = bodytext;

bodytext.prototype.display = function () {
    var field = '<span>' + urldecode(this.title) + '</span>';
    var mediaHTML = "";
    var ctlId = 'media_' + this.id + '_';
    var i = 0;

    field += '<label class="sh_label_instructions" id="field_' + this.id + '_instructions">' + urldecode(this.instructions) + '</label>';

    if (this.MediaList != null && this.MediaList.length > 0) {
        $.each(this.MediaList, function (key, value) {
            if (value["FileType"] == "Image") {
                var runAuto = false;
                if (value["Options"] && value["Options"]["RunAuto"]) {
                    runAuto = true;
                }
                mediaHTML = '<img id="' + ctlId + i.toString() + '" src="http://capimis.apps.dna.com.sa/Pictures/SurveyMedia/' + value["FileName"] + '" style="margin:0 auto;max-width:80%;max-height:80%;display:' + (runAuto ? 'block' : 'none') + ';" onclick="this.hide()" />';
                if (!runAuto) {
                    mediaHTML += '<p><button onclick="bodytext.prototype.RunMedia(this, \'' + ctlId + i.toString() + '\')" class="btn">Show Image</button></p>';
                }
            }
        });
    }
    field += mediaHTML;
    return field;
}

bodytext.prototype.RunMedia = function (btn, ctlId) {
    ctlId = '#' + ctlId;
    if ($(ctlId).css("display") == 'none') {
        $(ctlId).css("display", "block");
        $(btn).html("Hide");
    } else {
        $(ctlId).css("display", "none");
        $(btn).html("Show");
    }
    return false;
}

bodytext.prototype.getValues= function(obj,fno) {
	buildData.varFormControls[fno].value=obj.value;	
}

bodytext.prototype.setValues = function(value) {
}
