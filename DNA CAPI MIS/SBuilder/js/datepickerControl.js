function datepickerControls(fno, id, title) {
    formControl.call(this, fno, id, title);
    this.type = "text";
}
datepickerControls.prototype = Object.create(formControl.prototype); 
datepickerControls.prototype.constructor = datepickerControls;

datepickerControls.prototype.display = function() {
    var field = this.displayLabel();

   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="datepickerControls.prototype.getValues(this,'+this.qno+')"></div>';
    field += '<input class="form-control datepicker" type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="datepickerControls.prototype.getValues(this,'+this.qno+')">';
    field += '<script>$(function() { $(".datepicker").datepicker(); }); </script>';
    return field;
}

datepickerControls.prototype.getValues= function(obj,fno) {
    buildData.varFormControls[fno].value=obj.value;    
}

datepickerControls.prototype.setValues = function (value) {
    var controlId = this.id;
    $('#field_' + controlId + '_text').val(value);
    $(".datepicker").datepicker();
}
