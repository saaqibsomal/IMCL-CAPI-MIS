function singlechoiceGridControl (fno, id, title, topicData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice";
    this.optionData = optionData;
    this.topicData = topicData;
    this.value=[];
    this.dropDown=[];

   // this.val='';
}

singlechoiceGridControl.prototype = Object.create(formControl.prototype); 
singlechoiceGridControl.prototype.constructor = singlechoiceGridControl;

singlechoiceGridControl.prototype.display = function() {
    var field = this.displayLabel();

    field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qNo=this.qno;
    options=this.optionData;
    questions=this.topicData;
    dropdown=this.dropDown;


    if (questions != null) {
        $.each(questions, function (keyQ, valueQ) {

            //      dropdown[key]=new dropdownControls(key+1,value['Id'],value['FieldName'], options);

            //   field += dropdown[key].header(key) + dropdown[key].display(key) + dropdown[key].footer(key);  
            //      field += dropdown[key].display(key);  

            //name="'+rName+keyQ+'" id="'+rId+'_'+valueR['Id']+keyQ+

            field += '<label>' + valueQ['FieldName'] + '</label>';

            field += '<select id="' + rId + '_' + valueQ['Id'] + keyQ + '_dropdown" class="form-control" name="' + rName + keyQ + '" onchange="singlechoiceGridControl.prototype.getValues(this,' + qNo + ',' + keyQ + ')">';

            field += '<option></option>';

            if (options != null) {
                $.each(options, function (keyR, valueR) {

                    field += '<option value="' + (keyR + 1) + '">' + valueR["Option"] + '</option>';

                    //  field += '<input type="'+rType+'" name="'+rName+'" id="'+rId+'_'+value['Id']+'">'+value['Option'];
                });
            }
            //  field += '</select></div>';

            field += '</select>';

        });
    }
    field += '</div>';


    return field;
}


singlechoiceGridControl.prototype.getValues= function(obj,qno,subQnoId) {


    var abc=this.dropdown;

   // var finalgridValues=[];

    gridSingleDropDownValues=new Object();
    gridSingleDropDownValues.Id=subQnoId;
    gridSingleDropDownValues.Value=obj.value;

  //  finalgridSingleDropDownValues.push(gridSingleDropDownValues);

    
    buildData.varFormControls[qno].value[subQnoId]=gridSingleDropDownValues;
  
}










/*
    //////////////////////////////////////


    var abc=this.dropdown;

   // var finalgridValues=[];

    gridValues=new Object();
    gridValues.Id=subQId;
    gridValues.Value=obj.value;

  //  finalgridValues.push(gridValues);

    
    buildData.varFormControls[fno].value.push(gridValues);
*/
    











/* END 

function singlechoiceGridControl (fno, id, title, topicData, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "gridsinglechoice";   
    this.topicData= topicData;
    this.optionData = optionData;
    this.value = "";
   // this.val='';
}

dropdownControls.prototype = Object.create(formControl.prototype); 
dropdownControls.prototype.constructor = dropdownControls;

//dropdownControls.prototype.constructor = dropdownControls.prototype;


singlechoicedropdownControl.prototype = Object.create(dropdownControl.prototype); 
singlechoicedropdownControl.prototype.constructor = singlechoicedropdownControl;



singlechoicedropdownControl.prototype.display = function() {
    
	var field='<p>'+this.name+'</p>';
    field += this.display();
    return field;
}


*/