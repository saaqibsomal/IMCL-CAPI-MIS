function surveyForm(jsonData) {
  this.qno = 1;    //Current question being displayed. 1 based index
  this.formData = JSON.parse(jsonData);
  this.formControls = new Array();        //Form objects
  this.formView = '';
  this.connectedFormControls=new Array();
  this.connectedFormView = '';                      //Rendered HTML of all elements
  this.platform = "android";
  this.SurveyMediaUrl = 'SurveyMedia/';
}


surveyForm.prototype.build = function(mandatory) 
{
    if (this.platform == "web") {
        this.SurveyMediaUrl = 'http://capimis.apps.dna.com.sa/Pictures/SurveyMedia/';
    }

    varFormControls = new Array();        //Form objects
    varFormView = '';
    
    varConnectedFormControls = new Array();        //Form objects
    varConnectedFormView = '';

    var fno=0;
    var VQno = 0;       //Visible
    var HQno = 0;       //Hidden

    $.each(this.formData, function(key, value) {
        var fControl;
        var isVisible = false;
        if (!((typeof value['Visible'] != 'undefined') && (value['Visible'] == false || value['Visible'] == 'false' || value['Visible'] == 'False'))) {
            VQno++;
            fno = VQno;
            fControl = varFormControls[fno];
            isVisible = true;
        } else {
            HQno++;
            fno = HQno;
            fControl = varConnectedFormControls[fno];
        }

        if(value['FieldType']=='SLT' || value['FieldType']=='phone' || value['FieldType']=='email') {
            fControl = new textboxControls(fno, value['Id'], value['FieldName'], true, value['MinLength'], value['MaxLength'], value['NoOfLines']);
        } 
        else if(value['FieldType']=='XXX' || value['FieldType']=='') {
            fControl = new bodytext(fno, value['Id'], value['FieldName'], value['MediaFiles']);
        }
        else if(value['FieldType']=='CTR') {
            fControl = new numberCounterControls(fno, value['Id'], value['FieldName']);
        }
        else if(value['FieldType']=='RGS') {
            fControl = new rangeSliderControl(fno, value['Id'], value['FieldName'], 1, 7, value['Options']);
        }
        else if(value['FieldType']=='DAT') {
            fControl = new datepickerControls(fno, value['Id'], value['FieldName']);
        }
        else if (value['FieldType']=='NUM') {
            fControl = new integerTextboxControls(fno, value['Id'], value['FieldName'], value['MinLength'], value['MaxLength']);
        }
        else if (value['FieldType']=='PIC') {
            fControl = new imageLoadControls(fno, value['Id'], value['FieldName']);
        }
        else if(value['FieldType']=='RDO') {
            fControl = new radioboxControls(fno, value['Id'], value['FieldName'], value['Options'], value['Orientation']);

            if ((typeof value['IncludeField']!='undefined') && value['IncludeField']==false) {
                fControl.visible=false;
            }      
        } 
        else if(value['FieldType']=='STW') {
            fControl = new stopwatchControls(fno, value['Id'], value['FieldName']);
        } 
        else if(value['FieldType']=='DDN') {
            fControl = new dropdownControls(fno, value['Id'], value['FieldName'], value['Options']);
        } 
        else if(value['FieldType']=='SCD') {
            fControl = new singlechoiceGridControl(fno, value['Id'], value['FieldName'], value['Options'], value['Questions']);
        } 
        else if(value['FieldType']=='SCG') {
            fControl = new singlechoiceGridRadioControl(fno, value['Id'], value['FieldName'], value['Options'], value['Questions'], value['Orientation']);
        } 
        else if(value['FieldType']=='MCG') {
            fControl = new multichoiceGridCheckboxControl(fno, value['Id'], value['FieldName'], value['Options'], value['Questions']);
        }
        else if(value['FieldType']=='MLT') {
            fControl = new openendedTextControl(fno, value['Id'], value['FieldName'], value['Options']);
        }
        else if(value['FieldType']=='ONG') {
            fControl = new openendedIntegerControl(fno, value['Id'], value['FieldName'], value['Options']);
        }
        else if(value['FieldType']=='CHK') {
            fControl = new checkboxControls(fno, value['Id'], value['FieldName'], value['Options'], value['Orientation']);
        } 
        else if (value['FieldType'] == 'IRT') {               //Image Recognition Test
            fControl = new multiSelectImage(fno, value['Id'], value['FieldName'],
                value['BeforeImage'], value['BeforeImageRows'], value['BeforeImageColumns'], value['BeforeImageTimeout'], value['BeforeImageShuffle'],
                value['AfterImage'], value['AfterImageRows'], value['AfterImageColumns'], value['AfterImageShuffle'],
                value['Images'], value['ImagesToSelect']);
        }

        if ((typeof value['Mandatory'] != 'undefined') && value['Mandatory'] != null && (value['Mandatory'] == true || value['Mandatory'] == 'TRUE' || value['Mandatory'] == 'true' || value['Mandatory'] == 'True')) {
            fControl.mandatory = true;
        }
        else {
            fControl.mandatory = false;
        }

        if (value['FieldType'] == 'XXX' || value['FieldType'] == '') {
            fControl.mandatory=false;
        }
        else if ((value['FieldType']=='RDO' || value['FieldType']=='CHK') && (typeof value['Options']=='undefined' || value['Options']==null)) {
            fControl.mandatory=false;
        } 

        if (mandatory == "0") {
            fControl.mandatory = false;
        }

        fControl.ScriptOnEntry = surveyForm.prototype.ConvertHTMLSpecialToText(decodeURI(value['ScriptOnEntry']));
        fControl.ScriptOnValidate = surveyForm.prototype.ConvertHTMLSpecialToText(decodeURI(value['ScriptOnValidate']));
        fControl.ScriptOnExit = surveyForm.prototype.ConvertHTMLSpecialToText(decodeURI(value['ScriptOnExit']));
        fControl.SetInstructions(value['Instructions']);

        varFormView += fControl.header(fno) + fControl.display(fno) + fControl.footer(fno);

        if (isVisible) {
            varFormControls[fno] = fControl;
        }
        else {
            varConnectedFormControls[fno] = fControl;
        }

    });

    this.formControls = varFormControls;
    this.formView = varFormView;

    this.connectedFormControls = varConnectedFormControls;
    this.connectedFormView = varConnectedFormView;
}

