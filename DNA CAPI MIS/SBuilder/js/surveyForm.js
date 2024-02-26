function surveyForm(jsonData) {
  this.qno = 1;    //Current question being displayed. 1 based index
  this.formData = JSON.parse(jsonData);
  this.formControls = new Array();        //Form objects
  this.formView = '';
  this.connectedFormControls=new Array();
  this.connectedFormView = '';                      //Rendered HTML of all elements
  this.platform = "android";
  this.SurveyMediaUrl = 'http://demo.kcompute.com:8083/Pictures/SurveyMedia/';
  this.SurveyMediaDataUrl = 'http://demo.kcompute.com:8083/Pictures/';

  this.SectionName = '';
  this.sectionView = '';
  this.sectionViewsArr = new Array();
  this.sectNo = 1;
  this.sectionWithControls = new Array();
}


surveyForm.prototype.build = function(mandatory) 
{
    if (this.platform == "android") {
        this.SurveyMediaUrl = android.getSurveyMediaPath();
        this.SurveyMediaDataUrl = android.getSurveyMediaDataPath();
    }

    buildData = new Object();
    buildData.fno = 0;
    buildData.VQno = 0;       //Visible
    buildData.HQno = 0;       //Hidden

    buildData.varFormControls = new Array();        //Form objects
    buildData.varFormView = '';
    
    buildData.varConnectedFormControls = new Array();        //Form objects
    buildData.varConnectedFormView = '';

    var CreateSurveyFormControl = this.CreateSurveyFormControl;

    if (SectionWiseView) {
        varSectionsControl = new Array();
        varSectionWithControl = new Array();

        $.each(this.formData, function (key1, valueSect) {
            varSectionWithControl[key1] = new Array();
            varSectionsControl[key1] = '';

            if (valueSect['SectionName'] != 'undefined') {

                $.each(valueSect['Questions'], function (key, value) {
                    var fControl = CreateSurveyFormControl(buildData, value, mandatory);

                    if (fControl != null || typeof (fControl) != "undefined") {
                        varSectionWithControl[key1][key] = buildData.fno;
                    }

                });
            }
        });

        this.sectionWithControls = varSectionWithControl;
        this.sectionViewsArr = varSectionsControl;
    }
    else {
        $.each(this.formData, function (key, value) {
            CreateSurveyFormControl(buildData, value, mandatory);
        });
    }

    this.formControls = buildData.varFormControls;
    this.formView = buildData.varFormView;

    this.connectedFormControls = buildData.varConnectedFormControls;
    this.connectedFormView = buildData.varConnectedFormView;
}

