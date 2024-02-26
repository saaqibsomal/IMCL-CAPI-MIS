function radioboxControls (fno, id, title, optionData, orientation) {
    formControl.call(this, fno, id, title);
    this.type = "radio";
    this.optionData = optionData;

    if (optionData != null) {
        $.each(this.optionData, function (index, value) {
            this.value = 0;
            if (typeof value['IncludeField'] != 'undefined') {
                this.connectedField = value['IncludeField'];
            }
            else {
                this.connectedField = '';
            }
        });
    }
    if (!orientation) {
        this.orientation = "V";
    } else {
        this.orientation = orientation;
    }
}

radioboxControls.prototype = Object.create(formControl.prototype); 
radioboxControls.prototype.constructor = radioboxControls;

radioboxControls.prototype.display = function() {
    var field = this.displayLabel();

    rId=this.id;
    rName=this.name;
    rType=this.type;
    qno=this.qno;
    var oclass = "";
    if (this.orientation == "H") {
        oclass = "horizontal";
    }

    if (this.optionData != null) {
        var maxWidth = parseInt(100 / this.optionData.length);
        if (maxWidth == 0 || this.orientation == "V") maxWidth = 80;

        field += '<div class="mrradio ' + oclass + '">';
        $.each(this.optionData, function (key, value)
        {
            field += '<div style="width:' + maxWidth + '%;">';
            if (typeof (value["MediaFiles"]) != "undefined" && value["MediaFiles"] != null) {
                $.each(value["MediaFiles"], function (mfkey, mfvalue) {

                    if (mfvalue["FileType"] == "Image") {
                        field += '<img src="' + sf.SurveyMediaUrl + mfvalue["FileName"] + '" style="margin:0 auto;max-width:80%;max-height:80%;' + (oclass == "horizontal" ? "" : "float:left;") + '" />';
                    }
                });
            }

            field += '<input type="' + rType + '" name="' + rName + '" id="_' + rId + '_' + value['Id'] + '" onClick="radioboxControls.prototype.onChange(this,' + qno + ',' + key + ')" value="' + value['Id'] + '"><label for="_' + rId + '_' + value['Id'] + '" class="cus-label"><span></span><p style="width:80%;">' + value['Option'] + '</p></label></div>';
        });
        field += '</div>';
    }

    field+='<div id="radio_connected_'+qno+'"></div>'; 
    return field;
}



radioboxControls.prototype.onChange= function(obj,iter,itemNo) {

    buildData.varFormControls[iter].value = obj.value;

    var actionMove = buildData.varFormControls[iter].optionData[itemNo].connectedField;

    var Ids = $('#_' + buildData.varFormControls[iter].id + '_' + buildData.varFormControls[iter].optionData[itemNo].Id);

    var optionChecked = Ids.is(":checked");

    if (actionMove != "") {
        if (optionChecked) {

            $('#radio_connected_' + iter).html($('#field_' + actionMove).html());
        }
    }
    else {
        $('#radio_connected_' + iter).html('');
    }
}

radioboxControls.prototype.ShowOptionsAll = function() {
  var ctlId = this.id;
  $.each(this.optionData, function (key, value) {
      $("#_" + ctlId + "_" + value.Id).parent().show();
//    $("#_" + ctlId + "_" + value.Id).show();
  });
  
}

radioboxControls.prototype.HideOptions = function(optionsToHide) {
  var options = optionsToHide.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().hide();
  //  $("#_" + ctlId + "_" + value).hide();
  });
}

radioboxControls.prototype.ShowOptions = function(optionsToShow) {
  var options = optionsToShow.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().show();
  //  $("#_" + ctlId + "_" + value).show();
  });
}



radioboxControls.prototype.HideOptionsAll = function() {
  var ctlId = this.id;
  $.each(this.optionData, function (key, value) {
      $("#_" + ctlId + "_" + value.Id).parent().hide();
  //      $("#_" + ctlId + "_" + value.Id).hide();
  });
}




/****************Hide Questions***********/

/*
radioboxControls.prototype.ShowOptionsAll = function() {
  var ctlId = this.id;
  $.each(this.optionData, function (key, value) {
      $("#_" + ctlId + "_" + value.Id).parent().show();
  });
  
} */

radioboxControls.prototype.HideQuestions = function(id) {
//  var options = optionsToHide.split(',');
 // var ctlId = this.id;
//  $.each(options, function (key, value) {
      $("#field_" + id).hide();
 // });
}

/*
radioboxControls.prototype.ShowOptions = function(optionsToHide) {
  var options = optionsToHide.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().show();
  });
}

*/


radioboxControls.prototype.setValues = function (value) {
    $('#_'+this.id+'_'+value).prop('checked',true);
}