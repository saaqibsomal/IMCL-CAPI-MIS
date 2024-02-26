function soundrecordControls(fno, id, title,endOnField,UI,autoStart) {
    formControl.call(this, fno, id, title);    
    this.type = "SRO";
    this.endOnField=endOnField;
    this.path=null;
    this.UI=UI;
    this.autoStart=autoStart;
}
soundrecordControls.prototype = Object.create(formControl.prototype); 
soundrecordControls.prototype.constructor = soundrecordControls;



soundrecordControls.prototype.display = function() {
/*    var field = this.displayLabel();

    var currentQno=this.qno;

//    field+='<div><h1>Stopwatch</h1><span id="sw_h'+this.id+'">00</span>:<span id="sw_m'+this.id+'">00</span>:<span id="sw_s'+this.id+'">00</span>:<span id="sw_ms'+this.id+'">00</span><br/><br/><input type="button" value="Start" id="sw_start'+this.id+'" onClick="soundrecordControls.prototype.swStart()" /><input type="button" value="Pause" id="sw_pause'+this.id+'" onClick="soundrecordControls.prototype.swPause()" /><input type="button" value="Stop"  id="sw_stop'+this.id+'" onClick="soundrecordControls.prototype.swStop()" /><input type="button" value="Reset" id="sw_reset'+this.id+'" onClick="soundrecordControls.prototype.swReset()" /><br/><br/><span id="sw_status'+this.id+'">Idle</span></div>';
    field+='<div><h1>Stopwatch</h1><span id="sw_h">00</span>:<span id="sw_m">00</span>:<span id="sw_s">00</span>:<span id="sw_ms">00</span><br/><br/><input type="button" value="Start" id="sw_start" onClick="soundrecordControls.prototype.swStart('+currentQno+')" /><input type="button" value="Pause" id="sw_pause" onClick="soundrecordControls.prototype.swPause('+currentQno+')" /><input type="button" value="Stop"  id="sw_stop" onClick="soundrecordControls.prototype.swStop('+currentQno+')" /><input type="button" value="Reset" id="sw_reset" onClick="soundrecordControls.prototype.swReset('+currentQno+')" /><br/><br/><span id="sw_status">Idle</span></div>';



   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="soundrecordControls.prototype.getValues(this,'+this.qno+')'+this.id+'"></div>';
 //   field += '<input class="form-control" type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="soundrecordControls.prototype.getValues(this,'+this.qno+')">';
    return field;
*/
    return '';

}


soundrecordControls.prototype.setValues = function(value,path) {

 // $('#field_'+this.id+'_text').val(value);

 }


/*
soundrecordControls.prototype.stopRecording = function(id) {

} */




/*

soundrecordControls.prototype.startRecording = function(optionsToHide) {
  var options = optionsToHide.split(',');
  var ctlId = this.id;
  $.each(options, function (key, value) {
      $("#_" + ctlId + "_" + value).parent().show();
  });
}



soundrecordControls.prototype.getValues= function(obj,fno) {


	varFormControls[fno].value=obj.value;	
}

soundrecordControls.prototype.swStart= function(qNo) {
	$.StopWatch.startTimer('sw');
}    


soundrecordControls.prototype.swStop= function(qNo) {
	$.StopWatch.stopTimer();
} 

soundrecordControls.prototype.swReset= function(qNo) {
    $.StopWatch.resetTimer();
} 


soundrecordControls.prototype.swPause= function(qNo) {
    $.StopWatch.pauseTimer();
}    

*/