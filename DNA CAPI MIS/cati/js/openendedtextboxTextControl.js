function openendedTextControl (fno, id, title, questionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridopenendedchoice";
    this.questionData= questionData;
    this.value=[];
    this.textbox=[];

   // this.val='';
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

    if (typeof (questions) != "undefined") {
        $.each(questions, function (key, value) {
            //textbox[key]=new textboxControls(key+1,value['Id'],value['FieldName']);

            //   field += textbox[key].header(key) + textbox[key].display(key) + textbox[key].footer(key);  
            //field += textbox[key].display(key);
            field += '<p>' + value['FieldName'] + '</p>';

            field += '<input type="text" id="' + value['Id'] + '" name="" onblur="openendedTextControl.prototype.getValues(this,' + qNo + ',' + key + ',' + value['Id'] + ')" >';
        });
    }

    field += '</div>';
    return field;
}


openendedTextControl.prototype.getValues= function(obj,ques,subQues,SubQuesNo) {


    var abc=this.textbox;

   // var finalgridValues=[];

    gridOpenEndedMultipleTextBoxesValues_text=new Object();
    gridOpenEndedMultipleTextBoxesValues_text.Id=SubQuesNo;
    gridOpenEndedMultipleTextBoxesValues_text.Value=obj.value;

  //  finalgridOpenEndedMultipleTextBoxesValues_text.push(gridOpenEndedMultipleTextBoxesValues_text);

    
    varFormControls[ques].value[subQues]=gridOpenEndedMultipleTextBoxesValues_text;

    
}



