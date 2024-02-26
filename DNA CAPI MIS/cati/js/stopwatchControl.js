function stopwatchControls(fno, id, title) {
    formControl.call(this, fno, id, title);
    this.type = "stopwatch";
}
stopwatchControls.prototype = Object.create(formControl.prototype); 
stopwatchControls.prototype.constructor = stopwatchControls;

stopwatchControls.prototype.display = function() {
    var field = this.displayLabel();

    var currentQno=this.qno;

//    field+='<div><h1>Stopwatch</h1><span id="sw_h'+this.id+'">00</span>:<span id="sw_m'+this.id+'">00</span>:<span id="sw_s'+this.id+'">00</span>:<span id="sw_ms'+this.id+'">00</span><br/><br/><input type="button" value="Start" id="sw_start'+this.id+'" onClick="stopwatchControls.prototype.swStart()" /><input type="button" value="Pause" id="sw_pause'+this.id+'" onClick="stopwatchControls.prototype.swPause()" /><input type="button" value="Stop"  id="sw_stop'+this.id+'" onClick="stopwatchControls.prototype.swStop()" /><input type="button" value="Reset" id="sw_reset'+this.id+'" onClick="stopwatchControls.prototype.swReset()" /><br/><br/><span id="sw_status'+this.id+'">Idle</span></div>';
    field+='<div><h1>Stopwatch</h1><span id="sw_h">00</span>:<span id="sw_m">00</span>:<span id="sw_s">00</span>:<span id="sw_ms">00</span><br/><br/><input type="button" value="Start" id="sw_start" onClick="stopwatchControls.prototype.swStart('+currentQno+')" /><input type="button" value="Pause" id="sw_pause" onClick="stopwatchControls.prototype.swPause('+currentQno+')" /><input type="button" value="Stop"  id="sw_stop" onClick="stopwatchControls.prototype.swStop('+currentQno+')" /><input type="button" value="Reset" id="sw_reset" onClick="stopwatchControls.prototype.swReset('+currentQno+')" /><br/><br/><span id="sw_status">Idle</span></div>';



   // field += '<div><input type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="stopwatchControls.prototype.getValues(this,'+this.qno+')'+this.id+'"></div>';
 //   field += '<input class="form-control" type="'+this.type+'" name="'+this.name+'" id="field_'+this.id+'_text" onblur="stopwatchControls.prototype.getValues(this,'+this.qno+')">';
    return field;
}







stopwatchControls.prototype.getValues= function(obj,fno) {


	varFormControls[fno].value=obj.value;	
}

stopwatchControls.prototype.swStart= function(qNo) {
	$.StopWatch.startTimer('sw');
}    


stopwatchControls.prototype.swStop= function(qNo) {
	$.StopWatch.stopTimer();
} 

stopwatchControls.prototype.swReset= function(qNo) {
    $.StopWatch.resetTimer();
} 


stopwatchControls.prototype.swPause= function(qNo) {
    $.StopWatch.pauseTimer();
}    

