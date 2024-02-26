function checkboxControls(fno, id, title, optionData, orientation) {
    formControl.call(this, fno, id, title);
    this.type = "checkbox";
    this.value = '';
    this.name = id;
    this.optionData = optionData;

    if (optionData != null) {
        $.each(this.optionData, function (index, value) {
            this.value = 0;
            if (typeof value['Type'] != 'undefined') {
                this.type = value['Type'];
            }

            //else if (id == 2842 && index == 3) {
            //    this.type = 'NONE';
            //}

            else {
                this.type = '';
            }
        });


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

checkboxControls.prototype = Object.create(formControl.prototype);
checkboxControls.prototype.constructor = checkboxControls;

checkboxControls.prototype.display = function (index) {
    var field = this.displayLabel(index);

    rId = this.id;
    rName = this.name;
    rType = this.type;
    var fldNo = this.qno;
    var oclass = "";
    if (this.orientation == "H") {
        oclass = "horizontal";
    }

    this.optionData;

    if (this.optionData != null) {
        var maxWidth = parseInt(100 / this.optionData.length);
        if (maxWidth == 0 || this.orientation == "V") maxWidth = 80;

        field += '<div class="mmchkbx ' + oclass + '">';

        $.each(this.optionData, function (key, value) {
            field += '<div style="width:' + maxWidth + '%;">';
            if (typeof (value["MediaFiles"]) != "undefined" && value["MediaFiles"] != null) {
                $.each(value["MediaFiles"], function (mfkey, mfvalue) {

                    if (mfvalue["FileType"] == "Image") {
                        field += '<img src="http://capimis.apps.dna.com.sa/Pictures/SurveyMedia/' + mfvalue["FileName"] + '" style="margin:0 auto;max-width:80%;max-height:80%;' + (oclass == "horizontal" ? "" : "float:left;") + '" />';
                    }
                });
            }

            if (typeof value['Type'] == 'undefined') {
                field += '<input type="' + rType + '" name="' + rName + '" id="field_' + rId + '_' + value['Id'] + '" value="' + value['Id'] + '" onClick="checkboxControls.prototype.onChange(this, ' + fldNo + ',' + key + ')"><label for="field_' + rId + '_' + value['Id'] + '" class="cus-label"><span></span><p style="width:80%;">' + value['Option'] + '</p></label></div>';
            }
            else {
                field += '<input type="' + rType + '" name="' + rName + '" id="field_' + rId + '_' + value['Id'] + '" onClick="checkboxControls.prototype.onChange(this, ' + fldNo + ',' + key + ')"><label for="field_' + rId + '_' + value['Id'] + '" class="cus-label"><span></span><p style="width:80%;">' + value['Option'] + '</p></label></div>';
            }
        });
        field += '</div>';
        field += '<div style="margin-bottom: 5px;"></div>';
    }
    field += '<div id="check_connected_' + fldNo + '"></div>';

    return field;
}


checkboxControls.prototype.onChange = function (obj, fldNo, itemNo) {

    var Ids = $('#field_' + buildData.varFormControls[fldNo].id + '_' + buildData.varFormControls[fldNo].optionData[itemNo].Id);

    var fieldName = buildData.varFormControls[fldNo].id;

    var optionChecked = Ids.is(":checked");

    var actionType = buildData.varFormControls[fldNo].optionData[itemNo].type;

    var actionMove = buildData.varFormControls[fldNo].optionData[itemNo].connectedField;

    for (var i = 0 ; i < res.length - 1; i++) {
        chk_value[res[i]] = 1;
    };


    /*	$(".formControl input[type='checkbox']").is(':checked');

		var values = "";
 		$.each(this.optionData,function(index,value) {
 		//	if (this.chk_value[index] == 1) {
 				values += value['Id']+",";
 		//	}
 		});
 		this.value = values;
	*/

    //$(".formControl input[type='checkbox']").on('change',function() {


    if (actionMove != "") {

        if (optionChecked) {

            $('#check_connected_' + fldNo).html($('#field_' + actionMove).html());
            //Uncheck all
        }

        else {
            $('#check_connected_' + fldNo).html('');

        }

    }


    if (actionType == "NONE") {
        if (optionChecked) {
            $('input[name="' + fieldName + '"]').prop('checked', false);
            $('input[name="' + fieldName + '"]').prop('disabled', true);
            Ids.prop('checked', true);
            Ids.prop('disabled', false);
            this.flushValues(fldNo);

            //Uncheck all
        }
        else {
            $('input[name="' + fieldName + '"]').prop('checked', false);
            $('input[name="' + fieldName + '"]').prop('disabled', false);
            //Do nothing
        }
    } if (actionType == "ALL") {
        if (optionChecked) {
            this.getValues(fldNo, fieldName);
            $('input[name="' + fieldName + '"]').prop('checked', true);

            //Check all
        }
        else {
            this.flushValues(fldNo);
            $('input[name="' + fieldName + '"]').prop('checked', false);
            //Uncheck all
        }
    }


    else {
        //if (optionChecked) {

        this.getValues(fldNo, fieldName);

        //Uncheck checkbox of type NONE
        //}			
        //	else {
        //Uncheck checkbox of type ALL
        //	}

    }

    //	}); 	

    /*	var values = "";
 		checkboxControls.prototype.optionData.each() {
 			if (this.value = 1) {
 				values += this.id+",";
 			}
 		}
 		checkboxControls.prototype.value = values;

		if (checkboxControls.prototype.optionData[this.id].type == "NONE") {
        	if (this.checked) {
        		//Uncheck all
			}			
			else {
				//Do nothing
			}
		} else if (checkboxControls.prototype.optionData[this.id].type == "ALL") {
        	if (this.checked) {
        		//Check all
			}			
			else {
				//Uncheck all
			}
		} else {
        	if (this.checked) {
        		//Uncheck checkbox of type NONE
			}			
			else {
				//Uncheck checkbox of type ALL
			}

		}
	*/
    /*      if (this.checked) {
            $('input[name="'+this.name+'"]').prop('checked',false);
            $('input[name="'+this.name+'"]').prop('disabled',true);
            $('#'+this.id).prop('checked',true);
            $('#'+this.id).prop('disabled',false);        
          }

          else {
            $('input[name="'+this.name+'"]').prop('checked',false);
            $('input[name="'+this.name+'"]').prop('disabled',false);
          }
      */
    /*	this.values=value;
       this.chk_value=chk_value; */
}

checkboxControls.prototype.getValues = function (index, name) {
    buildData.varFormControls[index].value = '';
    $.each($('input[name="' + name + '"]:checked'), function (iter, value) {
        //	console.log(value.val());
        buildData.varFormControls[index].value += this.value + ',';
    });
}


checkboxControls.prototype.flushValues = function (index) {
    buildData.varFormControls[index].value = '';
}

checkboxControls.prototype.setValues = function (value) {

    var getValues = value.split(',');
    var controlId = this.id;

    $.each(getValues, function (iter, conValue) {
        $('#field_' + controlId + '_' + conValue).prop('checked', true);
    });

}

checkboxControls.prototype.ShowOptionsAll = function () {
    var ctlId = this.id;
    $.each(this.optionData, function (key, value) {
        $("#field_" + ctlId + "_" + value.Id).parent().show();
        //    $("#_" + ctlId + "_" + value.Id).show();
    });

}

checkboxControls.prototype.HideOptions = function (optionsToHide) {
    var options = optionsToHide.split(',');
    var ctlId = this.id;
    $.each(options, function (key, value) {
        $("#field_" + ctlId + "_" + value).parent().hide();
        //  $("#_" + ctlId + "_" + value).hide();
    });
}

checkboxControls.prototype.ShowOptions = function (optionsToShow) {
    var options = optionsToShow.split(',');
    var ctlId = this.id;
    $.each(options, function (key, value) {
        $("#field_" + ctlId + "_" + value).parent().show();
        //  $("#_" + ctlId + "_" + value).show();
    });
}

checkboxControls.prototype.HideOptionsAll = function () {
    var ctlId = this.id;
    $.each(this.optionData, function (key, value) {
        $("#field_" + ctlId + "_" + value.Id).parent().hide();
        //      $("#_" + ctlId + "_" + value.Id).hide();
    });
}

checkboxControls.prototype.HideQuestions = function (id) {
    //  var options = optionsToHide.split(',');
    // var ctlId = this.id;
    //  $.each(options, function (key, value) {
    $("#field_" + id).hide();
    // });
}
