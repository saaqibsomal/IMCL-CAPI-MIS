function singlechoicedropdownControl (fno, id, title, topicData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice";
    this.optionData = optionData;
    this.topicData = topicData;
    this.value=[];
   // this.val='';
}

singlechoicedropdownControl.prototype = Object.create(formControl.prototype); 
singlechoicedropdownControl.prototype.constructor = singlechoicedropdownControl;

singlechoicedropdownControl.prototype.display = function(index) {
    var field = this.displayLabel(index);

    field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    options=this.optionData;
    questions=this.topicData;

    if (questions != null) {
        $.each(questions, function (key, value) {

            field += '<div>';

            field += '<p>' + value['FieldName'] + '</p><br />';

            field += '<select id="' + rId + '_dropdown" name="' + rName + '" onchange="singlechoicedropdownControl.prototype.getValues(this,' + qNo + ',' + (key) + ',' + value['Id'] + ',' + questions.length + ')">';

            field += '<option></option>';

            if (options != null) {
                $.each(options, function (keyOpt, valueOpt) {

                    field += '<option value="' + valueOpt['Id'] + '">' + valueOpt["Option"] + '</option>';

                    //  field += '<input type="'+rType+'" name="'+rName+'" id="'+rId+'_'+value['Id']+'">'+value['Option'];
                });
            }
            field += '</select></div>';

        });
    }
    field += '</div>';


    return field;
}


singlechoicedropdownControl.prototype.getValues= function(obj,fno,subQ,subQId,quesLength) {


   // var finalgridValues=[];

    gridValues=new Object();
    gridValues.Id=subQId;
    gridValues.Value=obj.value;

  //  finalgridValues.push(gridValues);

    
    varFormControls[fno].value.push(gridValues);

    
}

