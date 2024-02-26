function openendedTextControl (fno, id, title, questionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridopenendedchoice";
    this.questionData = questionData;
    this.value = [];
    this.optionValue = [];

    if (this.questionData != null) {
        this.optionValue = new Array(this.questionData.length);
    }
}

openendedTextControl.prototype = Object.create(formControl.prototype); 
openendedTextControl.prototype.constructor = openendedTextControl;

openendedTextControl.prototype.display = function() {
    var field = this.displayLabel();

    field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    questions=this.questionData;
  //  textbox=this.textbox;

    if (questions != null) {
        $.each(questions, function (key, value) {
            //textbox[key]=new textboxControls(key+1,value['Id'],value['FieldName']);

            //   field += textbox[key].header(key) + textbox[key].display(key) + textbox[key].footer(key);  
            //field += textbox[key].display(key);
            field += '<p class="gridLabel">' + value['Option'] + '</p>';

            field += '<input class="form-control txt-fnt-mob" type="text" id="' + value['Id'] + '" name="" onblur="openendedTextControl.prototype.getValues(this, ' + qNo + ',' + key + ')" >';
        });
    }
    field += '</div>';

    return field;
}


openendedTextControl.prototype.getValues = function (obj, qno, subQnoId) {
    var opt = buildData.varFormControls[qno].optionValue;

    buildData.varFormControls[qno].optionValue[subQnoId] = obj.value;

    buildData.varFormControls[qno].value = buildData.varFormControls[qno].optionValue.join(",");    
}


openendedTextControl.prototype.onValidate = function () {
    if (this.questionData != null) {
        if (this.mandatory) {
            var nonempty = 0;
            var inpvalues = this.value.split(",");
            for (var i = 0; i < inpvalues.length; i++) {
                if (inpvalues[i].trim().length > 0) nonempty++;
            }
            return this.questionData.length == nonempty;
        }
    }
    return true;
}