surveyForm.prototype.setPage = function (pageNo) {
    $('.counter_text').text(pageNo + ' of ' + this.formControls.length);
    this.qno = pageNo;
}

surveyForm.prototype.NextPage = function () {

    //Validate current page
    if (this.validate(this.qno) && varFormControls[this.qno].onValidate()) {
        if (this.qno <= (this.formControls.length - 2)) {

            var jumpQ = varFormControls[this.qno].onExit();
            varFormControls[this.qno].nextqno = jumpQ;
            var currentQ = this.qno;

            this.qno += jumpQ;

            this.refreshPage();

            varFormControls[this.qno].prevqno = currentQ;
        }

        else {
            this.saveData();
        }
    }
}

surveyForm.prototype.PrevPage = function () {
    if (this.qno > 1) {
        if (varFormControls[this.qno].prevqno != null) {
            var prevQ = varFormControls[this.qno].prevqno;
            this.qno = prevQ;
        }
        else {
            this.qno--;
        }

        this.refreshPage();
    }
}

surveyForm.prototype.GetValue = function (id) {

    var matchedValue = '';

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                matchedValue = control.value;
            }
        }
    });

    return matchedValue;
}

surveyForm.prototype.SetValue = function (id, value) {

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.setValues(value);
            }
        }
    });
}

surveyForm.prototype.SetInstructions = function (id, text) {

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.SetInstructions(text);
                control.DisplayInstructions();
            }
        }
    });
}

surveyForm.prototype.Getqno = function(id) {

    var matchedValue = -1;

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                matchedValue = control.qno;
            }
        }
    });

    return matchedValue;
}

surveyForm.prototype.HideOptions = function (id, optionsToHide) {

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.HideOptions(optionsToHide);
            }
        }
    });
}

surveyForm.prototype.ShowOptions = function (id, optionsToHide) {

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.ShowOptions(optionsToHide);
            }
        }
    });
}

