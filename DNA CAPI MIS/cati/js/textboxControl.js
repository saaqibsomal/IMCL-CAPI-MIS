function textboxControls(fno, id, title,controlType) {
    formControl.call(this, fno, id, title);
    this.type = "text";
    this.controlType=controlType;
    this.OnValidateScript = "";
}
textboxControls.prototype = Object.create(formControl.prototype); 
textboxControls.prototype.constructor = textboxControls;

textboxControls.prototype.display = function() {
    var field = this.displayLabel();

    var cType=this.controlType;
   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="textboxControls.prototype.getValues(this,'+this.qno+')"></div>';
    field += '<input class="form-control txt-fnt-mob" type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="textboxControls.prototype.getValues(this,'+this.qno+','+cType+')">';
    return field;
}



textboxControls.prototype.getValues= function(obj,fno,cType) {

	if (cType) {
		varFormControls[fno].value=obj.value;
	}

	else {
		varConnectedFormControls[fno].value=obj.value;
	}
	

	
}

/*
textboxControls.prototype.onValidate = function() {
	//this.OnValidateScript="if(this.qno==5) return false; else return true;";
	//return eval(this.OnValidateScript);
	//if (this.qno==5) {
	//	surveyForm.prototype.GotoPage(20);
	//}
  return true;
}

*/

/*
textboxControls.prototype.onExit = function() {
	if (this.qno==5) {
		return 20;
	}
	return 1;
}*/

/*

textboxControls.prototype.number_validation = function(min,max) {
  var ctlId = this.id;

  var max_val=max+'';

  $("#field_" + ctlId+'_text').mask(max_val);

 } */



 textboxControls.prototype.setValues = function(value) {

 	$('#field_'+this.id+'_text').val(value);

 }