surveyForm.prototype.CreateSurveyFormControl = function (bd, value, mandatory) {
    var fControl;
    var isVisible = false;
    if (!((typeof value['Visible'] != 'undefined') && (value['Visible'] == false || value['Visible'] == 'false' || value['Visible'] == 'False'))) {
        bd.VQno++;
        bd.fno = bd.VQno;
        fControl = bd.varFormControls[bd.fno];
        isVisible = true;
    } else {
        bd.HQno++;
        bd.fno = bd.HQno;
        fControl = bd.varConnectedFormControls[bd.fno];
    }

    if (value['FieldType'] == 'SLT' || value['FieldType'] == 'phone' || value['FieldType'] == 'email') {
        fControl = new textboxControls(bd.fno, value['Id'], value['FieldName'], true, value['MinLength'], value['MaxLength'], value['NoOfLines']);
    }
    else if (value['FieldType'] == 'XXX' || value['FieldType'] == '') {
        fControl = new bodytext(bd.fno, value['Id'], value['FieldName'], value['MediaFiles']);
    }
    else if (value['FieldType'] == 'CTR') {
        fControl = new numberCounterControls(bd.fno, value['Id'], value['FieldName']);
    }
    else if (value['FieldType'] == 'RGS') {
        fControl = new rangeSliderControl(bd.fno, value['Id'], value['FieldName'], 1, 7, value['Options']);
    }
    else if (value['FieldType'] == 'DAT') {
        fControl = new datepickerControls(bd.fno, value['Id'], value['FieldName']);
    }
    else if (value['FieldType'] == 'NUM') {
        fControl = new integerTextboxControls(bd.fno, value['Id'], value['FieldName'], value['MinLength'], value['MaxLength']);
    }
    else if (value['FieldType'] == 'PIC') {
        fControl = new imageLoadControls(bd.fno, value['Id'], value['FieldName']);
    }
    else if (value['FieldType'] == 'RDO') {
        fControl = new radioboxControls(bd.fno, value['Id'], value['FieldName'], value['Options'], value['Orientation']);

        if ((typeof value['IncludeField'] != 'undefined') && value['IncludeField'] == false) {
            fControl.visible = false;
        }
    }
    else if (value['FieldType'] == 'STW') {
        fControl = new stopwatchControls(bd.fno, value['Id'], value['FieldName']);
    }
    else if (value['FieldType'] == 'DDN') {
        fControl = new dropdownControls(bd.fno, value['Id'], value['FieldName'], value['Options']);
    }
    else if (value['FieldType'] == 'SCD') {
        fControl = new singlechoiceGridControl(bd.fno, value['Id'], value['FieldName'], value['Options'], value['Questions']);
    }
    else if (value['FieldType'] == 'SCG') {
        fControl = new singlechoiceGridRadioControl(bd.fno, value['Id'], value['FieldName'], value['Options'], value['Questions'], value['Orientation']);
    }
    else if (value['FieldType'] == 'MCG') {
        fControl = new multichoiceGridCheckboxControl(bd.fno, value['Id'], value['FieldName'], value['Options'], value['Questions']);
    }
    else if (value['FieldType'] == 'MLT') {
        fControl = new openendedTextControl(bd.fno, value['Id'], value['FieldName'], value['Options']);
    }
    else if (value['FieldType'] == 'ONG') {
        fControl = new openendedIntegerControl(bd.fno, value['Id'], value['FieldName'], value['Options']);
    }
    else if (value['FieldType'] == 'CHK') {
        fControl = new checkboxControls(bd.fno, value['Id'], value['FieldName'], value['Options'], value['Orientation']);
    }
    else if (value['FieldType'] == 'IRT') {               //Image Recognition Test
        fControl = new multiSelectImage(bd.fno, value['Id'], value['FieldName'],
            value['BeforeImage'], value['BeforeImageRows'], value['BeforeImageColumns'], value['BeforeImageTimeout'], value['BeforeImageShuffle'],
            value['AfterImage'], value['AfterImageRows'], value['AfterImageColumns'], value['AfterImageShuffle'],
            value['Images'], value['ImagesToSelect']);
    }

    if (fControl != null || typeof (fControl) != "undefined") {
        if ((typeof value['Mandatory'] != 'undefined') && value['Mandatory'] != null && (value['Mandatory'] == true || value['Mandatory'] == 'TRUE' || value['Mandatory'] == 'true' || value['Mandatory'] == 'True')) {
            fControl.mandatory = true;
        }
        else {
            fControl.mandatory = false;
        }

        if (value['FieldType'] == 'XXX' || value['FieldType'] == '') {
            fControl.mandatory = false;
        }
        else if ((value['FieldType'] == 'RDO' || value['FieldType'] == 'CHK') && (typeof value['Options'] == 'undefined' || value['Options'] == null)) {
            fControl.mandatory = false;
        }

        if (mandatory == "0") {
            fControl.mandatory = false;
        }

        fControl.ScriptOnEntry = surveyForm.prototype.ConvertHTMLSpecialToText(urldecode(value['ScriptOnEntry']));
        fControl.ScriptOnValidate = surveyForm.prototype.ConvertHTMLSpecialToText(urldecode(value['ScriptOnValidate']));
        fControl.ScriptOnExit = surveyForm.prototype.ConvertHTMLSpecialToText(urldecode(value['ScriptOnExit']));
        fControl.SetInstructions(value['Instructions']);

        bd.varFormView += fControl.header(bd.fno) + fControl.display(bd.fno) + fControl.footer(bd.fno);

        if (isVisible) {
            bd.varFormControls[bd.fno] = fControl;
        }
        else {
            bd.varConnectedFormControls[bd.fno] = fControl;
        }
    }
    return fControl;
}

surveyForm.prototype.getSectionQno = function (sectNo) {
    var sectionQno = new Array();

    for (var i = 0; i < this.sectionWithControls[sectNo - 1].length; i++) {
        sectionQno[i] = this.sectionWithControls[sectNo - 1][i];
    }

    return sectionQno;
}

surveyForm.prototype.getSectionNameHeading = function (i) {
    var currentSectIter = this.formData[i - 1]['SectionName'];
    return currentSectIter;
}

surveyForm.prototype.getSectionName = function (i) {
    var currentSectIter = this.formData[i - 1]['SectionName'];
    currentSectIter = currentSectIter.replace(/\s/g, '');

    return currentSectIter;
}

