function dropdownControls (fno, id, title, optionData) {
    formControl.call(this, fno, id, title);
    this.type = "dropdown";
    this.optionData = optionData;
    // this.val='';
    this.BlankDefault = false;
}

dropdownControls.prototype = Object.create(formControl.prototype); 
dropdownControls.prototype.constructor = dropdownControls;

//dropdownControls.prototype.constructor = dropdownControls.prototype;


dropdownControls.prototype.display = function() {
    var label = this.displayLabel();
    var field = '';

    rId=this.id;
    rName=this.name;
    rType=this.type;

    firstValue = "";

    if (this.BlankDefault)
        field+='<option></option>';

    $.each(this.optionData, function(key, value) {
        if (firstValue.length == 0) {
            firstValue = value.Id;
            field += '<option selected value="' + value.Id + '">' + value["Option"] + '</option>';
        } else {
            field += '<option value="' + value.Id + '">' + value["Option"] + '</option>';
        }
    });

    this.value = firstValue;

    field = '<select id="field_' + rId + '_dropdown" value="' + firstValue + '" class="form-control" name="' + this.name + '" onchange="dropdownControls.prototype.getValues(this,' + this.qno + ')">' + field + '</select>';

    return label + field;
}


dropdownControls.prototype.getValues= function(obj,fno) {
    buildData.varFormControls[fno].value=obj.value;    
}

dropdownControls.prototype.setValues = function (value) {
    var controlId = this.id;
    $('#_field' + controlId + '_dropdown').val(value);
}
