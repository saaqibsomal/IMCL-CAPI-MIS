function numberCounterControls(fno, id, title) {
    formControl.call(this, fno, id, title);
    this.type = "text";
    this.counter=0;
}
numberCounterControls.prototype = Object.create(formControl.prototype); 
numberCounterControls.prototype.constructor = numberCounterControls;

numberCounterControls.prototype.display = function() {
    var field = this.displayLabel();

    var qNum=this.qno;
//    var counter=0;

   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="numberCounterControls.prototype.getValues(this,'+this.qno+')"></div>';
    field += '<input class="form-control" type="'+this.type+'" name="'+this.name+'" value="0" readonly id="field_'+this.id+'_text" >';
    field += '<button id="btn_Counter'+this.id+'" type="button" onClick="numberCounterControls.prototype.incCounter('+qNum+')">Counter</button>';
    return field;
}



numberCounterControls.prototype.getValues= function() {	
	varFormControls[fno].value=this.counter;	
}


numberCounterControls.prototype.incCounter= function(iter) {


	var currentQuestionId=varFormControls[iter].id;

	var currentCounter=$('#field_'+currentQuestionId+'_text').val();
	currentCounter++;
	$('#field_'+currentQuestionId+'_text').val(currentCounter);
	varFormControls[iter].value=currentCounter;
/*	Counter.inc('field_'+this.id+'_text');
var counter;	
//	this.counter=this.counter+1;
	$('#field_'+this.id+'_text').val(counter); */
}




numberCounterControls.prototype.setValues = function(value) {

    $("#field_'+this.id+'_text").val(value);

 }