surveyForm.prototype.showError = function (qno) {
    $('.err_msg').text('');
    var id = varFormControls[this.qno].id;
    $('#field_' + id + ' p').text('This field is required.');
}
surveyForm.prototype.showError = function (id, msg) {
    $('.err_msg_field').remove();
    //$('#field_' + id + ' label').first().append('<div class="alert alert-danger err_msg_field" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span>' + msg + '</div>');
}

surveyForm.prototype.setPage = function (pageNo) {
    $('.counter_text').text(pageNo + ' of ' + this.formControls.length);
    this.qno = pageNo;
}

surveyForm.prototype.NextPage = function () {
    if (SectionWiseView) {
        var thisForm = this;

        this.getSectionQno(1);
        this.qno = 1;
        var oktoproceed = true;

        $.each(this.getSectionQno(this.sectNo), function (index, value) {
            //if (index > 0) {
                $('.err_msg_field').remove();
                if (thisForm.validate(value) && thisForm.formControls[value].onValidate()) {
                    //$('#field_' + thisForm.formControls[value].id + ' label').first().append('');
                    $('#field_' + thisForm.formControls[value].id + ' label').css('color', '');
                }
                else {
                    //$('#field_' + thisForm.formControls[value].id + ' label').first().append('<div class="alert alert-danger err_msg_field" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span>This field is required.!</div>');
                    $('#field_' + thisForm.formControls[value].id + ' label').css('color', 'red');
                    oktoproceed = false;
                    return false;
                }
            //}
        });
        if (oktoproceed) {
            if (this.sectNo < this.formData.length) {
                this.sectNo++;
                this.refreshPage();
            }
            else {
                this.saveData();
            }
        }
    }
    else {
        //Validate current page
        if (this.validate(this.qno) && this.formControls[this.qno].onValidate()) {
            if (this.qno <= (this.formControls.length - 2)) {

                var jumpQ = this.formControls[this.qno].onExit();
                this.formControls[this.qno].nextqno = jumpQ;
                var currentQ = this.qno;

                this.qno += jumpQ;

                this.refreshPage();

                this.formControls[this.qno].prevqno = currentQ;
            }

            else {
                this.saveData();
            }
        }
    }
}

surveyForm.prototype.PrevPage = function () {
    if (SectionWiseView) {
        if (this.sectNo > 1) {
            this.sectNo--;
            this.refreshPage();
        }
    }
    else {
        if (this.qno > 1) {
            if (this.formControls[this.qno].prevqno != null) {
                var prevQ = this.formControls[this.qno].prevqno;
				if (prevQ == this.qno) prevQ--;
                this.qno = prevQ;
            }
            else {
                this.qno--;
            }

            this.refreshPage();
        }
    }
}

surveyForm.prototype.GetValue = function (id) {

    var matchedValue = '';

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                matchedValue = control.value;
				return false;
            }
        }
    });

    return matchedValue;
}

surveyForm.prototype.SetValue = function (id, value) {

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                control.setValues(value);
				return false;
            }
        }
    });
}

surveyForm.prototype.SetInstructions = function (id, text) {
    if (text) {
        $.each(this.formControls, function (index, control) {

            if (control && index > 0) {

                if (id == control.id) {
                    control.SetInstructions(text);
                    control.DisplayInstructions();
					return false;
                }
            }
        });
    }
}

surveyForm.prototype.Getqno = function(id) {

    var matchedValue = -1;

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                matchedValue = control.qno;
				return false;
            }
        }
    });

    return matchedValue;
}

surveyForm.prototype.HideOptions = function (id, optionsToHide) {

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                control.HideOptions(optionsToHide);
				return false;
            }
        }
    });
}

surveyForm.prototype.ShowOptions = function (id, optionsToHide) {

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                control.ShowOptions(optionsToHide);
				return false;
            }
        }
    });
}

surveyForm.prototype.ShowOptionsAll = function (id) {
    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                control.ShowOptionsAll();
				return false;
            }
        }
    });
}

surveyForm.prototype.HideOptionsAll = function (id) {
    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                control.HideOptionsAll();
				return false;
            }
        }
    });
}

surveyForm.prototype.HideAllQuestions = function (id, iter) {

    var pageMove = this.qno;
    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                // this.qno=control.qno+1;
                pageMove = pageMove + iter;
                // sf.setPage(control.qno);
                //  control.HideQuestions(id);
				return false;
            }
        }
    });

    this.qno = pageMove;
    // sf.refreshPage();
}


