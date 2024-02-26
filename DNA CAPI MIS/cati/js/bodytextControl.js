function bodytext(fno, id, title) {
    formControl.call(this, fno, id, title);
    this.type = "bodyText";
    this.title=title;   
}
bodytext.prototype = Object.create(formControl.prototype); 
bodytext.prototype.constructor = bodytext;

bodytext.prototype.display = function() {
  //  var field = this.displayLabel(index);

   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="bodytext.prototype.getValues(this,'+this.qno+')"></div>';
   field='<span>'+this.title+'</span>';

    return field;
}



bodytext.prototype.getValues= function(obj,fno) {
	
	varFormControls[fno].value=obj.value;

	
}



