function singlechoiceGridRadioControl (fno, id, title, optionData, topicData, orientation) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice2";
    this.optionData = optionData;       //Options
    this.topicData = topicData;         //Options breakdown
    this.value = "";
    this.optionValue = [];

    if (!orientation) {
        if (this.optionData != null) {
            this.optionValue = new Array(this.optionData.length);
        } else {
            this.optionData = [];
        }
        this.orientation = "H";     //Buttons will be in a Vertical group
    } else {
        if (this.topicData != null) {
            this.optionValue = new Array(this.topicData.length);
        } else {
            this.topicData = [];
        }
        this.orientation = orientation;
    }
}

singlechoiceGridRadioControl.prototype = Object.create(formControl.prototype); 
singlechoiceGridRadioControl.prototype.constructor = singlechoiceGridRadioControl;

singlechoiceGridRadioControl.prototype.display = function() {
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    questions=this.optionData;
    topics = this.topicData;

    rId=this.id;
    rName=this.name;
    rType=this.type;
    qno=this.qno;
    var orientation = this.orientation;
    var groupName;

    var field = this.displayLabel();
    field += '<div>';

    if (sf.ShortWidth) {
        if (questions != null) {
            $.each(questions, function (keyQ, valueQ) {

                this.Visible = true;        //Init flag
                field += '<div id="srcTR_' + qno + '_' + valueQ['Id'] + '"><label class="sh_label">' + valueQ['Option'] + '</label>';

                if (topics != null) {
                    $.each(topics, function (keyR, valueR) {

                        var currentMultiRadioTDId = 'srcTD_' + qno + '_' + valueR['Id'];

                        var currentMultiRadioId = rId + '_' + valueR['Id'] + keyQ;
                        if (orientation == "V") {
                            groupName = keyR;
                        } else {
                            groupName = keyQ;
                        }
                        field += '<div><input class="gridSWRadio" type="radio" name="' + rName + groupName + '" id="' + currentMultiRadioId + '" onClick="singlechoiceGridRadioControl.prototype.getValues(this,' + qno + ',' + keyQ + ')" value="' + valueR['Id'] + '"><label for="' + currentMultiRadioId + '" class="cus-label">' + valueR['Option'] + '</label></div>';
                    });
                }
                field += '</div>';
            });
        }
    } else {

        field += '<table border="1">';
        field += '<th>';

        if (topics != null) {
            if (topics != null) {
                $.each(topics, function (keyL, valueL) {
                    this.Visible = true;        //Init flag
                    var currentMultiRadioTHId = 'srcTH_' + qno + '_' + valueL['Id'];
                    field += '<td class="rdr-hdng" id="' + currentMultiRadioTHId + '"><label>' + valueL['Option'] + '</label></td>';

                });
            }
        }
        field += '</th>';

        if (questions != null) {
            $.each(questions, function (keyQ, valueQ) {

                this.Visible = true;        //Init flag
                field += '<tr id="srcTR_' + qno + '_' + valueQ['Id'] + '"><td>' + valueQ['Option'] + '</td>';

                if (topics != null) {
                    $.each(topics, function (keyR, valueR) {

                        var currentMultiRadioTDId = 'srcTD_' + qno + '_' + valueR['Id'];

                        var currentMultiRadioId = rId + '_' + valueR['Id'] + keyQ;
                        if (orientation == "V") {
                            groupName = keyR;
                        } else {
                            groupName = keyQ;
                        }
                        field += '<td align="center" class="' + currentMultiRadioTDId + '"><div><input type="radio" name="' + rName + groupName + '" id="' + currentMultiRadioId + '" onClick="singlechoiceGridRadioControl.prototype.getValues(this,' + qno + ',' + keyQ + ')" value="' + valueR['Id'] + '"><label for="' + currentMultiRadioId + '" class="cus-label"><span></span></label></div></td>';
                    });
                }
                field += '</tr>';
            });
        }

        field += '</table>';
    }
    field += '</div>';

    return field;
}


singlechoiceGridRadioControl.prototype.getValues= function(obj,qno,subQnoId) {
    var opt = buildData.varFormControls[qno].optionValue;

    //Remove previous selection from array if orientation is Vertical
    if (buildData.varFormControls[qno].orientation == "V") {

        buildData.varFormControls[qno].optionValue[parseInt(obj.value)-1] = subQnoId;
        /*for (i = 0; i < opt.length; i++) {
            if (opt[i].length > 0) {
                var topics = opt[i].split("|");
                for (t = 0; t < topics.length; t++) {
                    if (topics[t] == obj.value) {
                        topics.splice(t, 1);
                        opt[i] = topics.join("|");
                        break;
                    }
                }
            }
        }*/
    } else {
        buildData.varFormControls[qno].optionValue[subQnoId] = obj.value;
    }

    buildData.varFormControls[qno].value = buildData.varFormControls[qno].optionValue.join(",");

/*   $('#'+multiRadioId).parent().css( "background-color", "red" ); */

//    obj.find('td').css('background-color', '#00ff00');

  //  $("td#"+multiRadioId).css('background-color','#00ff00');
  //  $("#"+multiRadioId).attr('bgcolor','#00ff00');
}