surveyForm.prototype.number_validation = function (id, min, max) {

    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                control.number_validation(min, max);
				return false;
            }
        }
    });
}


surveyForm.prototype.ShowQuestions = function (id) {

    var pageMove = this.qno;
    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                pageMove = control.qno;
				return false;
            }
        }
    });

    this.qno = pageMove;
    sf.refreshPage();

}

surveyForm.prototype.BackQuestions = function (id) {

    var pageMove = this.qno;
    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (control && index > 0) {

            if (id == control.id) {
                // this.qno=control.qno+1;
                pageMove = pageMove - 1;
                // sf.setPage(control.qno);
                //  control.HideQuestions(id);
				return false;
            }
        }
    });

    this.qno = pageMove;
    // sf.refreshPage();
}


surveyForm.prototype.hideQuestion = function (id) {

    var currentQno = this.Getqno(id);
    this.qno = currentQno + 1;

    this.refreshPage();
}


surveyForm.prototype.refreshPage = function () {
    if (SectionWiseView) {
        //OnEntry of all the controls in the section
        $.each(this.getSectionQno(this.sectNo), function (index, value) {
            try {
                if (typeof (value) != "undefined" && typeof (buildData.varFormControls[value]) != "undefined") {
                    buildData.varFormControls[value].onEntry();
                }
            }
            catch (err) {
                sf.alert("On refreshPage.getSectionQno: " + this.sectNo + ", " + index + ", " + value + ".\n\r" + err.stack);
            }
        });

        $('.counter_text').html('<span>' + this.sectNo + '</span> of ' + (this.formData.length));
        $('.formControl').hide();

        //Render all the controls in the section
        $.each(this.getSectionQno(this.sectNo), function (index, value) {
            try {
                if (typeof (value) != "undefined" && typeof (buildData.varFormControls[value]) != "undefined") {
                    var name = buildData.varFormControls[value].id;
                    $('.formControl#field_' + name).show();
                }
            }
            catch (err) {
                sf.alert("On refreshPage.getSectionQno2: " + this.sectNo + ", " + index + ", " + value + ".\n\r" + err.stack);
            }
        });

        $('#section_name').text(this.getSectionNameHeading(this.sectNo));
    }
    else {
        if (this.formControls[this.qno]) {
            try {
                this.formControls[this.qno].onEntry();
            } catch (e) {
                alert("OnEntry fails for control index: " + this.qno + ". Stack trace: " + e.stack);
            }

            $('.counter_text').text(this.qno + ' of ' + (this.formControls.length - 1));
            $('.formControl').hide();
            var name = this.formControls[this.qno].id;
            $('.formControl#field_' + name).show();
        }
    }
}

surveyForm.prototype.validate = function (index) {
    if (this.formControls[index].mandatory && (this.formControls[index].value == null || this.formControls[index].value == '')) {
        this.alert("Please input values");
        return false;
    }
    return true;
}

surveyForm.prototype.alert = function (message) {
    if (this.platform == "android") {
        android.alert(message);
    }
    else if (this.platform == "web") {
        alert(message);
    }
}

surveyForm.prototype.HideBackButton = function () {
    $('.backButton').hide();
}
surveyForm.prototype.ShowBackButton = function () {
    $('.backButton').show();
}
surveyForm.prototype.HideForwardButton = function () {
    $('.forwardButton').hide();
}
surveyForm.prototype.ShowForwardButton = function () {
    $('.forwardButton').show();
}
surveyForm.prototype.HideCounters = function () {
    $('.counter_text').hide();
}
surveyForm.prototype.ShowCounters = function () {
    $('.counter_text').show();
}

//Filter Options (Answers) by value of provided question
surveyForm.prototype.FilterAnswersByValue = function (prevQId) {
    var currentQ = this.formControls[this.qno];
    if (typeof (currentQ.optionData) != "undefined" && currentQ.optionData != null && currentQ.optionData.length > 0) {
        
        var prevQno = this.Getqno(prevQId);
        if (prevQno >= 0) {
            try {
                currentQ.HideOptionsAll();

                var prevQ = this.formControls[prevQno];
                if (prevQ.value != null && prevQ.value.length > 0) {
                    currentQ.ShowOptions(prevQ.value);
                }
            } catch (e) {
            }
        }
    }
}

