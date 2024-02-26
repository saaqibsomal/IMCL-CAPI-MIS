function dropdownControls (fno, id, title, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "dropdown";
    this.optionData = optionData;
   // this.val='';
}

dropdownControls.prototype = Object.create(formControl.prototype); 
dropdownControls.prototype.constructor = dropdownControls;

//dropdownControls.prototype.constructor = dropdownControls.prototype;


dropdownControls.prototype.display = function() {
    var field = this.displayLabel();

   // field += '<div>';
    rId=this.id;
    rName=this.name;
    rType=this.type;

    field+='<select id="'+rId+'_dropdown" class="form-control" name="'+this.name+'" onchange="dropdownControls.prototype.getValues(this,'+this.qno+')">';

    field+='<option></option>';

    if (typeof (this.optionData) != "undefined") {
        $.each(this.optionData, function (key, value) {

            field += '<option value="' + (key + 1) + '">' + value["Option"] + '</option>';

            //  field += '<input type="'+rType+'" name="'+rName+'" id="'+rId+'_'+value['Id']+'">'+value['Option'];
        });
    }
  //  field += '</select></div>';

    field += '</select>';

    return field;
}


dropdownControls.prototype.getValues= function(obj,fno) {
    
    varFormControls[fno].value=obj.value;

    
}

