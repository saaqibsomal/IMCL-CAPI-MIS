function integerTextboxControls(fno, id, title) {
    formControl.call(this, fno, id, title);
    this.type = "number";   

}
integerTextboxControls.prototype = Object.create(formControl.prototype); 
integerTextboxControls.prototype.constructor = integerTextboxControls;

integerTextboxControls.prototype.display = function() {
   var field = this.displayLabel();

   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="textboxControls.prototype.getValues(this,'+this.qno+')"></div>';
    field += '<input class="form-control txt-fnt-mob" type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_number" onblur="integerTextboxControls.prototype.getValues(this,'+this.qno+')">';
    return field;
}



integerTextboxControls.prototype.getValues= function(obj,fno) {

	varFormControls[fno].value=obj.value;	
}


integerTextboxControls.prototype.number_validation = function(min,max) {
  var ctlId = this.id;
  var maxVal=max+'';

//  $("#field_" + ctlId+'_text').mask(max);
 
    /*  $("#field_" + ctlId+'_text').keyup(function() {

        var currentVal=parseInt($(this).val());

        if (currentVal<0 && currentVal>999999999) {

          var checkVal=parseInt($(this).val());
           $("#field_" + ctlId+'_text').val(checkVal);
        }

        
      });

*/
 
// + "_" + value.Id

}



integerTextboxControls.prototype.setValues = function(value) {

    $('#field_'+this.id+'_number').val(value);

 }



   
