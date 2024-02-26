function singlechoicedropdownControl (fno, id, title, questionData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice";
    this.optionData = optionData;
    this.questionData= questionData;
    this.value=[];
   // this.val='';
}

singlechoicedropdownControl.prototype = Object.create(formControl.prototype); 
singlechoicedropdownControl.prototype.constructor = singlechoicedropdownControl;

singlechoicedropdownControl.prototype.display = function() {
    var field = this.displayLabel();

    field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    options=this.optionData;
    questions=this.questionData;

    if (typeof (questions) != "undefined") {
        $.each(questions, function (key, value) {

            field += '<div>';

            field += '<p>' + value['FieldName'] + '</p><br />';

            field += '<select id="' + rId + '_dropdown" name="' + rName + '" onchange="singlechoicedropdownControl.prototype.getValues(this,' + qNo + ',' + (key) + ',' + value['Id'] + ',' + questions.length + ')">';

            field += '<option></option>';

            $.each(options, function (keyOpt, valueOpt) {

                field += '<option value="' + valueOpt['Id'] + '">' + valueOpt["Option"] + '</option>';

                //  field += '<input type="'+rType+'" name="'+rName+'" id="'+rId+'_'+value['Id']+'">'+value['Option'];
            });

            field += '</select></div>';

        });
    }
    field += '</div>';
    return field;
}


singlechoicedropdownControl.prototype.getValues= function(obj,fno,subQ,subQId,quesLength) {


   // var finalgridValues=[];

    gridMultipleChoiceDropDownValues=new Object();
    gridMultipleChoiceDropDownValues.Id=subQId;
    gridMultipleChoiceDropDownValues.Value=obj.value;

  //  finalgridMultipleChoiceDropDownValues.push(gridMultipleChoiceDropDownValues);

    
    varFormControls[fno].value[subQ]=gridMultipleChoiceDropDownValues;

    
}

