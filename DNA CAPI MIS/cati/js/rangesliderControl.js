function rangeSliderControl(fno, id, title,min,max) {
    formControl.call(this, fno, id, title);
    this.type = "slider";
    this.min=min;
    this.max=max;
}
rangeSliderControl.prototype = Object.create(formControl.prototype); 
rangeSliderControl.prototype.constructor = rangeSliderControl;

rangeSliderControl.prototype.display = function() {
    var field = this.displayLabel();

   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="rangeSliderControl.prototype.getValues(this,'+this.qno+')"></div>';
    field += '<div id="field_'+this.id+'_slider"></div>';
    field += '<script>$(function() { $("#field_'+this.id+'_slider").slider({ range: "max", min: '+this.min+', max: '+this.max+', value: 2, slide: function(event,ui) { $("#field_'+this.id+'_slider").val(ui.value); } });  }); </script>';
    return field;
}



rangeSliderControl.prototype.getValues= function(obj,fno) {
	
	varFormControls[fno].value=obj.value;





	
}