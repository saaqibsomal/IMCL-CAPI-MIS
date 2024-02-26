function openendedIntegerControl (fno, id, title, questionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridopenendedchoice";
    this.questionData= questionData;
    this.value=[];
    this.textbox=[];

   // this.val='';
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

    if (typeof (questions) != "undefined") {
        $.each(questions, function (key, value) {

            field += '<p>' + value['FieldName'] + '</p>';

            field += '<input type="text" id="' + value['Id'] + '" name="" onblur="openendedIntegerControl.prototype.getValues(this,' + qNo + ',' + key + ',' + value['Id'] + ')" >';

        });
    }
    field += '</div>';


    return field;
}


openendedIntegerControl.prototype.getValues= function(obj,fno,subQId,SubQuesNo) {


    var abc=this.textbox;

   // var finalgridValues=[];

    gridOpenEndedMultipleIntegerTextBoxesValues=new Object();
    gridOpenEndedMultipleIntegerTextBoxesValues.Id=SubQuesNo;
    gridOpenEndedMultipleIntegerTextBoxesValues.Value=obj.value;

  //  finalgridOpenEndedMultipleIntegerTextBoxesValues.push(gridOpenEndedMultipleIntegerTextBoxesValues);

    
    varFormControls[fno].value[subQId]=gridOpenEndedMultipleIntegerTextBoxesValues;

    
}



