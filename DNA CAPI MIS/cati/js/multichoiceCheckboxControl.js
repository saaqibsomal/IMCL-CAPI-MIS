function multichoiceGridCheckboxControl (fno, id, title, questionData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridmultichoice2";
    this.optionData = optionData;
    this.questionData= questionData;
    this.value=[];
    this.radioBox=[];

   // this.val='';
}

multichoiceGridCheckboxControl.prototype = Object.create(formControl.prototype); 
multichoiceGridCheckboxControl.prototype.constructor = multichoiceGridCheckboxControl;

multichoiceGridCheckboxControl.prototype.display = function() {
    var field = this.displayLabel();

    field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    options=this.optionData;
    questions=this.questionData;
    radioBox=this.radioBox;


    var field = this.displayLabel();

 //   field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qno=this.qno;

    field+='<table border="1">';

    field+='<th>';
    if (typeof (questions) != "undefined") {
        $.each(options, function (keyL, valueL) {
            field += '<td><label>' + valueL['Option'] + '</label></td>';

        });
    }
    field+='</th>';

    if (typeof (questions) != "undefined") {
        $.each(questions, function (keyQ, valueQ) {

            field += '<tr><td>' + valueQ['FieldName'] + '</td>';

            $.each(options, function (keyR, valueR) {
                field += '<td id=><div class=""><input type="checkbox" name="' + rName + keyQ + '" id="' + rId + '_' + valueR['Id'] + keyQ + '" onClick="multichoiceGridCheckboxControl.prototype.onChange(this,' + qno + ',' + keyQ + ')" value="' + valueR['Id'] + '"></div></td>';
            });

            field += '</tr>';
        });
    }
//    field += '</div>';

    field+='</table>';



    field += '</div>';

    return field;

    



 /*   $.each(questions, function(key, value) {

        radioBox[key]=new radioboxControls(key+1,value['Id'],value['FieldName'], options);

     //   field += radioBox[key].header(key) + radioBox[key].display(key) + radioBox[key].footer(key);  
        field += radioBox[key].display(key);      
    

    });

    field += '</div>';


    return field; */
}


multichoiceGridCheckboxControl.prototype.onChange= function(obj,subQId,keyQ) {


    var abc=this.radioBox;

   // var finalgridValues=[];

    gridMultiCheckBoxValues=new Object();
    gridMultiCheckBoxValues.Id=keyQ;


   if (typeof varFormControls[subQId].value[keyQ]!='undefined') {
        var tempValue=varFormControls[subQId].value[keyQ].Value;
        gridMultiCheckBoxValues.Value=tempValue+','+obj.value;
    }

    else {
        gridMultiCheckBoxValues.Value=obj.value;
    }

   

  //  finalgridMultiCheckBoxValues.push(gridMultiCheckBoxValues);

    


    varFormControls[subQId].value[keyQ]=gridMultiCheckBoxValues;

    
}










/* END 

function multichoiceGridCheckboxControl (fno, id, title, questionData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice";   
    this.questionData= questionData;
    this.optionData = optionData;
    this.value = "";
   // this.val='';
}

radioBoxControls.prototype = Object.create(formControl.prototype); 
radioBoxControls.prototype.constructor = radioBoxControls;

//radioBoxControls.prototype.constructor = radioBoxControls.prototype;


singlechoiceradioBoxControl.prototype = Object.create(radioBoxControl.prototype); 
singlechoiceradioBoxControl.prototype.constructor = singlechoiceradioBoxControl;



singlechoiceradioBoxControl.prototype.display = function() {
    
	var field='<p>'+this.name+'</p>';
    field += this.display();
    return field;
}


*/