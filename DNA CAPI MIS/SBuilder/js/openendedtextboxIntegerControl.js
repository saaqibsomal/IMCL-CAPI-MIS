function openendedIntegerControl (fno, id, title, questionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridopenendedchoice";
    this.questionData= questionData;
    this.value=[];
    this.optionValue = [];

    if (this.questionData != null) {
        this.optionValue = new Array(this.questionData.length);
    }
}

openendedIntegerControl.prototype = Object.create(formControl.prototype); 
openendedIntegerControl.prototype.constructor = openendedIntegerControl;

openendedIntegerControl.prototype.display = function() {
    var field = this.displayLabel();

    field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    questions=this.questionData;
    textbox=this.textbox;

    if (questions != null) {
        $.each(questions, function (key, value) {

            field += '<p class="gridLabel">' + value['Option'] + '</p>';

            field += '<input class="form-control txt-fnt-mob" type="number" id="' + value['Id'] + '" name="" onblur="openendedIntegerControl.prototype.getValues(this,' + qNo + ',' + key + ')" >';
        });
    }
    field += '</div>';
    return field;
}



openendedIntegerControl.prototype.getValues = function (obj, qno, subQnoId) {
    var opt = buildData.varFormControls[qno].optionValue;

    buildData.varFormControls[qno].optionValue[subQnoId] = obj.value;

    buildData.varFormControls[qno].value = buildData.varFormControls[qno].optionValue.join(",");
}


openendedIntegerControl.prototype.onValidate = function () {
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