surveyForm.prototype.ShowOptionsAll = function (id) {
    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.ShowOptionsAll();
            }
        }
    });
}

surveyForm.prototype.HideOptionsAll = function (id) {
    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.HideOptionsAll();
            }
        }
    });
}

surveyForm.prototype.HideAllQuestions = function (id, iter) {

    var pageMove = this.qno;
    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                // this.qno=control.qno+1;
                pageMove = pageMove + iter;
                // sf.setPage(control.qno);
                //  control.HideQuestions(id);
            }
        }
    });

    this.qno = pageMove;
    // sf.refreshPage();
}


surveyForm.prototype.number_validation = function (id, min, max) {

    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                control.number_validation(min, max);
            }
        }
    });
}


surveyForm.prototype.ShowQuestions = function (id) {

    var pageMove = this.qno;
    var formPointer = this.formControls;

    $.each(this.formControls, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                pageMove = control.qno;
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

        if (index > 0) {

            if (id == control.id) {
                // this.qno=control.qno+1;
                pageMove = pageMove - 1;
                // sf.setPage(control.qno);
                //  control.HideQuestions(id);
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

surveyForm.prototype.setRecordingValue = function (id, filename, path) {

    var connectedFormPointer = this.connectedFormControls;

    $.each(connectedFormPointer, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                // this.qno=control.qno+1;
                sf.searchField(control.id, filename, path);
                //     control.value=filename;
                // sf.setPage(control.qno);
                //  control.HideQuestions(id);
            }
        }
    });

    var formPointer = this.formControls;

    $.each(formPointer, function (index, control) {

        if (index > 0) {

            if (id == control.id) {
                // this.qno=control.qno+1;
                //  control.value=filename;
                sf.searchField(control.id, filename, path);

                //  setValues
                // sf.setPage(control.qno);
                //  control.HideQuestions(id);
            }
        }
    });
}


surveyForm.prototype.refreshPage = function () {
    if (varFormControls[this.qno]) {
        try {
            varFormControls[this.qno].onEntry();
        } catch (e) {
            alert("OnEntry fails for control index: " + this.qno + ". Stack trace: " + e.stack);
        }

        $('.counter_text').text(this.qno + ' of ' + (this.formControls.length - 1));
        $('.formControl').hide();
        var name = varFormControls[this.qno].id;
        $('.formControl#field_' + name).show();
    }
}

surveyForm.prototype.validate = function (index) {
    if (varFormControls[index].mandatory && (varFormControls[index].value == null || varFormControls[index].value == '')) {
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

    for (var i = 0; i <= varFormControls.length - 2; i++) {
        finalData[i] = new Object();
        finalData[i].id = varFormControls[i + 1].id;
        finalData[i].value = varFormControls[i + 1].value;
        finalData[i].ResponseTime = varFormControls[i + 1].ResponseTime;
    };


    for (var i = 1; i < varConnectedFormControls.length; i++) {
        finalData[varFormControls.length + (i - 2)] = new Object();
        finalData[varFormControls.length + (i - 2)].id = varConnectedFormControls[i].id;
        finalData[varFormControls.length + (i - 2)].value = varConnectedFormControls[i].value;
        finalData[varFormControls.length + (i - 2)].ResponseTime = varConnectedFormControls[i].ResponseTime;
    };

    var save_final_data = JSON.stringify(finalData);

    console.log(save_final_data);
    if (typeof (android) != "undefined" && android != null) {
        android.setData(save_final_data);
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




surveyForm.prototype.updateValues = function(updateValues) 
{

  var updatedValues=JSON.parse(updateValues);

  $.each(updatedValues, function(key, value) {
    sf.searchField(value.id,value.value,'');
  //  var qno=sf.Getqno(value.id);
  //  $('input[name="field_'+value.id+'"]').val(value.value);
  //  varFormControls[qno].value=value.value;
  });

  sf.refreshPage();
}


surveyForm.prototype.searchField = function(id,value,path) 
{
  //  varFormControls[fno].value;
  $.each(this.formControls, function(key, control) {
    
    if (key>0) {
      if (id==control.id && control.type!="bodyText") {
        varFormControls[control.qno].setValues(value,path);
        varFormControls[control.qno].value=value;
      }
    }
  });
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