//Filter Options (Topics) by value of provided question
surveyForm.prototype.FilterTopicsByValue = function (prevQId) {
    var currentQ = this.formControls[this.qno];
    if (typeof (currentQ.topicData) != "undefined" && currentQ.topicData != null && currentQ.topicData.length > 0) {

        var prevQno = this.Getqno(prevQId);
        if (prevQno >= 0) {
            try {
                currentQ.HideTopicsAll();

                var prevQ = this.formControls[prevQno];
                if (prevQ.value != null && prevQ.value.length > 0) {
                    currentQ.ShowTopics(prevQ.value);
                }
            } catch (e) {
            }
        }
    }
}

/*

android.getFormData(this.savePartialFormData());



surveyForm.prototype.savePartialFormData = function() {

  var finalData=[];

for (var i = 0; i <= varFormControls.length-2; i++) {
    finalData[i]=new Object(); 
    finalData[i].id=varFormControls[i+1].id;
    finalData[i].value=varFormControls[i+1].value;
};




 var save_final_data=JSON.stringify(finalData);
// console.log();

  return save_final_data;

}

*/



surveyForm.prototype.saveData = function () {

    var finalData = [];

    var conntectedFormBeginPointer = 0;

    for (var i = 0; i <= this.formControls.length - 2; i++) {
        finalData[i] = new Object();
        finalData[i].id = this.formControls[i + 1].id;
        finalData[i].value = this.formControls[i + 1].value;
        finalData[i].ResponseTime = this.formControls[i + 1].ResponseTime;
    };


    for (var i = 1; i < this.connectedFormControls.length; i++) {
        finalData[this.formControls.length + (i - 2)] = new Object();
        finalData[this.formControls.length + (i - 2)].id = this.connectedFormControls[i].id;
        finalData[this.formControls.length + (i - 2)].value = this.connectedFormControls[i].value;
        finalData[this.formControls.length + (i - 2)].ResponseTime = this.connectedFormControls[i].ResponseTime;
    };

    var save_final_data = JSON.stringify(finalData);

    console.log(save_final_data);
    if (typeof (android) != "undefined" && android != null) {
        android.setData(save_final_data);
    } 
    else if (typeof (OnlineSurvey) != "undefined" && OnlineSurvey != null) {
        OnlineSurveyCallback(save_final_data);
    }
}




/*
surveyForm.prototype.setNone = function(index) {

  if(varFormControls[index].type=="checkbox" && varFormControls[index].noneSelection) {
    varFormControls[index].noneSelection=true;
  }

}
*/

//setNone();


surveyForm.prototype.setControlValue = function (id, filename) {

    var formPointer = this.connectedFormControls;

    $.each(formPointer, function (index, control) {
        try {
            if (index >= 0) {
                if (id == control.id) {
                    formPointer[control.qno].value = filename;
                    formPointer[control.qno].setValues(filename);
                }
            }
        } catch (e) {
        }
    });

    formPointer = this.formControls;

    $.each(formPointer, function (index, control) {
        try {
            if (index >= 0) {
                if (id == control.id) {
                    formPointer[control.qno].value = filename;
                    formPointer[control.qno].setValues(filename);
                }
            }
        } catch (e) {
        }
    });
}

surveyForm.prototype.updateValues = function(updateValues) 
{
  var updatedValues=JSON.parse(updateValues);
  var varFormControls = this.formControls;

  $.each(updatedValues, function(key, value) {
      var qno = sf.Getqno(value.FieldId);
      if (qno >= 0 && qno < varFormControls.length) {
          varFormControls[qno].value = value.FieldValue;
          varFormControls[qno].setValues(value.FieldValue);
      }
  });

  sf.refreshPage();
}

surveyForm.prototype.ConvertHTMLSpecialToText = function(str) {
    str = surveyForm.prototype.replaceAll(str, "&amp;", "&");
    str = surveyForm.prototype.replaceAll(str, "&gt;", ">");
    str = surveyForm.prototype.replaceAll(str, "&lt;", "<");
    str = surveyForm.prototype.replaceAll(str, "&quot;", "\"");
    str = surveyForm.prototype.replaceAll(str, "&#039;", "'");
    str = surveyForm.prototype.replaceAll(str, "&#39;", "'");
    return str;
}

surveyForm.prototype.replaceAll = function(string, find, replace) {
    return string.replace(new RegExp(surveyForm.prototype.escapeRegExp(find), 'g'), replace);
}

surveyForm.prototype.escapeRegExp = function(str) {
    return str.replace(/[-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
}

