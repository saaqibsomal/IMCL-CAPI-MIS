function imageLoadControls(fno, id, title, controlType) {
    formControl.call(this, fno, id, title);
    this.type = "text";
    this.controlType=controlType;
    this.OnValidateScript = "";
}
imageLoadControls.prototype = Object.create(formControl.prototype); 
imageLoadControls.prototype.constructor = imageLoadControls;

imageLoadControls.prototype.display = function() {
    var field = this.displayLabel();

    var cType=this.controlType;
   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="imageLoadControls.prototype.getValues(this,'+this.qno+')"></div>';
   // field += '<input class="form-control txt-fnt-mob" type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="imageLoadControls.prototype.getValues(this,'+this.qno+','+cType+')">';
    	
   	var ctrlId='field_'+this.id+'_image';

    	field+='<div id="'+ctrlId+'" class="ctrlIdimg" onclick="imageLoadControls.prototype.setImage('+this.id+')"><img src="./img/default.jpg"></div>';

    return field;
}



imageLoadControls.prototype.getValues= function(obj,fno,cType) {

    if (cType)
    {
		buildData.varFormControls[fno].value=obj.value;
	}
	else
	{
        buildData.varConnectedFormControls[fno].value = obj.value;
	}
}


imageLoadControls.prototype.setImage = function(controlId) {
	android.imageFunction(controlId);
}


imageLoadControls.prototype.setValues = function(value) {

 // $('#field_'+this.id+'_image img').attr("src","/storage/sdcard0/DSDS"+value);

 //	android.alert("/storage/sdcard0/DSDS"+value);
 //	android.alert('#field_'+this.id+'_image img');

 	//android.alert(path);
    var path = sf.SurveyMediaDataUrl;
    $('#field_' + this.id + '_image').children("img").attr("src", path + value);
}

/*
imageLoadControls.prototype.onValidate = function() {
	//this.OnValidateScript="if(this.qno==5) return false; else return true;";
	//return eval(this.OnValidateScript);
	//if (this.qno==5) {
	//	surveyForm.prototype.GotoPage(20);
	//}
  return true;
}

*/

/*
imageLoadControls.prototype.onExit = function() {
	if (this.qno==5) {
		return 20;
	}
	return 1;
}*/

/*

imageLoadControls.prototype.number_validation = function(min,max) {
  var ctlId = this.id;

  var max_val=max+'';

  $("#field_" + ctlId+'_text').mask(max_val);

 } */

/*

 imageLoadControls.prototype.setValues = function(value) {

 	$('#field_'+this.id+'_text').val(value);

 } */


