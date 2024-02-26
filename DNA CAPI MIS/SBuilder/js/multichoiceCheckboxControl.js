function multichoiceGridCheckboxControl (fno, id, title, optionData, topicData) {
    formControl.call(this, fno, id, title);
    this.type = "gridmultichoice2";
    this.optionData = optionData;
    this.topicData = topicData;
    this.value=[];
}

multichoiceGridCheckboxControl.prototype = Object.create(formControl.prototype); 
multichoiceGridCheckboxControl.prototype.constructor = multichoiceGridCheckboxControl;

multichoiceGridCheckboxControl.prototype.display = function() {
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    questions = this.optionData;
    topics = this.topicData;

    rId=this.id;
    rName=this.name;
    rType=this.type;
    qno=this.qno;

    var field = this.displayLabel();
    field += '<div>';
    field += '<table border="1">';
    field+='<th>';

    if (topics != null) {
        $.each(topics, function (keyL, valueL) {
            var currentMultiRadioTHId = 'mcTH_' + qno + '_' + valueL['Id'];
            field += '<td id="' + currentMultiRadioTHId + '"><label>' + valueL['Option'] + '</label></td>';
        });
    }
    field+='</th>';

    if (questions != null) {
        $.each(questions, function (keyQ, valueQ) {

            field += '<tr id="mcTR_' + qno + '_' + valueQ['Id'] + '"><td>' + valueQ['FieldName'] + '</td>';

            if (topics != null) {
                $.each(topics, function (keyR, valueR) {
                    var currentMultiChoiceTDId = 'mcTD_' + qno + '_' + valueR['Id'];

                    field += '<td class="' + currentMultiChoiceTDId + '"><div><input type="checkbox" name="' + rName + keyQ + '" id="' + rId + '_' + valueR['Id'] + keyQ + '" onClick="multichoiceGridCheckboxControl.prototype.onChange(this,' + qno + ',' + keyQ + ')" value="' + valueR['Id'] + '"><label for="' + rId + '_' + valueR['Id'] + keyQ + '" class="cus-label"><span></span></label></div></td>';
                });
            }
            field += '</tr>';
        });
    }

    field+='</table>';
    field += '</div>';
    return field;
}


multichoiceGridCheckboxControl.prototype.onChange = function (obj, subQId, keyQ) {

    gridMultiCheckBoxValues = new Object();
    gridMultiCheckBoxValues.Id = keyQ;

    if (typeof buildData.varFormControls[subQId].value[keyQ] != 'undefined') {
        var tempValue = buildData.varFormControls[subQId].value[keyQ].Value;
        gridMultiCheckBoxValues.Value = tempValue + ',' + obj.value;
    }
    else {
        gridMultiCheckBoxValues.Value = obj.value;
    }

    buildData.varFormControls[subQId].value[keyQ] = gridMultiCheckBoxValues;
}

//Hide Show Options/Answers
multichoiceGridCheckboxControl.prototype.ShowOptionsAll = function () {
    $.each(this.optionData, function (key, value) {
        $("#mcTR_" + this.qno + "_" + value['Id']).show();
    });
}

multichoiceGridCheckboxControl.prototype.HideOptions = function (optionsToHide) {
    var options = optionsToHide.split(',');
    $.each(options, function (key, value) {
        if (value.length > 0) {
            $("#mcTR_" + this.qno + "_" + value).hide();
        }
    });
}

multichoiceGridCheckboxControl.prototype.ShowOptions = function (optionsToShow) {
    var options = optionsToShow.split(',');
    $.each(options, function (key, value) {
        if (value.length > 0) {
            $("#mcTR_" + this.qno + "_" + value).show();
        }
    });
}

multichoiceGridCheckboxControl.prototype.HideOptionsAll = function () {
    $.each(this.optionData, function (key, value) {
        $("#mcTR_" + this.qno + "_" + value['Id']).hide();
    });
}

//Hide Show Topics/Columns
multichoiceGridCheckboxControl.prototype.ShowTopicsAll = function () {
    $.each(this.topicData, function (key, value) {
        $("#mcTH_" + this.qno + "_" + value['Id']).show();
        $(".mcTD_" + this.qno + "_" + value['Id']).show();
    });
}

multichoiceGridCheckboxControl.prototype.HideTopics = function (topicsToHide) {
    var topics = topicsToHide.split(',');
    $.each(topics, function (key, value) {
        if (value.length > 0) {
            $("#mcTH_" + this.qno + "_" + value).hide();
            $(".mcTD_" + this.qno + "_" + value).hide();
        }
    });
}

multichoiceGridCheckboxControl.prototype.ShowTopics = function (topicsToShow) {
    var topics = topicsToShow.split(',');
    $.each(topics, function (key, value) {
        if (value.length > 0) {
            $("#mcTH_" + this.qno + "_" + value).show();
            $(".mcTD_" + this.qno + "_" + value).show();
        }
    });
}

multichoiceGridCheckboxControl.prototype.HideTopicsAll = function () {
    $.each(this.topicData, function (key, value) {
        $("#mcTH_" + this.qno + "_" + value['Id']).hide();
        $(".mcTD_" + this.qno + "_" + value['Id']).hide();
    });
}

multichoiceGridCheckboxControl.prototype.setValues = function (value) {
    if (value != null && value.length > 0) {
        var getValues = value.split(',');
        var controlId = this.id;

        $.each(getValues, function (iter, conValue) {
            $('#field_' + controlId + '_' + conValue).prop('checked', true);
        });
    }
}
