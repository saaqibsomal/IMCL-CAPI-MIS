function checkboxControls(fno, id, title, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "checkbox";
    this.value='';
    this.name=id;
    this.optionData = optionData;

    if (typeof (this.optionData) != "undefined") {
        $.each(this.optionData, function (index, value) {
            this.value = 0;
            if (typeof value['Type'] != 'undefined') {
                this.type = value['Type'];
            }

            else if (id == 2842 && index == 3) {
                this.type = 'NONE';
            }

            else {
                this.type = '';
            }
        });
    }

    if (typeof (this.optionData) != "undefined") {
        $.each(this.optionData, function (index, value) {
            this.value = 0;
            if (typeof value['IncludeField'] != 'undefined') {
                this.connectedField = value['IncludeField'];
            }

            else {
                this.connectedField = '';
            }
        });
    }
}




checkboxControls.prototype = Object.create(formControl.prototype); 
checkboxControls.prototype.constructor = checkboxControls;

checkboxControls.prototype.display = function(index) {
    var field = this.displayLabel(index);

 //   field += '<div><input type="'+this.type+'" name="'+this.name+'" id="'+this.id+'"></div>';
 //	field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    var fldNo = this.qno;

    this.optionData;

    field += '<div class="mmchkbx">';

    if (typeof (this.optionData) != "undefined") {
        $.each(this.optionData, function (key, value) {

            if (typeof value['Type'] == 'undefined') {
                field += '<div><input type="' + rType + '" name="' + rName + '" id="field_' + rId + '_' + value['Id'] + '" value="' + value['Id'] + '" onClick="checkboxControls.prototype.onChange(this, ' + fldNo + ',' + key + ')"><label for="field_' + rId + '_' + value['Id'] + '" class="cus-label"><span></span>' + value['Option'] + '</label></div>';
            }

            else {
                field += '<div><input type="' + rType + '" name="' + rName + '" id="field_' + rId + '_' + value['Id'] + '" onClick="checkboxControls.prototype.onChange(this, ' + fldNo + ',' + key + ')"><label for="field_' + rId + '_' + value['Id'] + '" class="cus-label"><span></span>' + value['Option'] + '</label></div>';
            }

            //	field += '<input type="'+rType+'" name="'+rName+'" value="'+(key+1)+'">'+value['Option'];
        });
    }
    field += '</div>';

    field += '<div style="margin-bottom: 5px;"></div>';

    field+='<div id="check_connected_'+fldNo+'"></div>'; 


    return field;
}


checkboxControls.prototype.onChange = function(obj, fldNo, itemNo) {

	//varFormControls[fldNo].optionData[itemNo].value = obj.checked ? 1 : 0;

	//var val=$('#field_Q5_Q5_4')

	var Ids=$('#field_'+varFormControls[fldNo].id+'_'+varFormControls[fldNo].optionData[itemNo].Id);

	var fieldName=varFormControls[fldNo].id;

	var optionChecked=Ids.is(":checked");

	var actionType = varFormControls[fldNo].optionData[itemNo].type;

	var actionMove = varFormControls[fldNo].optionData[itemNo].connectedField;


 //   var res = values.split(",");
  //  res.splice(res.length-1,1)


    for (var i = 0 ; i < res.length-1; i++) {
    	chk_value[res[i]]=1;
    };


 	/*	$(".formControl input[type='checkbox']").is(':checked');

		var values = "";
 		$.each(this.optionData,function(index,value) {
 		//	if (this.chk_value[index] == 1) {
 				values += value['Id']+",";
 		//	}
 		});
 		this.value = values;
	*/

	//$(".formControl input[type='checkbox']").on('change',function() {


		 if (actionMove != "") {
      
		      if (optionChecked) {
	            
		        $('#check_connected_'+fldNo).html($('#field_'+actionMove).html());

			         //   alert($('#field_'+actionMove).html());
			         //   alert(actionMove);
			       //   $('#field_'+actionMove+'_text').html
			            //Uncheck all
			  }

			  else {
			      $('#check_connected_'+fldNo).html('');
			      
			    }     
		      
		  }

	    



		if (actionType == "NONE") {
        	if (optionChecked) {
        		$('input[name="'+fieldName+'"]').prop('checked',false);
	            $('input[name="'+fieldName+'"]').prop('disabled',true);
	            Ids.prop('checked',true);
	            Ids.prop('disabled',false);
	            this.flushValues(fldNo);

        		//Uncheck all
			}			
			else {
				$('input[name="'+fieldName+'"]').prop('checked',false);
	            $('input[name="'+fieldName+'"]').prop('disabled',false);
				//Do nothing
			}
		}  if (actionType == "ALL") {
        	if (optionChecked) {
        		this.getValues(fldNo,fieldName);
        		$('input[name="'+fieldName+'"]').prop('checked',true);

        		//Check all
			}			
			else {
				this.flushValues(fldNo);
				$('input[name="'+fieldName+'"]').prop('checked',false);
				//Uncheck all
			}
		} 

		
		else {
        	//if (optionChecked) {

        		this.getValues(fldNo,fieldName);
        		
        		//Uncheck checkbox of type NONE
			//}			
		//	else {
				//Uncheck checkbox of type ALL
		//	}

		}
		
	//	}); 	

 	/*	var values = "";
 		checkboxControls.prototype.optionData.each() {
 			if (this.value = 1) {
 				values += this.id+",";
 			}
 		}
 		checkboxControls.prototype.value = values;

		if (checkboxControls.prototype.optionData[this.id].type == "NONE") {
        	if (this.checked) {
        		//Uncheck all
			}			
			else {
				//Do nothing
			}
		} else if (checkboxControls.prototype.optionData[this.id].type == "ALL") {
        	if (this.checked) {
        		//Check all
			}			
			else {
				//Uncheck all
			}
		} else {
        	if (this.checked) {
        		//Uncheck checkbox of type NONE
			}			
			else {
				//Uncheck checkbox of type ALL
			}

		}
	*/	
    /*      if (this.checked) {
            $('input[name="'+this.name+'"]').prop('checked',false);
            $('input[name="'+this.name+'"]').prop('disabled',true);
            $('#'+this.id).prop('checked',true);
            $('#'+this.id).prop('disabled',false);        
          }

          else {
            $('input[name="'+this.name+'"]').prop('checked',false);
            $('input[name="'+this.name+'"]').prop('disabled',false);
          }
      */    
	 /*	this.values=value;
		this.chk_value=chk_value; */
 }

 checkboxControls.prototype.getValues= function(index,name) {
 	varFormControls[index].value='';
	$.each($('input[name="'+name+'"]:checked'),function(iter,value) {
	//	console.log(value.val());
		varFormControls[index].value+=this.value+',';
	});
 }


checkboxControls.prototype.flushValues= function(index) {
 	varFormControls[index].value='';	
}
/*
checkboxControls.prototype.setNone = function(index) {
	$('input[none='none']').click(function() {
		alert('hello world');
	});
} */





checkboxControls.prototype.setValues = function(value) {

	var getValues=value.split(',');
	var controlId=this.id;

 	$.each(getValues,function(iter,conValue) {
 		$('#field_'+controlId+'_'+conValue).prop('checked',true);
 	});

 }