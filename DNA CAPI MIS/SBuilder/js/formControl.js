function formControl(fno, id, title) {
    this.qno = fno;  
    this.nextqno=null;
    this.prevqno=null;
    this.value = null;
    this.visible=true;
    this.title = title;
    this.instructions = "";
    this.type = "text";
    this.id=id;
    this.name='field_'+id;
    this.mandatory = false; 
    this.uiElement = null; 
    this.OnValidateScript = "";
    this.ScriptOnEntry = "";
    this.ScriptOnValidate = "";
    this.ScriptOnExit = "";
    this.ResponseTime = 0;      //in Milliseconds
    this.Stopwatch = new Stopwatch();
}

formControl.prototype.constructor = formControl.prototype;

formControl.prototype.header = function() {
    var field='<div class="form-group formControl" id="field_'+this.id+'">';
    return field;
}
formControl.prototype.footer = function() {
    var field='</div>';
    return field;
}
formControl.prototype.displayLabel = function() {
    var field = '<label class="sh_label">' + decodeURI(this.title) + '</label>';
    field += '<label class="sh_label_instructions" id="field_' + this.id + '_instructions">' + decodeURI(this.instructions) + '</label>';
    //  this.value=this.getValues(-1);
    return field;
}
formControl.prototype.SetInstructions = function (text) {
    if (text) {
        this.instructions = text;
    }
    else {
        this.instructions = '';
    }
}
formControl.prototype.DisplayInstructions = function () {
    $('#field_' + this.id + '_instructions').text(this.instructions);
}


formControl.prototype.getValues = function() {
  var values=$('#'+'field_'+this.id+'_text').val();

  return values;
}


formControl.prototype.setValues = function (value) {
}

formControl.prototype.onValidate = function () {
    var evalValue = true;
    if (this.ScriptOnValidate.length > 0) {
        try {
            evalValue = (new Function(this.ScriptOnValidate)());
            if (typeof (evalValue) != "boolean") {
                evalValue = true;
            }
        } catch (e) {
                sf.alert("Contact Administrator: Error in script");
        }
    }
    return evalValue;
}

formControl.prototype.onExit = function () {
    var evalValue = 1;
    if (this.ScriptOnExit.length > 0) {
        try {
            evalValue = (new Function(this.ScriptOnExit)());
            if (typeof (evalValue) != "number") {
                evalValue = 1;
            }
        } catch (e) {
            sf.alert("Contact Administrator: Error in script");
        }
    }
    this.ResponseTime += this.Stopwatch.stop();
    return evalValue;
}


formControl.prototype.onEntry = function () {
    this.Stopwatch.start();
    if (this.ScriptOnEntry.length > 0) {
        Function(this.ScriptOnEntry)();
    }
}


formControl.prototype.HideOptions = function(optionsToHide) {
}

function onSROValidate(id) {
  android.startHiddenRecording(id);
}


function StopRecording() {
  android.stopHiddenRecording('5555');
}


///////////////////////////////////////////////////////////////
// Stop watch
var Stopwatch = function (options) {

    var offset,
        clock,
        interval;

    // default options
    options = options || {};
    options.delay = options.delay || 100;

    // initialize
    reset();

    function start() {
        if (!interval) {
            offset = Date.now();
            interval = setInterval(update, options.delay);
        }
    }

    function stop() {
        if (interval) {
            clearInterval(interval);
            interval = null;
            return clock;
        }
    }

    function reset() {
        clock = 0;
    }

    function update() {
        clock += delta();
    }

    function delta() {
        var now = Date.now(),
            d = now - offset;

        offset = now;
        return d;
    }

    // public API
    this.start = start;
    this.stop = stop;
    this.reset = reset;
};

function urldecode(str) {
    str = str.replace(/[%!'()*]/g, function (c) {
        return '%' + c.charCodeAt(0).toString(16);
    });
    str = decodeURIComponent(str.replace(/\+/g, '%20'));
    try {
        str = decodeURIComponent(str);
    }
    catch (e) {
    }
    return str;
}