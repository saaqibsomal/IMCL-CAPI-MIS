function radioboxControls (fno, id, title, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "radio";
    this.optionData = optionData;


    $.each(this.optionData, function(index,value) {
      this.value = 0;
    if (typeof value['IncludeField'] != 'undefined') {
      this.connectedField = value['IncludeField'];        
    }

    else {
      this.connectedField = ''; 
    }
  });
}



radioboxControls.prototype = Object.create(formControl.prototype); 
radioboxControls.prototype.constructor = radioboxControls;

radioboxControls.prototype.display = function() {
    var field = this.displayLabel();

 //   field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;
    qno=this.qno;
  /*  var textFieldValue=sf.getValue('field_2839');

  if (textFieldValue=='2') {
    this.optionData.splice(1,1);
  }
*/


    if (this.optionData!=null) {
      field+='<div class="mrradio">';
        $.each(this.optionData, function(key, value) {

          field += '<div><input type="'+rType+'" name="'+rName+'" id="_'+rId+'_'+value['Id']+'" onClick="radioboxControls.prototype.onChange(this,'+qno+','+key+')" value="'+value['Id']+'"><label for="_'+rId+'_'+value['Id']+'" class="cus-label"><span></span>'+value['Option']+'</label></div>';
        });
      field+='</div>';

    } 

    field+='<div id="radio_connected_'+qno+'"></div>'; 

//    field += '</div>';

    return field;
}



radioboxControls.prototype.onChange= function(obj,iter,itemNo) {

    varFormControls[iter].value=obj.value;

    var actionMove = varFormControls[iter].optionData[itemNo].connectedField;

    var Ids=$('#_'+varFormControls[iter].id+'_'+varFormControls[iter].optionData[itemNo].Id);

    var optionChecked=Ids.is(":checked");



    if (actionMove != "") {
      
      if (optionChecked) {
            
            $('#radio_connected_'+iter).html($('#field_'+actionMove).html());

         //   alert($('#field_'+actionMove).html());
         //   alert(actionMove);
       //   $('#field_'+actionMove+'_text').html
            //Uncheck all
      }     
      
    }

    else {
      $('#radio_connected_'+iter).html('');
      
    }

    /*
    var Ids=$('#field_'+varFormControls[fldNo].id+'_'+varFormControls[fldNo].optionData[itemNo].Id);

    var fieldName=varFormControls[fldNo].id;


    var optionChecked=Ids.is(":checked");

    var actionType = varFormControls[fldNo].optionData[itemNo].type;  */
}

radioboxControls.prototype.ShowOptionsAll = function() {
  var ctlId = this.id;
  $.each(this.optionData, function (key, value) {
      $("#_" + ctlId + "_" + value.Id).parent().show();
//    $("#_" + ctlId + "_" + value.Id).show();
  });
  
}

radioboxControls.prototype.HideOptions = function(optionsToHide) {
  var options = optionsToHide.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().hide();
  //  $("#_" + ctlId + "_" + value).hide();
  });
}

radioboxControls.prototype.ShowOptions = function(optionsToHide) {
  var options = optionsToHide.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().show();
  //  $("#_" + ctlId + "_" + value).show();
  });
}



radioboxControls.prototype.HideOptionsAll = function() {
  var ctlId = this.id;
  $.each(this.optionData, function (key, value) {
      $("#_" + ctlId + "_" + value.Id).parent().hide();
  //      $("#_" + ctlId + "_" + value.Id).hide();
  });
}




/****************Hide Questions***********/

/*
radioboxControls.prototype.ShowOptionsAll = function() {
  var ctlId = this.id;
  $.each(this.optionData, function (key, value) {
      $("#_" + ctlId + "_" + value.Id).parent().show();
  });
  
} */

radioboxControls.prototype.HideQuestions = function(id) {
//  var options = optionsToHide.split(',');
 // var ctlId = this.id;
//  $.each(options, function (key, value) {
      $("#field_" + id).hide();
 // });
}

/*
radioboxControls.prototype.ShowOptions = function(optionsToHide) {
  var options = optionsToHide.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().show();
  });
}

*/


radioboxControls.prototype.setValues = function(value) {

  
    $('#_'+this.id+'_'+value).prop('checked',true);
  

 }