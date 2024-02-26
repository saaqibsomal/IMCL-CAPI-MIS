function singlechoiceGridRadioControl (fno, id, title, questionData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice2";
    this.optionData = optionData;
    this.questionData= questionData;
    this.value=[];
    this.radioBox=[];

   // this.val='';
}

singlechoiceGridRadioControl.prototype = Object.create(formControl.prototype); 
singlechoiceGridRadioControl.prototype.constructor = singlechoiceGridRadioControl;

singlechoiceGridRadioControl.prototype.display = function() {
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

    $.each(options, function(keyL, valueL) {
        field+='<td><label>'+valueL['Option']+'</label></td>';

    });

    field+='</th>';

    if (typeof (questions) != "undefined") {
        $.each(questions, function (keyQ, valueQ) {

            field += '<tr><td>' + valueQ['FieldName'] + '</td>';

            $.each(options, function (keyR, valueR) {

                var currentMultiRadioTDId = 'columnMultiRadio' + qno + '_' + keyQ + keyR;

                var currentMultiRadioId = rId + '_' + valueR['Id'] + keyQ;

                field += '<td align="center" id="' + currentMultiRadioTDId + '"><div class=""><input type="radio" name="' + rName + keyQ + '" id="' + currentMultiRadioId + '" onClick="singlechoiceGridRadioControl.prototype.getValues(this,' + qno + ',' + keyQ + ')" value="' + valueR['Id'] + '"></div></td>';
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


singlechoiceGridRadioControl.prototype.getValues= function(obj,qno,subQnoId) {


    var abc=this.radioBox;

   // var finalgridValues=[];

    gridSingleRadioValues=new Object();
    gridSingleRadioValues.Id=subQnoId;
    gridSingleRadioValues.Value=obj.value;

  //  finalgridSingleRadioValues.push(gridSingleRadioValues);    
    varFormControls[qno].value[subQnoId]=gridSingleRadioValues;

 /*   $('#'+multiRadioId).parent().css( "background-color", "red" ); */

//    obj.find('td').css('background-color', '#00ff00');

  //  $("td#"+multiRadioId).css('background-color','#00ff00');
  //  $("#"+multiRadioId).attr('bgcolor','#00ff00');
}










/* END 

function singlechoiceGridRadioControl (fno, id, title, questionData, optionData) {
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