singlechoiceGridRadioControl.prototype.onValidate = function () {
    if (this.mandatory) {
        var inpvalues = this.value.split(",");
        var nonempty = 0;
        for (var i = 0; i < inpvalues.length; i++) {
            if (inpvalues[i].trim().length > 0) nonempty++;
        }

        var visibleAnswers = 0;
        if (this.orientation == "V") {
            var localData = this.topicData;
            $.each(this.topicData, function (key, value) {
                if (localData[key].Visible) visibleAnswers++;
            });
        } else {
            var localData = this.optionData;
            $.each(this.optionData, function (key, value) {
                if (localData[key].Visible) visibleAnswers++;
            });
        }
        return visibleAnswers == nonempty;
    }
    return true;
}

singlechoiceGridRadioControl.prototype.GetKey = function (list, search) {
    var foundKey = -1;
    $.each(list, function (key, value) {
        if (value['Id'] == search) {
            foundKey = key;
            return;
        }
    });
    return foundKey;
}

//Hide Show Options/Answers
singlechoiceGridRadioControl.prototype.ShowOptionsAll = function () {
    var index = this.qno;
    var localData = this.optionData;
    $.each(this.optionData, function (key, value) {
        localData[key].Visible = true;
        $("#srcTR_" + index + "_" + value['Id']).show();
    });
}

singlechoiceGridRadioControl.prototype.HideOptions = function (optionsToHide) {
    var index = this.qno;
    var options = optionsToHide.split(',');
    var localData = this.optionData;

    $.each(options, function (key, value) {
        if (value.length > 0) {
            var id = singlechoiceGridRadioControl.prototype.GetKey(localData, value);
            localData[id].Visible = false;
            $("#srcTR_" + index + "_" + value).hide();
        }
    });
}

singlechoiceGridRadioControl.prototype.ShowOptions = function (optionsToShow) {
    var index = this.qno;
    var options = optionsToShow.split(',');
    var localData = this.optionData;

    $.each(options, function (key, value) {
        if (value.length > 0) {
            var id = singlechoiceGridRadioControl.prototype.GetKey(localData, value);
            localData[id].Visible = true;
            $("#srcTR_" + index + "_" + value).show();
        }
    });
}

singlechoiceGridRadioControl.prototype.HideOptionsAll = function () {
    var index = this.qno;
    var localData = this.optionData;

    $.each(this.optionData, function (key, value) {
        localData[key].Visible = false;
        $("#srcTR_" + index + "_" + value['Id']).hide();
    });
}

//Hide Show Topics/Columns
singlechoiceGridRadioControl.prototype.ShowTopicsAll = function () {
    var index = this.qno;
    var localData = this.topicData;

    $.each(this.topicData, function (key, value) {
        localData[key].Visible = true;
        $("#srcTH_" + index + "_" + value['Id']).show();
        $(".srcTD_" + index + "_" + value['Id']).show();
    });
}

singlechoiceGridRadioControl.prototype.HideTopics = function (topicsToHide) {
    var index = this.qno;
    var topics = topicsToHide.split(',');
    var localData = this.topicData;

    $.each(topics, function (key, value) {
        if (value.length > 0) {
            var id = singlechoiceGridRadioControl.prototype.GetKey(localData, value);
            localData[id].Visible = false;
            $("#srcTH_" + index + "_" + value).hide();
            $(".srcTD_" + index + "_" + value).hide();
        }
    });
}

singlechoiceGridRadioControl.prototype.ShowTopics = function (topicsToShow) {
    var index = this.qno;
    var topics = topicsToShow.split(',');
    var localData = this.topicData;

    $.each(topics, function (key, value) {
        if (value.length > 0) {
            var id = singlechoiceGridRadioControl.prototype.GetKey(localData, value);
            localData[id].Visible = true;
            $("#srcTH_" + index + "_" + value).show();
            $(".srcTD_" + index + "_" + value).show();
        }
    });
}

singlechoiceGridRadioControl.prototype.HideTopicsAll = function () {
    var index = this.qno;
    var localData = this.topicData;

    $.each(this.topicData, function (key, value) {
        localData[key].Visible = false;
        $("#srcTH_" + index + "_" + value['Id']).hide();
        $(".srcTD_" + index + "_" + value['Id']).hide();
    });
}

singlechoiceGridRadioControl.prototype.setValues = function (value) {

    this.optionValue = value.split(',');
    var controlId = this.id;
    questions = this.optionData;

    if (questions != null) {
        for(var i = 0; i < this.optionValue.length; i++) {
            $.each(questions, function (keyQ, valueQ) {
                if (keyQ == i && topics != null) {
                    $.each(topics, function (keyR, valueR) {
                        var currentMultiRadioId = controlId + '_' + valueR['Id'] + keyQ;
                        $('#' + currentMultiRadioId).prop('checked', true);
                    });
                }
            });
        }
    }
}
