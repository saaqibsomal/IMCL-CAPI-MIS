function sectionControl(sectionName) {
  this.sectionName=sectionName;
/*  this.qno = fno;  
  this.nextqno=null;
  this.prevqno=null;
  this.value = null;
  this.visible=true;
  this.title=title;  
  this.type = "text";
  this.id=id;
  this.name='field_'+id;
  this.mandatory = false; 
  this.uiElement = null; 
  this.OnValidateScript = "";
  */
}

sectionControl.prototype.constructor = sectionControl.prototype;

sectionControl.prototype.sectionHeader = function() {
  //  var field='<div class="form-group sectionControl" id="field_'+this.id+'">';
      var field='<div class="form-group sectionControl" id="'+this.sectionName+'">';

    return field;
}
sectionControl.prototype.sectionFooter = function() {
    var field='</div>';
    return field;
}

/*
sectionControl.prototype.displayInnerForms = function() {
    var field='<label>'+this.title+'</label>';
  //  this.value=this.getValues(-1);
    return field;
}





sectionControl.prototype.onExit = function() {
    return 1;
}


*/



