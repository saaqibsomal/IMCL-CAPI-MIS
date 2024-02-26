function textboxControls(fno, id, title, controlType, MinLength, MaxLength, NoOfLines) {
    formControl.call(this, fno, id, title);
    this.type = "text";
    this.NoOfLines = NoOfLines;
    this.MinLength = (MinLength ? (parseInt(MinLength) > 0 ? MinLength : 0) : 0);
    this.MaxLength = (MaxLength ? (parseInt(MaxLength) > 0 ? MaxLength : 0) : 0);
    this.controlType=controlType;
}

textboxControls.prototype = Object.create(formControl.prototype); 
textboxControls.prototype.constructor = textboxControls;

textboxControls.prototype.display = function() {
    var field = this.displayLabel();

    var cType = this.controlType;
    if (this.NoOfLines > 1) {
        field += '<textarea class="txt-fnt-mob" rows="' + this.NoOfLines + '" name="' + this.name + '" id="field_' + this.id + '_text" onkeyup="textboxControls.prototype.getValues(this,' + this.qno + ',' + cType + ')"></textarea>';
    } else {
        field += '<input class="form-control txt-fnt-mob" type="' + this.type + '" name="' + this.name + '" id="field_' + this.id + '_text" onkeyup="textboxControls.prototype.getValues(this,' + this.qno + ',' + cType + ')">';
    }
    field += '<span class="txt-info" id="field_' + this.id + '_info"></span>';
    return field;
}

textboxControls.prototype.getValues= function(obj,fno,cType) {
    var ctl;
	if (cType) {
		ctl = buildData.varFormControls[fno];
	}
	else {
	    ctl = buildData.varConnectedFormControls[fno];
	}
	ctl.value = obj.value;

	var info = 'Count: ' + ctl.value.length;
	if (ctl.MinLength > 0) {
	    info += ' | Required: ' + ctl.MinLength;
	}
	if (ctl.MaxLength > 0) {
	    info += ' | Max Chars: ' + ctl.MaxLength;
	}
	$('#field_'+ctl.id+'_info').html(info);
}


textboxControls.prototype.setValues = function (value) {
    $('#field_' + this.id + '_text').val(value);
}

textboxControls.prototype.onValidate = function () {
    if (this.MinLength > 0) {
        if (this.value.length < this.MinLength) {
            sf.alert("Minimum number of characters required are " + this.MinLength.toString());
            return false;
        }
    }
    if (this.MaxLength > 0) {
        if (this.value.length > this.MaxLength) {
            sf.alert("Maximum number of characters allowed are " + this.MaxLength.toString());
            return false;
        }
    }
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
